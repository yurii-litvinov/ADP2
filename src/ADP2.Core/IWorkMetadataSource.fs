namespace ADP2.Core

/// Interface of a general metadata provider. It must supply work name, author and so on, leaving 
/// document fields empty.
type IWorkMetadataSource =
    /// Provides a list with works metadata.
    abstract member GetWorksMetadata: unit -> Work list
