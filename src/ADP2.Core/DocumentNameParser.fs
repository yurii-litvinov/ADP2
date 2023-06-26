namespace ADP2.Core

open System.Text.RegularExpressions

/// Utility that helps to process file name and classify a file according to a fixed name format traditionally used
/// for qualification works of SE chair.
module DocumentNameParser =
    /// Regexp for file naming scheme in this format:
    /// <transliterated surname of a student>-<document kind>.<extension>
    /// For example, "Ololoev-report.pdf"
    let private generalPattern = @"((?<ShortName>[a-z.]+)-)+(?<Kind>(slides)|(presentation)|(report)|(consultant-review)|(advisor-review)|(reviewer-review))"

    /// Pattern for matching advisor and consultant reviews in one file. Separate from general pattern to avoid 
    /// regex greediness issues.
    let private highPriorityPattern = @"((?<ShortName>[a-z.]+)-)+(?<Kind>(advisor-consultant-review))"

    /// Pattern for matching old advisor review file format named simply as "review". Separate from general pattern 
    /// to avoid regex greediness issues.
    let private lowPriorityPattern = @"((?<ShortName>[a-z.]+)-)+(?<Kind>(review))"

    /// Regex corresponding to general pattern.
    let private generalRegex = Regex(generalPattern, RegexOptions.IgnoreCase)

    /// Regex corresponding to high priority pattern.
    let private highPriorityRegex = Regex(highPriorityPattern, RegexOptions.IgnoreCase)

    /// Regex corresponding to low priority pattern.
    let private lowPriorityRegex = Regex(lowPriorityPattern, RegexOptions.IgnoreCase)

    /// Classifies files to document kinds.
    let private toDocumentKind = function
        | "report" -> Text
        | "slides" | "presentation" -> Slides
        | "advisor-consultant-review" -> AdvisorConsultantReview
        | "review" | "advisor-review" -> AdvisorReview
        | "consultant-review" -> ConsultantReview
        | "reviewer-review" -> ReviewerReview
        | _ -> failwith "Incorrect document kind, regex seems to be invalid"

    /// Parses given file name and produces corresponding Document entry if parsing was successful.
    /// Returns file name if it failed to match.
    let parse fileName: Choice<Document, string> =
        let regexMatch =
            let priorityMatch = highPriorityRegex.Match(fileName)
            if priorityMatch.Success then
                Some priorityMatch
            else
                let generalMatch = generalRegex.Match(fileName)
                if generalMatch.Success then
                    Some generalMatch
                else
                    let lowPriorityMatch = lowPriorityRegex.Match(fileName)
                    if lowPriorityMatch.Success then
                        Some lowPriorityMatch
                    else
                        None

        let splittedName = fileName.Split("\\")
        let fileNamePart = splittedName |> Seq.last

        match regexMatch with
            | Some regexMatch ->
                let authors = regexMatch.Groups.["ShortName"].Captures |> Seq.map (fun c -> c.Value) |> Seq.toList
                let kind = regexMatch.Groups.["Kind"].Value
                let fileNamePart = (System.IO.FileInfo fileNamePart).Name
                Choice1Of2 {FileName = fileNamePart; FileNameWithRelativePath = fileName; Authors = authors; Kind = toDocumentKind kind}
            | None -> Choice2Of2 fileName

