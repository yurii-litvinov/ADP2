namespace ADP2.Core

/// Joins data from metadata source and files into one knowledge base, filters it and checks correctness.
module Processor =
    let private processMetadata (knowledgeBase: KnowledgeBase) (metadata: Work) =
        if not <| knowledgeBase.Merge metadata then
            knowledgeBase.AddAsWorkWithNoFiles metadata

    /// Adds metadata to existing works in knowledge base. Reports metadata if no matching files were found.
    let mergeWorks (knowledgeBase: KnowledgeBase) (metadata: Work seq) =
        metadata |> Seq.iter (processMetadata knowledgeBase)
