namespace ADP2.Core

open DocUtils
open MetadataSourceUtils

/// Implementation of metadata source specifically for 2nd course Google tables.
/// For example, https://docs.google.com/spreadsheets/d/1b1fhGFInVDNXAb_Ok14Nl03V-DviKe-GrE2Geuwsw9o
type SecondCourseMetadataSource(config: GoogleSheetsMetadataSourceConfig) =
    let readSheet (sheet: Sheet) =
        let rows = sheet.ReadSheet("A", "D", 2)
        rows
        |> Seq.map Seq.toList
        |> Seq.map (
            function
            | [ author; advisor; _; title ] -> 
                createWorkMetadata author advisor title
            | _ -> failwith "Invalid table"
            )

    interface IWorkMetadataSource with
        member _.GetWorksMetadata () =
            let service = new GoogleSheetService(config.CredentialsFile, config.ApplicationName)
            let sheets = service.Spreadsheet(config.SpreadsheetId).Sheets ()
            sheets 
            |> Seq.map (fun sheetId -> service.Sheet(config.SpreadsheetId, sheetId))
            |> Seq.map readSheet
            |> Seq.concat
            |> addNamesIfNeeded
            |> Seq.toList
