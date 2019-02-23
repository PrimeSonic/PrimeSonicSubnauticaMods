namespace CustomCraft2SML
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using Common;

    public static class QPatch
    {        
        private static readonly Regex LogLevel = new Regex("\"DebugLogsEnabled\"\\s*:\\s*(?<value>false|true),", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static void Patch()
        {
            QuickLogger.Info($"Started patching. Version {QuickLogger.GetAssemblyVersion()}");

            try
            {
                CheckLogLevel();

                HelpFilesWriter.HandleReadMeFile();
                HelpFilesWriter.GenerateOriginalRecipes();
                WorkingFileParser.HandleWorkingFiles();

                QuickLogger.Info("Finished patching.");
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Critical error during patching{Environment.NewLine}{ex}");
            }
        }

        internal static void CheckLogLevel()
        {
            string jsonText = File.ReadAllText(FileLocations.ModJson);

            Match match = LogLevel.Match(jsonText);

            if (match.Success)
            {
                Group capturedValue = match.Groups["value"];

                if (bool.TryParse(capturedValue.Value, out bool result))
                {
                    QuickLogger.DebugLogsEnabled = result;
                }
            }
        }
    }
}
