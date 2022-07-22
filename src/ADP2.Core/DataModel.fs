namespace ADP2.Core

/// Kind of a document related to a work.
/// For second course and third course works there is text, slides and advisor review, optionally a consultant review. 
/// Advisor and consultant review can be submitted as a single document.
/// Qualification works also have separate reviewer reviews.
type DocumentKind =
    | Text
    | Slides
    | AdvisorReview
    | ConsultantReview
    | AdvisorConsultantReview
    | ReviewerReview

/// Document related to qualification work, stored as a file with special name format:
/// <transliterated surname of a student>-<document kind>.<extension>
/// For example, "Ololoev-report.pdf"
/// One document can have several authors, for example, slides for team project can be named 
/// as "Ivanov-Petrov-slides.pdf".
/// If there are several students with the same surname, name is added to a surname in the file name, 
/// for example, "Ivanov.Ivan-slides.pdf"
type Document = { 
    FileName: string
    Authors: string list
    Kind: DocumentKind 
}

/// <summary>
/// All collected information about one work. Includes list of related documents and some metainformation 
/// to be used by generator.
/// </summary>
/// <param name="shortName">
///     Transliterated student surname or surname and name, used as an Id of a work throughout the system.
/// </param>
type Work(shortName: string) =
    
    /// Transliterated student surname or surname and name, used as an Id of a work throughout the system.
    member val ShortName = shortName with get, set
    
    /// Title of the work.
    member val Title = "" with get, set

    /// Full name of the author (<Surname> <First name> <Middle name>).
    member val AuthorName = "" with get, set

    /// URL of a source code, if present.
    member val SourceUri = "" with get, set

    /// Commiter name for a source code, if present.
    member val CommitterName = "" with get, set

    /// Name of the scientific advisor. Used to work with ambuguous advisors.
    member val AdvisorName = "" with get, set

    /// Surname of the scientific advisor.
    member val AdvisorSurname = "" with get, set

    /// Text of a work, if present.
    member val Text: Document option = None with get, set

    /// Slides for this work, if present.
    member val Slides: Document option = None with get, set

    /// Scientific advisor review, if present.
    member val AdvisorReview: Document option = None with get, set

    /// Consultant review, if present.
    member val ConsultantReview: Document option = None with get, set

    /// Reviewer review, if present.
    member val ReviewerReview: Document option = None with get, set

    /// Adds a new document to the work entry.
    member this.Add (document: Document) =
        match document.Kind with
        | Text -> this.Text <- Some document
        | Slides -> this.Slides <- Some document
        | AdvisorReview -> this.AdvisorReview <- Some document
        | ConsultantReview -> this.ConsultantReview <- Some document
        | AdvisorConsultantReview -> 
            this.AdvisorReview <- Some document
            this.ConsultantReview <- Some document
        | ReviewerReview -> this.ReviewerReview <- Some document

    /// Merges metainformation from other work to this one.
    member this.Merge (other: Work) =
        assert (other.ShortName = this.ShortName)
        if other.Title <> "" && this.Title = "" then 
            this.Title <- other.Title
        if other.AuthorName <> "" && this.AuthorName = "" then
            this.AuthorName <- other.AuthorName
        if other.AdvisorSurname <> "" && this.AdvisorSurname = "" then
            this.AdvisorSurname <- other.AdvisorSurname
        if other.AdvisorName <> "" && this.AdvisorName = "" then
            this.AdvisorName <- other.AdvisorName
        if other.ConsultantReview.IsSome && this.ConsultantReview.IsNone then
            this.ConsultantReview <- other.ConsultantReview
        if other.AdvisorReview.IsSome && this.AdvisorReview.IsNone then
            this.AdvisorReview <- other.AdvisorReview
        if other.ReviewerReview.IsSome && this.ReviewerReview.IsNone then
            this.ReviewerReview <- other.ReviewerReview
        if other.Slides.IsSome && this.Slides.IsNone then
            this.Slides <- other.Slides
        if other.Text.IsSome && this.Text.IsNone then
            this.Text <- other.Text

    override this.ToString() =
        $"{this.ShortName}"
