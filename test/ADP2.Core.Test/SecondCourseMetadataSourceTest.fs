namespace ADP2.Core.Test

open NUnit.Framework
open ADP2.Core
open FsUnit
open System.IO

type SecondCourseMetadataSourceTest() =

    let config = 
        { ApplicationName = "ADP2"
          CredentialsFile = "../../../../../../credentials.json" 
          SpreadsheetId = "1b1fhGFInVDNXAb_Ok14Nl03V-DviKe-GrE2Geuwsw9o" }

    [<Test>]
    member _.MetadataForTheFirstStudentShouldBeReceivedCorrectly () =
        if File.Exists config.CredentialsFile then
            let dataSource = SecondCourseMetadataSource(config) :> IWorkMetadataSource
            let works = dataSource.GetWorksMetadata()
            let firstStudent = Seq.head works
            firstStudent.AdvisorSurname |> should equal "Мордвинов"
            firstStudent.AuthorName |> should equal "Баруткин Илья Дмитриевич"
            firstStudent.ShortName |> should equal "Barutkin"
            firstStudent.Title |> should equal "Реализация проекции на основе модели в теории битовых векторов"
        else
            Assert.Ignore("No credentials for Google Sheets found.")

    [<Test>]
    member _.MetadataShouldContainPloskin () =
        if File.Exists config.CredentialsFile then
            let dataSource = SecondCourseMetadataSource(config) :> IWorkMetadataSource
            let works = dataSource.GetWorksMetadata()
            works |> Seq.tryFind (fun w -> w.ShortName = "Ploskin") |> should be (ofCase <@Some@>)
        else
            Assert.Ignore("No credentials for Google Sheets found.")

    [<Test>]
    member _.MetadataShouldContainBabich () =
        if File.Exists config.CredentialsFile then
            let dataSource = SecondCourseMetadataSource(config) :> IWorkMetadataSource
            let works = dataSource.GetWorksMetadata()
            works |> Seq.tryFind (fun w -> w.ShortName = "Babich") |> should be (ofCase <@Some@>)
        else
            Assert.Ignore("No credentials for Google Sheets found.")

    [<Test>]
    member _.MetadataShouldContainBothPorsevs () =
        if File.Exists config.CredentialsFile then
            let dataSource = SecondCourseMetadataSource(config) :> IWorkMetadataSource
            let works = dataSource.GetWorksMetadata()
            works |> Seq.tryFind (fun w -> w.ShortName = "Porsev.Egor") |> should be (ofCase <@Some@>)
            works |> Seq.tryFind (fun w -> w.ShortName = "Porsev.Denis") |> should be (ofCase <@Some@>)
        else
            Assert.Ignore("No credentials for Google Sheets found.")
