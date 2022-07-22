open System
open System.IO
open FSharp.Json
open ADP2.Core

type ApplicationConfig =
    { CredentialsFile: string }

[<EntryPoint>]
let main _ =
    if not <| File.Exists "_config.json" then
        File.Copy(AppDomain.CurrentDomain.BaseDirectory + "/config.json", "_config.json")
        printfn "Generated _config.json file, please edit it if needed and run program again."
        printfn "See README.md for allowed work types and programmes."
    else
        let appConfig = 
            Json.deserialize<ApplicationConfig>
                (File.ReadAllText <| AppDomain.CurrentDomain.BaseDirectory + "application-config.json")

        let globalConfig = 
            { ApplicationName = "ADP2"
              CredentialsFile = appConfig.CredentialsFile
              MetadataConfigFile = "_config.json" }

        let metadataSource = MetadataSource(globalConfig)

        let knowledgeBase = ADP2.Core.Workflow.generateWorksInfo metadataSource

        if knowledgeBase.HasErrors then
            printfn "Unknown files:"
            knowledgeBase.UnknownFiles |> Seq.iter (printfn "%s")

            printfn ""
            printfn "Works with no files:"
            knowledgeBase.WorksWithNoFiles |> Seq.iter (fun w -> printfn "%s: %s" w.ShortName w.Title)

            printfn ""
            printfn "Works with no metainformation:"
            knowledgeBase.WorksWithNoMetainformation |> Seq.iter (fun w -> printfn "%s" w.ShortName)

        if not <| File.Exists "_upload.py" then
            File.Copy(AppDomain.CurrentDomain.BaseDirectory + "/_upload.py", "_upload.py")

        printfn ""
        printfn "_out.json and _upload.py were generated. Please check for erros in output and rename files if needed. Then check _out.json manually, then, if everything is ok, run 'python _upload.py'."
    0