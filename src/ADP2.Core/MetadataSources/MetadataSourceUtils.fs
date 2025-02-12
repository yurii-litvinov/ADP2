﻿namespace ADP2.Core

/// A set of useful functions to work with works metadata.
module MetadataSourceUtils =

    /// Common configuration of metadata, shared by Google and Yandex metadata sources.
    type DataConfig =
        { SheetName: string

          AuthorNameColumn: string
          AdvisorColumn: string
          TitleColumn: string
          SourceUriColumn: string
          CommitterNameColumn: string
          ResultColumn: string
          DoNotPublishColumn: string }

    /// Marks in a "Зачёт" column that meant that a work was successfully defended.
    let allowedResults = Set.ofList [ "A"; "B"; "C"; "D"; "E"; "да" ]

    /// Gets surname from full name in "Surname Name MiddleName" format.
    let getSurname (name: string) = name.Split([| ' ' |]) |> Seq.head

    /// Returns advisor name and surname from full name in "Surname Name MiddleName" format or
    /// "Dr. I.I. Surname" format.
    let parseAdvisor (raw: string) =
        let surname =
            if raw.Contains('.') then
                raw.Split([| ' '; '.' |]) |> Seq.last
            else
                getSurname raw

        let name =
            if raw.Contains('.') then
                raw.[0 .. raw.Length - surname.Length - 1]
            else if raw.Contains(' ') |> not then
                ""
            else
                raw.Split([| ' ' |]) |> Seq.skip 1 |> Seq.head

        (name, surname)

    /// Searches for non-unique surnames in a Work sequence and adds first name after dot for them.
    let addNamesIfNeeded (works: Work seq) =
        let surnameCounts = works |> Seq.countBy (fun w -> w.ShortName) |> Map.ofSeq

        works
        |> Seq.map (fun w ->
            if surnameCounts.[w.ShortName] > 1 then
                w.ShortName <- w.ShortName + "." + (w.AuthorName.Split(' ').[1])

            w)

    /// Gets what optional column names are defined in config
    let isDefinedOptionalColumns (config: DataConfig) =
        let sourceUriDefined =
            not (config.SourceUriColumn = "-" || config.SourceUriColumn = "")

        let doNotPublishDefined =
            not (config.DoNotPublishColumn = "-" || config.DoNotPublishColumn = "")

        (sourceUriDefined, doNotPublishDefined)

    /// Gets "interesting" column names from config, ignoring undefined optional columns
    let getInterestingColumns (config: DataConfig) =
        let columns =
            [ config.AuthorNameColumn
              config.AdvisorColumn
              config.TitleColumn
              config.ResultColumn ]

        let sourceUriDefined, doNotPublishDefined = isDefinedOptionalColumns config

        let columns =
            if sourceUriDefined then
                if config.CommitterNameColumn = "" then
                    config.SourceUriColumn :: columns
                else
                    config.CommitterNameColumn :: config.SourceUriColumn :: columns
            else
                columns

        let columns =
            if doNotPublishDefined then
                config.DoNotPublishColumn :: columns
            else
                columns

        columns

    /// Constructs new Work record.
    let createWorkMetadata author advisor title (sourceUrl: string) committerName doNotPublish =
        let cleanup (str: string) =
            str.Replace("\r", "").Replace("\n", " ")

        let work = Work(author |> getSurname)
        let advisor = parseAdvisor advisor
        work.AdvisorName <- fst advisor
        work.AdvisorSurname <- snd advisor
        work.AuthorName <- author
        work.Title <- cleanup title
        work.SourceUri <- if sourceUrl.StartsWith "http" then sourceUrl else ""
        work.CommitterName <- if work.SourceUri = "" then "" else committerName
        work.DoNotPublish <- (cleanup doNotPublish) <> ""
        work
