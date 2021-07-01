namespace ADP2.Core.Test

open NUnit.Framework
open ADP2.Core
open FsUnit
open System.IO

type ThirdCourseMetadataSourceTest() =

    let config = 
        { ApplicationName = "ADP2"
          CredentialsFile = "../../../../../../credentials.json" 
          SpreadsheetId = "1YSEU5KglOTxQGJwFNjNtRSo-VI9wbIvHuPd7_ncI33Y" }

    [<Test>]
    member _.MetadataForTheFirstStudentShouldBeReceivedCorrectly () =
        if File.Exists config.CredentialsFile then
            let dataSource = ThirdCourseMetadataSource(config) :> IWorkMetadataSource
            let works = dataSource.GetWorksMetadata()
            let firstStudent = Seq.head works
            firstStudent.AdvisorSurname |> should equal "Литвинов"
            firstStudent.AuthorName |> should equal "Асланов Элтон Хосрович"
            firstStudent.ShortName |> should equal "Aslanov"
            firstStudent.Title |> should equal "Реализация редактора для разметки DICOM-изображений"
        else
            Assert.Ignore("No credentials for Google Sheets found.")

    [<Test>]
    member _.MetadataShouldContainFomina () =
        if File.Exists config.CredentialsFile then
            let dataSource = SecondCourseMetadataSource(config) :> IWorkMetadataSource
            let works = dataSource.GetWorksMetadata()
            works |> Seq.tryFind (fun w -> w.ShortName = "Fomina") |> should be (ofCase <@Some@>)
        else
            Assert.Ignore("No credentials for Google Sheets found.")
