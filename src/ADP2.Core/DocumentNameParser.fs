namespace ADP2.Core

open System.Text.RegularExpressions

/// Utility that helps to process file name and classify a file according to a fixed name format traditionally used
/// for qualification works of SE chair.
module DocumentNameParser =

    /// Helper class that does the opposite of Maybe monad --- runs bound functions until one of them returns Some.
    /// Totally not a monad, but useful nonetheless.
    type private MatcherBuilder() =
        member x.Bind(v: Match, f) = if v.Success then Some v else f None
        member x.Return(v: Match) = if v.Success then Some v else None

    let private unmaybe = MatcherBuilder()

    /// Regexp for file naming scheme in this format:
    /// <transliterated surname of a student>-<document kind>.<extension>
    /// For example, "Ololoev-report.pdf"
    let private generalPattern =
        @"((?<ShortName>[a-z.]+)-)+(?<Kind>(slides)|(presentation)|(report)|(consultant-review)|(advisor-review)|(reviewer-review))"

    /// Regexp for file naming scheme in new russian format:
    /// <Actual russian surname of a student><.optional name>-<semester>-<document kind>.<extension>
    /// For example, "Ололоев.Йцукен-4-семест-отчёт.pdf"
    let private newGeneralPattern =
        @"((?<ShortName>[а-яё.]+)-)+(\d-семестр)-(?<Kind>(презентация)|(отчёт)|(отчет)|(отзыв-консультанта)|(отзыв))"

    /// Pattern for matching advisor and consultant reviews in one file. Separate from general pattern to avoid
    /// regex greediness issues.
    let private highPriorityPattern =
        @"((?<ShortName>[a-z.]+)-)+(?<Kind>(advisor-consultant-review))"

    /// Pattern for matching old advisor review file format named simply as "review". Separate from general pattern
    /// to avoid regex greediness issues.
    let private lowPriorityPattern = @"((?<ShortName>[a-z.]+)-)+(?<Kind>(review))"

    /// Regex corresponding to general pattern.
    let private generalRegex = Regex(generalPattern, RegexOptions.IgnoreCase)

    /// Regex corresponding to new (russian) general pattern.
    let private newGeneralRegex = Regex(newGeneralPattern, RegexOptions.IgnoreCase)

    /// Regex corresponding to high priority pattern.
    let private highPriorityRegex = Regex(highPriorityPattern, RegexOptions.IgnoreCase)

    /// Regex corresponding to low priority pattern.
    let private lowPriorityRegex = Regex(lowPriorityPattern, RegexOptions.IgnoreCase)

    /// Classifies files to document kinds.
    let private toDocumentKind =
        function
        | "report"
        | "отчет"
        | "отчёт" -> Text
        | "slides"
        | "presentation"
        | "презентация"
        | "слайды" -> Slides
        | "advisor-consultant-review"
        | "отзыв-научного-руководителя-и-консультанта" -> AdvisorConsultantReview
        | "review"
        | "advisor-review"
        | "отзыв" -> AdvisorReview
        | "consultant-review"
        | "отзыв-консультанта" -> ConsultantReview
        | "reviewer-review"
        | "рецензия" -> ReviewerReview
        | _ -> failwith "Incorrect document kind, regex seems to be invalid"

    /// Parses given file name and produces corresponding Document entry if parsing was successful.
    /// Returns file name if it failed to match.
    let parse fileName : Choice<Document, string> =
        let regexMatch =
            let foundMatch =
                unmaybe {
                    let! _ = highPriorityRegex.Match(fileName)
                    let! _ = generalRegex.Match(fileName)
                    let! _ = newGeneralRegex.Match(fileName)
                    return lowPriorityRegex.Match(fileName)
                }

            foundMatch

        let splittedName = fileName.Split("\\")
        let fileNamePart = splittedName |> Seq.last

        match regexMatch with
        | Some regexMatch ->
            let authors =
                regexMatch.Groups.["ShortName"].Captures
                |> Seq.map (fun c -> c.Value)
                |> Seq.toList

            let kind = regexMatch.Groups.["Kind"].Value
            let fileNamePart = (System.IO.FileInfo fileNamePart).Name

            Choice1Of2
                { FileName = fileNamePart
                  FileNameWithRelativePath = fileName
                  Authors = authors
                  Kind = toDocumentKind kind }
        | None -> Choice2Of2 fileName
