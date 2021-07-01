namespace ADP2.Core.Test

open NUnit.Framework
open ADP2.Core
open FsUnitTyped

type InventorizerTest() =

    [<Test>]
    member _.InventorizerShouldGetFileNamesCorrectly () =
        let knowledgeBase = Inventorizer.Inventorize("../../../../../test/Data")
        knowledgeBase.UnknownFiles |> shouldBeEmpty
        knowledgeBase.AllWorks |> shouldHaveLength 3
        knowledgeBase.AllWorks |> Seq.tryFind (fun w -> w.ShortName = "Porsev.Egor") |> shouldNotEqual None
