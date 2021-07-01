namespace ADP2.Core

open FSharp.Json
open System.IO
open System

/// Joins data from metadata source and files into one knowledge base, filters it and checks correctness.
module Processor =
    let private advisorsPath = AppDomain.CurrentDomain.BaseDirectory + "/Data/advisors.json"
    let private advisors = 
        Json.deserialize<list<string>>(File.ReadAllText advisorsPath)
        |> Seq.map (fun s -> let splitted = s.Split(' ') in (splitted.[0], splitted.[1]))
        |> Set.ofSeq

    let private advisorFromChair (work: Work) =
        advisors.Contains (work.AdvisorSurname, work.AdvisorName)
    
    let private processMetadata (knowledgeBase: KnowledgeBase) (metadata: Work) =
        if not <| knowledgeBase.Merge metadata then
            knowledgeBase.AddAsWorkWithNoFiles metadata

    /// Adds metadata to existing works in knowledge base. Reports metadata if no matching files were found.
    let mergeWorks (knowledgeBase: KnowledgeBase) (metadata: Work seq) =
        metadata
        |> Seq.filter advisorFromChair
        |> Seq.iter (processMetadata knowledgeBase)
