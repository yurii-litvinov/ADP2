namespace ADP2.Core

/// Repository of all information known about works.
type KnowledgeBase() = 
    let mutable works = Map.empty

    /// A list of files that are failed to match any of known types of documents, used for error reporting.
    let mutable unknownFiles = []

    /// A list of works mentioned in metadata but for whose no files are present.
    let mutable worksWithNoFiles = []

    /// Adds given document to a repository creating new Work record if needed, or adds a new document 
    /// to an existing one.
    member _.Add (document : Document) =
        let authors = document.Authors
        for author in authors do
            let work = 
                if works.ContainsKey author then
                    works.[author]
                else
                    Work(author)
            work.Add document
            works <- works.Add (author, work)
        ()

    /// Adds a file not recognized by document name parser for error reporting.
    member _.AddUnknownFile (fileName: string) =
        unknownFiles <- fileName :: unknownFiles

    /// Adds a work mentioned in metadata but for whose no files are present.
    member _.AddAsWorkWithNoFiles (work: Work) =
        worksWithNoFiles <- work :: worksWithNoFiles

    /// Returns a collection of files not recognized by document name parser, for error reporting.
    member _.UnknownFiles =
        unknownFiles |> List.toSeq

    /// Returns a collection of works for that no files were found, for error reporting.
    member _.WorksWithNoFiles =
        worksWithNoFiles |> List.toSeq

    /// Returns a collection of works for that no metainformation was found, for error reporting.
    member _.WorksWithNoMetainformation =
        works |> Seq.filter (fun w -> w.Value.Title = "") |> Seq.map (fun w -> w.Value)

    /// Returns true if there are some problems with knowledge base.
    member this.HasErrors =
        not (Seq.isEmpty this.UnknownFiles && Seq.isEmpty this.WorksWithNoFiles && Seq.isEmpty this.WorksWithNoMetainformation)

    /// Returns all existing Works records.
    member _.AllWorks = (works |> Map.toSeq |> Seq.map snd) |> Seq.sortBy (fun w -> w.AuthorName)

    /// Checks if given author id is present in a database.
    member _.Contains shortName =
        works.ContainsKey shortName

    /// Merges given work with work with same author id if it is present in a knowledge base and returns true, else returns false.
    member _.Merge (work: Work) =
        if works.ContainsKey work.ShortName then
            works.[work.ShortName].Merge work
            true
        else
            false
