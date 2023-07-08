namespace ADP2.Core.Test

open NUnit.Framework
open ADP2.Core
open FsUnit
open System.IO

type FourthCourseMetadataSourceTest() =

    let config = 
        { GoogleApplicationName = "ADP2"
          GoogleCredentialsFile = "../../../../../../credentials.json" 
          MetadataConfigFile = "Data/fourthCourseConfig.json" }

    [<Test>]
    member _.MetadataForTheFirstStudentShouldBeReceivedCorrectly () =
        if File.Exists config.GoogleCredentialsFile then
            let dataSource = GoogleSheetsMetadataSource(config) :> IMetadataSource
            let works = dataSource.GetWorksMetadataAsync() |> Async.AwaitTask |> Async.RunSynchronously
            let firstStudent = Seq.head works
            firstStudent.AdvisorSurname |> should equal "Терехов"
            firstStudent.AuthorName |> should equal "Асеева Серафима Олеговна"
            firstStudent.ShortName |> should equal "Aseeva"
            firstStudent.Title |> should equal "Разработка отладчика для IDE RuC"
            firstStudent.SourceUri |> should equal "https://github.com/manelyset/RuC,  https://github.com/manelyset/RuC-VM"
            firstStudent.CommitterName |> should equal "manelyset"
        else
            Assert.Ignore("No credentials for Google Sheets found.")
