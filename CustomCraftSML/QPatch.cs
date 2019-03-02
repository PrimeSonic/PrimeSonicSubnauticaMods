namespace CustomCraft2SML
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using Common;

    public static class QPatch
    {        
        private static readonly Regex LogLevel = new Regex("\"DebugLogsEnabled\"[ \f\n\r\t\v]*:[ \f\n\r\t\v]*(false|true),", RegexOptions.IgnoreCase); // Oldschool whitespace checks for .NET 3.5

        public static void Patch()
        {
            QuickLogger.Info($"Started patching. Version {QuickLogger.GetAssemblyVersion()}");

            try
            {
                CheckLogLevel();

                HelpFilesWriter.HandleHelpFiles();
                
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

            if (match.Success && match.Groups.Count > 1)
            {
                Group capturedValue = match.Groups[1];

                if (bool.TryParse(capturedValue.Value, out bool result))
                {
                    QuickLogger.DebugLogsEnabled = result;                    
                }
            }

            if (QuickLogger.DebugLogsEnabled)
                QuickLogger.Debug("Debug logging is enable");
            else
                QuickLogger.Info("To enable Debug logging, change the \"DebugLogsEnabled\" attribute in the mod.json file to true");
        }
    }
}
