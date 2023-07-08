namespace ADP2.Core

/// Global configuration used by all parts of the application and loaded from application-config.json
type ApplicationConfig =
    {
        /// Path to a JSON file with metadata configuration.
        MetadataConfigFile: string
        GoogleCredentialsFile: string
        GoogleApplicationName: string
    }
