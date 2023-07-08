namespace ADP2.Core

open FSharp.Json
open System.IO

/// Serializes knowledge base into JSON file.
module Serializer =
    /// Metainformation about work.
    type ThesisInfo =
        { type_id: int
          course_id: int 
          name_ru: string 
          author: string
          source_uri: string
          supervisor: string
          publish_year: int 
          secret_key: string }

    /// Information about work, including related files.
    type WorkInfo =
        { thesis_text: string option
          reviewer_review: string option
          presentation: string option
          supervisor_review: string option
          thesis_info: ThesisInfo
        }

    /// Type of qualification work as accepted by SE Chair site API.
    type WorkType =
    | BachelorReport = 2
    | BachelorThesis = 3
    | MasterThesis = 4
    | AutumnPractice2ndYear = 5
    | SpringPractice2ndYear = 6
    | AutumnPractice3rdYear = 7
    | SpringPractice3rdYear = 8

    /// Educational programme as accepted by SE Chair site API.
    type Programme =
    | SoftwareAndAdministrationOfInformationSystemsBachelors = 1
    | SoftwareEngineeringBachelors = 2
    | SoftwareAndAdministrationOfInformationSystemsMasters = 3
    | FundamentalInformaticsAndInformationTechnologies = 4
    | InformationTechnologies = 5
    | Group344 = 6
    | SoftwareEngineeringMasters = 7

    /// Configuration file format.
    type Config = 
        { [<JsonField(EnumValue = EnumMode.Value)>]
          WorkType: WorkType
          [<JsonField(EnumValue = EnumMode.Value)>]
          Programme: Programme
          Year: int 
          SecretKey: string }

    let private config = Json.deserialize<Config>(File.ReadAllText "_config.json")

    let private docToString (d: Document option) =
        match d with 
        | None -> None
        | Some d -> Some d.FileNameWithRelativePath

    let private serializeWork (work: Work) =
        { thesis_text = docToString work.Text
          reviewer_review = docToString work.ReviewerReview
          presentation = docToString work.Slides
          supervisor_review = docToString work.AdvisorReview
          thesis_info =
            { type_id = int config.WorkType
              course_id = int config.Programme
              name_ru = work.Title
              author = work.AuthorName
              source_uri = work.SourceUri
              supervisor = work.AdvisorSurname
              publish_year = config.Year 
              secret_key = config.SecretKey } }

    /// Serializes knowledge base into _out.json in a format understood by SE Chair site API.
    let serialize (knowledgeBase: KnowledgeBase) = 
        let works = knowledgeBase.AllWorks
        let jsonConfig = JsonConfig.create(serializeNone = SerializeNone.Omit)
        
        let json = 
            works 
            |> Seq.filter (fun w -> not w.DoNotPublish) 
            |> Seq.map serializeWork 
            |> Seq.toList 
            |> (Json.serializeEx jsonConfig)

        File.WriteAllText("_out.json", json)
