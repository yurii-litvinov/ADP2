namespace ADP2.Core

open DocUtils.YandexSheets
open MetadataSourceUtils
open FSharp.Json
open System.IO
open System

/// Part of a metadata config related to Yandex.Disk sheets.
type YandexSheetConfig =
    {
        YandexSheetFolderUrl: string
        YandexSheetFileName: string
    }

/// Implementation of metadata source for qualification works Yandex Disk tables. Required columns are
/// configured via config.json file.
/// For example, see https://disk.yandex.ru/i/qLaiHCTTzwqyuQ
type YandexDiskMetadataSource(appConfig: ApplicationConfig) =

    let readSheet (config: DataConfig) (sheet: DocUtils.Xlsx.Sheet) =
        let columns = 
            [
                config.AuthorNameColumn
                config.AdvisorColumn
                config.TitleColumn
                config.ResultColumn
            ]

        let sourceUriDefined = not (config.SourceUriColumn = "-" || config.SourceUriColumn = "")
        let doNotPublishDefined = not (config.DoNotPublishColumn = "-" || config.DoNotPublishColumn = "")

        let columns = 
            if sourceUriDefined then
                config.CommitterNameColumn :: config.SourceUriColumn :: columns
            else
                columns

        let columns = 
            if doNotPublishDefined then 
                config.DoNotPublishColumn :: columns
            else
                columns

        let createMetadata (row: Map<string, string>) =
            let sourceUri = if sourceUriDefined then row[config.SourceUriColumn] else ""
            let commiterName = 
                if sourceUriDefined && sourceUri <> "" && sourceUri <> "—" then 
                    row[config.CommitterNameColumn] 
                else
                    ""

            let doNotPublish= if doNotPublishDefined then row[config.DoNotPublishColumn] else ""
            createWorkMetadata
                row[config.AuthorNameColumn] 
                row[config.AdvisorColumn] 
                row[config.TitleColumn] 
                sourceUri
                commiterName
                doNotPublish

        let rows = sheet.ReadByHeaders(columns)
        rows
        |> Seq.choose (
            fun row ->
                if allowedResults.Contains row[config.ResultColumn] then
                    Some <| createMetadata row 
                else
                    None
            )

    interface IMetadataSource with

        member _.GetWorksMetadataAsync () =
            let dataConfig = Json.deserialize<DataConfig>(File.ReadAllText appConfig.MetadataConfigFile)
            let specificConfig = Json.deserialize<YandexSheetConfig>(File.ReadAllText appConfig.MetadataConfigFile)

            task {
                let service = YandexService.FromClientSecretsFile()

                let spreadsheetPath = specificConfig.YandexSheetFolderUrl.Remove(0, "https://disk.yandex.ru/client/disk/".Length)
                let unencodedSpreadsheetPath = Uri.UnescapeDataString(spreadsheetPath)
                let unencodedSpreadsheetPath = unencodedSpreadsheetPath + "/" + specificConfig.YandexSheetFileName + ".xlsx"

                let! spreadsheet = service.GetSpreadsheetAsync(unencodedSpreadsheetPath)
                let sheet = spreadsheet.Sheet dataConfig.SheetName

                let data = readSheet dataConfig sheet
                return
                    data
                    |> addNamesIfNeeded
                    |> Seq.toList
            }
