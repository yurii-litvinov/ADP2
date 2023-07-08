namespace ADP2.Core

namespace ADP2.Core

open System.Threading.Tasks

/// Abstraction of work metadata source. Allows to get information about qualification works,
/// such as title, author name, supervisor and so on.
type IMetadataSource =

    /// Asynchronously gets metadata about qualification works, such as title, author name, supervisor and so on.
    abstract GetWorksMetadataAsync: unit -> Task<Work list>
