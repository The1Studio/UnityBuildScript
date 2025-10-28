namespace BuildScripts.Editor
{
    /// <summary>
    /// Constants used throughout the build system to avoid magic strings and numbers.
    /// </summary>
    public static class BuildConstants
    {
        // Paths

        /// <summary>
        /// Relative path to build logs directory from Unity project root.
        /// </summary>
        public const string BUILD_LOGS_PATH = "../Build/Logs";

        /// <summary>
        /// Relative path to build client output directory from Unity project root.
        /// </summary>
        public const string BUILD_CLIENT_PATH = "../Build/Client";

        // Log files

        /// <summary>
        /// Prefix for build report log files. Full filename format: {PREFIX}.{platform}.{EXTENSION}
        /// </summary>
        public const string LOG_FILE_PREFIX = "Build-Client-Report";

        /// <summary>
        /// File extension for build report log files.
        /// </summary>
        public const string LOG_FILE_EXTENSION = ".log";

        // GUI

        /// <summary>
        /// Standard width in pixels for "Remove" buttons in Unity Editor GUI.
        /// </summary>
        public const int REMOVE_BUTTON_WIDTH = 70;

        // iOS defaults

        /// <summary>
        /// Default minimum iOS version target for builds. Can be overridden via -iosTargetOSVersion command-line argument.
        /// </summary>
        public const string DEFAULT_IOS_TARGET_VERSION = "13.0";
    }
}
