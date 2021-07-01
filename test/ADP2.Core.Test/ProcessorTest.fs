namespace ADP2.Core.Test

open NUnit.Framework
open ADP2.Core
open FsUnitTyped
open System.IO

type ProcessorTest() =

    let config = 
        { ApplicationName = "ADP2"
          CredentialsFile = "../../../../../../credentials.json" 
          SpreadsheetId = "1b1fhGFInVDNXAb_Ok14Nl03V-DviKe-GrE2Geuwsw9o" }

    [<Test>]
    member _.MetainformationShouldBeFilteredCorrectly () =
        if File.Exists config.CredentialsFile then
            let dataSource = SecondCourseMetadataSource(config) :> IWorkMetadataSource
            let works = dataSource.GetWorksMetadata()
            let knowledgeBase = KnowledgeBase ()
            Processor.mergeWorks knowledgeBase works
            knowledgeBase.WorksWithNoMetainformation |> shouldBeEmpty
            knowledgeBase.WorksWithNoFiles |> shouldHaveLength 15
        else
            Assert.Ignore("No credentials for Google Sheets found.")
