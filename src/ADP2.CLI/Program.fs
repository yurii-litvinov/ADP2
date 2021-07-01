open System
open System.IO
open FSharp.Json
open ADP2.Core

type ApplicationConfig =
    { CredentialsFile: string }

type Config = 
    { GoogleSheetId: string }

[<EntryPoint>]
let main _ =
    if not <| File.Exists "_config.json" then
        File.Copy(AppDomain.CurrentDomain.BaseDirectory + "/config.json", "_config.json")
        printfn "Generated _config.json file, please edit it if needed and run program again."
        printfn "Allowed work types:"
        printfn "  2 - practice"
        printfn "  3 - bachelors qualification work"
        printfn "  4 - masters qualification work"
    else
        let appConfig = Json.deserialize<ApplicationConfig>(AppDomain.CurrentDomain.BaseDirectory + "/application-config.json")
        let config = Json.deserialize<Config>(File.ReadAllText "_config.json")

        let metadataConfig = 
            { ApplicationName = "ADP2"
              CredentialsFile = appConfig.CredentialsFile
              SpreadsheetId = config.GoogleSheetId }

        let metadataSource = SecondCourseMetadataSource(metadataConfig)

        ADP2.Core.Workflow.generateWorksInfo metadataSource

        if not <| File.Exists "_upload.py" then
            File.Copy(AppDomain.CurrentDomain.BaseDirectory + "/_upload.py", "_upload.py")
    0