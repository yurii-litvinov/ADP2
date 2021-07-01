namespace ADP2.Core

/// Ties it all together: module that describes processing frm getting data to generating metadata for site.
module Workflow =
    /// Gets metadata using given metadata provider, inventorizes files in working directory and prints JSON
    /// with all data needed to upload works.
    let generateWorksInfo (metadataSource: IWorkMetadataSource) =
        let metadata = metadataSource.GetWorksMetadata ()

        let knowledgeBase = Inventorizer.Inventorize(".")
        Processor.mergeWorks knowledgeBase metadata

        Serializer.serialize knowledgeBase

        knowledgeBase
