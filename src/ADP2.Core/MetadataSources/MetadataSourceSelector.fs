namespace ADP2.Core

open FSharp.Json
open System.IO

/// Module for selecting appropriate work metadata source using application config.
module MetadataSourceSelector =

    /// Part of a metadata config interesting for selector.
    type Config =
        { YandexSheetFolderUrl: string
          YandexSheetFileName: string
          GoogleSheetId: string }

    /// Selects and creates metadata source according to an URL of spreadshhet with metadata.
    /// Google Sheets and Yandex.Disk sources are supported.
    let selectMetadataSource (appConfig: ApplicationConfig) : IMetadataSource =
        let dataConfig =
            Json.deserialize<Config> (File.ReadAllText appConfig.MetadataConfigFile)

        if dataConfig.YandexSheetFolderUrl.Length > 0 then
            YandexDiskMetadataSource(appConfig)
        else
            GoogleSheetsMetadataSource(appConfig)
