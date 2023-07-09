namespace ADP2.Core.Test

open NUnit.Framework
open ADP2.Core
open FsUnitTyped
open System.IO

type ProcessorTest() =

    let config =
        { GoogleApplicationName = "ADP2"
          GoogleCredentialsFile = "../../../../../../credentials.json"
          MetadataConfigFile = "Data/secondCourseConfig.json" }

    [<Test>]
    member _.MetainformationShouldBeFilteredCorrectly() =
        if File.Exists config.GoogleCredentialsFile then
            let dataSource = GoogleSheetsMetadataSource(config) :> IMetadataSource

            let works =
                dataSource.GetWorksMetadataAsync() |> Async.AwaitTask |> Async.RunSynchronously

            let knowledgeBase = KnowledgeBase()
            Processor.mergeWorks knowledgeBase works
            knowledgeBase.WorksWithNoMetainformation |> shouldBeEmpty
            knowledgeBase.WorksWithNoFiles |> shouldHaveLength 17
        else
            Assert.Ignore("No credentials for Google Sheets found.")
