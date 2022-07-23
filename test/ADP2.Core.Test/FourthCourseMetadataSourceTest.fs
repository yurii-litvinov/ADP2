namespace ADP2.Core.Test

open NUnit.Framework
open ADP2.Core
open FsUnit
open System.IO

type FourthCourseMetadataSourceTest() =

    let config = 
        { ApplicationName = "ADP2"
          CredentialsFile = "../../../../../../credentials.json" 
          MetadataConfigFile = "Data/fourthCourseConfig.json" }

    [<Test>]
    member _.MetadataForTheFirstStudentShouldBeReceivedCorrectly () =
        if File.Exists config.CredentialsFile then
            let dataSource = MetadataSource(config)
            let works = dataSource.GetWorksMetadata()
            let firstStudent = Seq.head works
            firstStudent.AdvisorSurname |> should equal "Терехов"
            firstStudent.AuthorName |> should equal "Асеева Серафима Олеговна"
            firstStudent.ShortName |> should equal "Aseeva"
            firstStudent.Title |> should equal "Разработка отладчика для IDE RuC"
            firstStudent.SourceUri |> should equal "https://github.com/manelyset/RuC,  https://github.com/manelyset/RuC-VM"
            firstStudent.CommitterName |> should equal "manelyset"
        else
            Assert.Ignore("No credentials for Google Sheets found.")
