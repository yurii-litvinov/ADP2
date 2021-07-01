namespace ADP2.Core

open DocUtils
open MetadataSourceUtils

/// Implementation of metadata source specifically for 3rd course Google tables.
/// For example, https://docs.google.com/spreadsheets/d/1YSEU5KglOTxQGJwFNjNtRSo-VI9wbIvHuPd7_ncI33Y
type ThirdCourseMetadataSource(config: GoogleSheetsMetadataSourceConfig) =

    let readSheet (sheet: Sheet) =
        let rows = sheet.ReadSheet("A", "F", 2)
        rows
        |> Seq.map Seq.toList
        |> Seq.map (
            function
            | [ author; _; _; advisor; _; title ] -> 
                createWorkMetadata author advisor title
            | _ -> failwith "Invalid table"
            )

    interface IWorkMetadataSource with
        member _.GetWorksMetadata () =
            let service = new GoogleSheetService(config.CredentialsFile, config.ApplicationName)
            let sheet = service.Sheet(config.SpreadsheetId, "Лист1")
            sheet 
            |> readSheet
            |> addNamesIfNeeded
            |> Seq.toList
