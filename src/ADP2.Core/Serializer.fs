namespace ADP2.Core

open FSharp.Json
open System.IO

/// Serializes knowledge base into JSON file.
module Serializer =
    /// Metainformation about work.
    type private ThesisInfo =
        { type_id: int
          course_id: int 
          name_ru: string 
          author: string
          supervisor: string
          publish_year: int 
          secret_key: string }

    /// Information about work, including related files.
    type private WorkInfo =
        { thesis_text: string option
          reviewer_review: string option
          presentation: string option
          supervisor_review: string option
          thesis_info: ThesisInfo
        }

    /// Type of qualification work as accepted by SE Chair site API.
    type private WorkType =
    | Practice = 2
    | BachelorsDiploma = 3
    | MastersDiploma = 4

    /// Configuration file format.
    type private Config = 
        { [<JsonField(EnumValue = EnumMode.Value)>]
          WorkType: WorkType 
          Course: int
          Year: int }

    let private config = Json.deserialize<Config>(File.ReadAllText "_config.json")

    let private docToString (d: Document option) =
        match d with 
        | None -> None
        | Some d -> Some d.FileName

    let private serializeWork (work: Work) =
        { thesis_text = docToString work.Text
          reviewer_review = docToString work.ReviewerReview
          presentation = docToString work.Slides
          supervisor_review = docToString work.AdvisorReview
          thesis_info =
            { type_id = int config.WorkType
              course_id = config.Course
              name_ru = work.Title
              author = work.AuthorName
              supervisor = work.AdvisorSurname
              publish_year = config.Year 
              secret_key = "" } }

    /// Serializes knowledge base into _out.json in a format understood by SE Chair site API.
    let serialize (knowledgeBase: KnowledgeBase) = 
        let works = knowledgeBase.AllWorks
        let jsonConfig = JsonConfig.create(serializeNone = SerializeNone.Omit)
        let json = works |> Seq.map serializeWork |> Seq.toList |> (Json.serializeEx jsonConfig)
        File.WriteAllText("_out.json", json)
