namespace ADP2.Core.Test

open NUnit.Framework
open ADP2.Core
open FsUnit
open System.IO

type ThirdCourseMetadataSourceTest() =

    let config = 
        { ApplicationName = "ADP2"
          CredentialsFile = "../../../../../../credentials.json" 
          MetadataConfigFile = "Data/thirdCourseConfig.json" }

    [<Test>]
    member _.MetadataForTheFirstStudentShouldBeReceivedCorrectly () =
        if File.Exists config.CredentialsFile then
            let dataSource = MetadataSource(config)
            let works = dataSource.GetWorksMetadata()
            let firstStudent = Seq.head works
            firstStudent.AdvisorSurname |> should equal "Смирнов"
            firstStudent.AuthorName |> should equal "Израилев Андрей Дмитриевич"
            firstStudent.ShortName |> should equal "Izrailev"
            firstStudent.Title |> should equal "Использование DPI для формирования правил аппаратного ускорения трафика в условиях ограниченных ресурсов"
            firstStudent.SourceUri |> should equal ""
            firstStudent.CommitterName |> should equal ""
        else
            Assert.Ignore("No credentials for Google Sheets found.")

    [<Test>]
    member _.MetadataShouldContainBothPorsevs () =
        if File.Exists config.CredentialsFile then
            let dataSource = MetadataSource(config)
            let works = dataSource.GetWorksMetadata()
            works |> Seq.tryFind (fun w -> w.ShortName = "Porsev.Egor") |> should be (ofCase <@Some@>)
            works |> Seq.tryFind (fun w -> w.ShortName = "Porsev.Denis") |> should be (ofCase <@Some@>)
        else
            Assert.Ignore("No credentials for Google Sheets found.")

    [<Test>]
    member _.MetadataShouldContainSourceUriAndCommitter() =
        if File.Exists config.CredentialsFile then
            let dataSource = MetadataSource(config)
            let works = dataSource.GetWorksMetadata()
            works |> Seq.tryFind (fun w -> w.ShortName = "Usmanov") |> should be (ofCase <@Some@>)
            let usmanov = works |> Seq.find (fun w -> w.ShortName = "Usmanov")
            usmanov.SourceUri |> should equal "https://github.com/spbu-se/WebAutomataConstructor"
            usmanov.CommitterName |> should equal "ususucsus"
        else
            Assert.Ignore("No credentials for Google Sheets found.")
