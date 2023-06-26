namespace ADP2.Core

/// Configuration for reading data from Google Sheets.
type GlobalConfig =
    { ApplicationName: string
      CredentialsFile: string
      MetadataConfigFile: string }

/// A set of useful functions to work with works metadata.
module MetadataSourceUtils =

    /// Marks in a "Зачёт" column that meant that a work was successfully defended.
    let allowedResults = Set.ofList [ "A"; "B"; "C"; "D"; "E"; "да" ]

    /// Gets surname from full name in "Surname Name MiddleName" format.
    let getSurname (name: string) =
        name.Split([|' '|]) |> Seq.head

    /// Uses standard transliteration rules to transliterate given string.
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

    /// Returns advisor name and surname from full name in "Surname Name MiddleName" format or
    /// "Dr. I.I. Surname" format.
    let parseAdvisor (raw: string) =
        let surname =
            if raw.Contains('.') then
                raw.Split([|' '; '.'|]) |> Seq.last
            else
                getSurname raw
        let name = 
            if raw.Contains('.') then
                raw.[0..raw.Length - surname.Length - 1]
            else if raw.Contains(' ') |> not then
                ""
            else
                raw.Split([|' '|]) |> Seq.skip 1 |> Seq.head
        (name, surname)

    /// Searches for non-unique surnames in a Work sequence and adds first name after dot for them.
    let addNamesIfNeeded (works: Work seq) =
        let surnameCounts = works |> Seq.countBy (fun w -> w.ShortName) |> Map.ofSeq
        works 
        |> Seq.map (fun w -> 
            if surnameCounts.[w.ShortName] > 1 then 
                w.ShortName <- w.ShortName + "." + (transliterate <| w.AuthorName.Split(' ').[1]) 
            w)

    /// Constructs new Work record.
    let createWorkMetadata author advisor title (sourceUrl: string) committerName =
        let cleanup (str: string) =
            str.Replace("\r", "").Replace("\n", " ")

        let work = Work(author |> getSurname |> transliterate)
        let advisor = parseAdvisor advisor
        work.AdvisorName <- fst advisor
        work.AdvisorSurname <- snd advisor
        work.AuthorName <- author
        work.Title <- cleanup title
        work.SourceUri <- if sourceUrl.StartsWith "http" then sourceUrl else ""
        work.CommitterName <- if work.SourceUri = "" then "" else committerName
        work
