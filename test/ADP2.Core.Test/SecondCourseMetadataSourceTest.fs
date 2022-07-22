namespace ADP2.Core.Test

open NUnit.Framework
open ADP2.Core
open FsUnit
open System.IO

type SecondCourseMetadataSourceTest() =

    let config = 
        { ApplicationName = "ADP2"
          CredentialsFile = "../../../../../../credentials.json" 
          MetadataConfigFile = "Data/secondCourseConfig.json" }

    [<Test>]
    member _.MetadataForTheFirstStudentShouldBeReceivedCorrectly () =
        if File.Exists config.CredentialsFile then
            let dataSource = MetadataSource(config)
            let works = dataSource.GetWorksMetadata()
            let firstStudent = Seq.head works
            firstStudent.AdvisorSurname |> should equal "Литвинов"
            firstStudent.AuthorName |> should equal "Бакова Елена Зауровна"
            firstStudent.ShortName |> should equal "Bakova"
            firstStudent.Title |> should equal "Создание категорий уведомлений в HwProj-2"
        else
            Assert.Ignore("No credentials for Google Sheets found.")

    [<Test>]
    member _.MetadataShouldContainMoskalenko() =
        if File.Exists config.CredentialsFile then
            let dataSource = MetadataSource(config)
            let works = dataSource.GetWorksMetadata()
            works |> Seq.tryFind (fun w -> w.ShortName = "Moskalenko") |> should be (ofCase <@Some@>)
        else
            Assert.Ignore("No credentials for Google Sheets found.")

    // Remove this test when Filatova defends her practice.
    [<Test>]
    member _.MetadataShouldNotContainFilatovaYet() =
        if File.Exists config.CredentialsFile then
            let dataSource = MetadataSource(config)
            let works = dataSource.GetWorksMetadata()
            works |> Seq.tryFind (fun w -> w.ShortName = "Filatova") |> should equal None
        else
            Assert.Ignore("No credentials for Google Sheets found.")
