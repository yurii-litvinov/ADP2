namespace ADP2.Core

open DocUtils.GoogleSheets
open MetadataSourceUtils
open FSharp.Json
open System.IO

/// Configuration for reading data from Google Sheets.
type GoogleSheetsConfig = { GoogleSheetId: string }

/// Implementation of metadata source for qualification works Google tables. Required columns are
/// configured via config.json file.
/// For example, see https://docs.google.com/spreadsheets/d/103E-S9SxzRAPiTt_Yr6428243hzIKzjy1WaLR_W7HDM
type GoogleSheetsMetadataSource(appConfig: ApplicationConfig) =

    let readSheetAsync (config: DataConfig) (sheet: Sheet) =
        let columns = getInterestingColumns config

        let sourceUriDefined, doNotPublishDefined = isDefinedOptionalColumns config

        task {
            let! rows = sheet.ReadByHeadersAsync(columns)

            return
                rows
                |> Seq.choose (fun row ->
                    if allowedResults.Contains row[config.ResultColumn] then

                        let sourceUri = if sourceUriDefined then row[config.SourceUriColumn] else ""

                        let commiterName =
                            if sourceUriDefined && sourceUri <> "" && sourceUri <> "—" then
                                row[config.CommitterNameColumn]
                            else
                                ""

                        let doNotPublish =
                            if doNotPublishDefined then
                                row[config.DoNotPublishColumn]
                            else
                                ""

                        Some (createWorkMetadata
                                row[config.AuthorNameColumn]
                                row[config.AdvisorColumn]
                                row[config.TitleColumn]
                                sourceUri
                                commiterName
                                doNotPublish)
                    else
                        None)
        }

    interface IMetadataSource with

        member _.GetWorksMetadataAsync() =
            let dataConfig =
                Json.deserialize<DataConfig> (File.ReadAllText appConfig.MetadataConfigFile)

            let additionalConfig =
                Json.deserialize<GoogleSheetsConfig> (File.ReadAllText appConfig.MetadataConfigFile)

            task {
                let! service =
                    GoogleSheetService.CreateAsync(appConfig.GoogleCredentialsFile, appConfig.GoogleApplicationName)

                let sheet = service.Sheet(additionalConfig.GoogleSheetId, dataConfig.SheetName)
                let! data = readSheetAsync dataConfig sheet
                return data |> addNamesIfNeeded |> Seq.toList
            }
