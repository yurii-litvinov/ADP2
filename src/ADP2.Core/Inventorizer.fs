namespace ADP2.Core

open System.IO

/// Searches .pdf documents by given path and tries to parse them into Work document types.
type Inventorizer() =

    /// Searches .pdf documents by given path and tries to parse them into Work document types, filling Knowledge base. 
    /// Reports any unknown files.
    static member Inventorize(path: string) =
        let knowledgeBase = KnowledgeBase()
        let files = Directory.GetFiles(path, "*.pdf")
        files 
        |> Seq.map DocumentNameParser.parse
        |> Seq.iter (function 
                     | Choice1Of2 document -> knowledgeBase.Add document
                     | Choice2Of2 fileName -> knowledgeBase.AddUnknownFile fileName
                    )

        knowledgeBase
