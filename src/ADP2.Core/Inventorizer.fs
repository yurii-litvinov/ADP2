namespace ADP2.Core

open System.IO

/// Searches .pdf documents by given path and tries to parse them into Work document types.
type Inventorizer() =

    /// Lists files in "plain folder" format, where all documents are in one folder
    static let getFilesFromPlainFolder(path: string) =
        Directory.GetFiles(path, "*.pdf")

    /// Lists files in grouped format, where folder contains a subfolder for each student with his documents.
    static let getFilesFromFoldersByStudent(path: string) =
        let directories = Directory.GetDirectories(path)
        directories
        |> Seq.map getFilesFromPlainFolder
        |> Seq.concat

    /// Determines folder structure and lists documents.
    static let getFiles(path: string) =
        let directories = Directory.GetDirectories(path)
        if directories.Length > 1 then
            getFilesFromFoldersByStudent path
        else
            getFilesFromPlainFolder path

    /// Searches .pdf documents by given path and tries to parse them into Work document types, filling Knowledge base. 
    /// Reports any unknown files.
    static member Inventorize(path: string) =
        let knowledgeBase = KnowledgeBase()
        let files = getFiles path
        files 
        |> Seq.map DocumentNameParser.parse
        |> Seq.iter (function 
                     | Choice1Of2 document -> knowledgeBase.Add document
                     | Choice2Of2 fileName -> knowledgeBase.AddUnknownFile fileName
                    )

        knowledgeBase
