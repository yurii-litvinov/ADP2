namespace ADP2.Core

open DocUtils

/// Configuration for reading data from Google Sheets.
type SecondCourseMetadataSourceConfig =
    { ApplicationName: string 
      CredentialsFile: string
      SpreadsheetId: string }

/// Implementation of metadata source specifically for 2nd course Google tables.
/// For example, https://docs.google.com/spreadsheets/d/1b1fhGFInVDNXAb_Ok14Nl03V-DviKe-GrE2Geuwsw9o
type SecondCourseMetadataSource(config: SecondCourseMetadataSourceConfig) =
    let getSurname (name: string) =
        name.Split([|' '|]) |> Seq.head

    let transliterate (name: string) =
        let transliterationList = 
            [ 'а', "a"
              'б', "b"
              'в', "v"
              'г', "g"
              'д', "d"
              'е', "e"
              'ё', "e"
              'ж', "zh"
              'з', "z"
              'и', "i"
              'й', "i"
              'к', "k"
              'л', "l"
              'м', "m"
              'н', "n"
              'о', "o"
              'п', "p"
              'р', "r"
              'с', "s"
              'т', "t"
              'у', "u"
              'ф', "f"
              'х', "kh"
              'ц', "ts"
              'ч', "ch"
              'ш', "sh"
              'щ', "shch"
              'ы', "y"
              'ь', ""
              'ъ', "ie"
              'э', "e"
              'ю', "iu"
              'я', "ia" ]
        let transliterationCapsList = 
            transliterationList 
            |> List.map 
                (fun (a, b) -> (a.ToString().ToUpper().[0], if b = "" then b else b.ToUpper().[0].ToString() + b.[1..]))
        let transliterationMap = transliterationList @ transliterationCapsList |> Map.ofList
        
        name.ToCharArray() 
        |> Seq.map(fun ch -> if transliterationMap.ContainsKey ch then transliterationMap.[ch] else ch.ToString()) 
        |> Seq.reduce (+)

    let parseAdvisor (raw: string) =
        let surname =
            if raw.Contains('.') then
                raw.Split([|' '; '.'|]) |> Seq.last
            else
                getSurname raw
        let name = 
            if raw.Contains('.') then
                raw.[0..raw.Length - surname.Length - 1]
            else if raw = "" then
                ""
            else
                raw.Split([|' '|]) |> Seq.skip 1 |> Seq.head
        (name, surname)

    let addNamesIfNeeded (works: Work seq) =
        let surnameCounts = works |> Seq.countBy (fun w -> w.ShortName) |> Map.ofSeq
        works 
        |> Seq.map (fun w -> 
            if surnameCounts.[w.ShortName] > 1 then 
                w.ShortName <- w.ShortName + "." + (transliterate <| w.AuthorName.Split(' ').[1]) 
            w)

    let readSheet (sheet: Sheet) =
        let rows = sheet.ReadSheet("A", "D", 2)
        rows
        |> Seq.map Seq.toList
        |> Seq.map (
            function
            | [ author; advisor; _; title ] -> 
                let work = Work(author |> getSurname |> transliterate)
                let advisor = parseAdvisor advisor
                work.AdvisorName <- fst advisor
                work.AdvisorSurname <- snd advisor
                work.AuthorName <- author
                work.Title <- title
                work
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
