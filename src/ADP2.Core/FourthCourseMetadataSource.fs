namespace ADP2.Core

open DocUtils
open MetadataSourceUtils

/// Implementation of metadata source specifically for bachelor's qualification works Google tables.
/// For example, https://docs.google.com/spreadsheets/d/1jZftsNQ1D1KQvmkA_WkE_cdUNYXyVHn68EHWzhyNq_k
type FourthCourseMetadataSource(config: GoogleSheetsMetadataSourceConfig) =

    let readSheet (sheet: Sheet) =
        let rows = sheet.ReadSheet("A", "D", 2)
        rows
        |> Seq.map Seq.toList
        |> Seq.map (
            function
            | [ author; _; title; advisor; ] -> 
                createWorkMetadata author advisor title
            | _ -> failwith "Invalid table"
            )

    interface IWorkMetadataSource with
        member _.GetWorksMetadata () =
            let service = new GoogleSheetService(config.CredentialsFile, config.ApplicationName)
            let sheet = service.Sheet(config.SpreadsheetId, "SE")
            sheet 
            |> readSheet
            |> addNamesIfNeeded
            |> Seq.toList
