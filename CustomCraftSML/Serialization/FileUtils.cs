namespace CustomCraft2SML.Serialization
{
    using System;
    using System.IO;
    using Common;

    public static class FileUtils
    {
        public static string ReadStringFromFile(string fileLocation, string defaultString = null)
        {
            try
            {
                if (File.Exists(fileLocation))
                    return File.ReadAllText(fileLocation);

                string contents = defaultString ?? string.Empty;
                File.WriteAllText(fileLocation, contents);
                return contents;
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Error on ReadStringFromFile '{fileLocation}':{Environment.NewLine}{ex}");
                return null;
            }

        }

        public static bool WriteStringToFile(string fileLocation, string contents)
        {
            try
            {
                File.WriteAllText(fileLocation, contents);
                return true;
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Error on WriteStringToFile '{fileLocation}':{Environment.NewLine}{ex}");
                return false;
            }
        }
    }

}

