﻿namespace ADP2.Core

open DocUtils
open MetadataSourceUtils
open FSharp.Json
open System.IO

type DataConfig = 
    { GoogleSheetId: string
      SheetName: string
      AuthorNameColumn: string
      AdvisorColumn: string
      TitleColumn: string
      SourceUriColumn: string
      CommitterNameColumn: string
      ResultColumn: string
    }

/// Implementation of metadata source for qualification works Google tables. Required columns are
/// configured via config.json file.
/// For example, see https://docs.google.com/spreadsheets/d/103E-S9SxzRAPiTt_Yr6428243hzIKzjy1WaLR_W7HDM
type MetadataSource(appConfig: GlobalConfig) =

    let readSheet (config: DataConfig) (sheet: Sheet) =
        let columns = [config.AuthorNameColumn; config.AdvisorColumn; config.TitleColumn; config.ResultColumn]
        let columns = 
            if config.SourceUriColumn = "-" then 
                columns 
            else 
                config.CommitterNameColumn :: config.SourceUriColumn :: columns
        
        let rows = sheet.ReadByHeaders(columns)
        rows
        |> Seq.choose (
            fun row ->
                if allowedResults.Contains row[config.ResultColumn] then
                    if config.SourceUriColumn = "-" then
                        Some <| createWorkMetadata 
                                    row[config.AuthorNameColumn] 
                                    row[config.AdvisorColumn] 
                                    row[config.TitleColumn] 
                                    "" 
                                    ""
                    else
                        Some <| createWorkMetadata 
                                    row[config.AuthorNameColumn] 
                                    row[config.AdvisorColumn] 
                                    row[config.TitleColumn] 
                                    row[config.SourceUriColumn] 
                                    row[config.CommitterNameColumn] 
                else
                    None
            )

    member _.GetWorksMetadata () =
        let dataConfig = Json.deserialize<DataConfig>(File.ReadAllText appConfig.MetadataConfigFile)

        let service = new GoogleSheetService(appConfig.CredentialsFile, appConfig.ApplicationName)

        let sheet = service.Sheet(dataConfig.GoogleSheetId, dataConfig.SheetName)
        sheet 
        |> (readSheet dataConfig)
        |> addNamesIfNeeded
        |> Seq.toList