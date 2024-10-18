open System
open System.IO
open FSharp.Json
open ADP2.Core

[<EntryPoint>]
let main _ =
    if not <| File.Exists "_config.json" then
        File.Copy(AppDomain.CurrentDomain.BaseDirectory + "/config.json", "_config.json")
        printfn "Generated _config.json file, please edit it if needed and run program again."
        printfn "See README.md for allowed work types and programmes."
    else
        let appConfig =
            Json.deserialize<ApplicationConfig> (
                File.ReadAllText
                <| AppDomain.CurrentDomain.BaseDirectory + "application-config.json"
            )

        let knowledgeBase = ADP2.Core.Workflow.generateWorksInfo appConfig

        if knowledgeBase.HasErrors then
            printfn "Unknown files:"
            knowledgeBase.UnknownFiles |> Seq.iter (printfn "%s")

            printfn ""
            printfn "Works with no files:"

            knowledgeBase.WorksWithNoFiles
            |> Seq.iter (fun w -> printfn "%s: (transliterated as '%s') %s" w.ShortName (DataModelUtils.transliterate w.ShortName) w.Title)

            printfn ""
            printfn "Works with no metainformation:"

            knowledgeBase.WorksWithNoMetainformation
            |> Seq.iter (fun w -> printfn "%s" w.ShortName)

        if not <| Seq.isEmpty knowledgeBase.WorksNotForPublishing then
            printfn ""
            printfn "Works explicitly marked as 'Not for publishing':"

            knowledgeBase.WorksNotForPublishing
            |> Seq.iter (fun w -> printfn "%s" w.ShortName)

        if not <| File.Exists "_upload.py" then
            File.Copy(AppDomain.CurrentDomain.BaseDirectory + "/_upload.py", "_upload.py")

        printfn ""

        printfn
            "_out.json and _upload.py were generated. Please check for erros in output and rename files if needed. Then check _out.json manually, then, if everything is ok, run 'python _upload.py'."

    0
