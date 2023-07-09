namespace ADP2.Core

/// Helper functions to simplify working with documents and metainformation.
module DataModelUtils =

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
            |> List.map (fun (a, b) ->
                (a.ToString().ToUpper().[0], (if b = "" then b else b.ToUpper().[0].ToString() + b.[1..])))

        let transliterationMap = transliterationList @ transliterationCapsList |> Map.ofList

        name.ToCharArray()
        |> Seq.map (fun ch ->
            if transliterationMap.ContainsKey ch then
                transliterationMap.[ch]
            else
                ch.ToString())
        |> Seq.reduce (+)
