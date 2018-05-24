namespace Common
{
    using System;
    using System.IO;
    using Oculus.Newtonsoft.Json;

    /// <summary>
    /// A class for handling simple config file loading and saving.
    /// </summary>
    /// <typeparam name="T">The config class.</typeparam>
    internal class ConfigManager<T> where T : new()
    {
        private readonly string _fileLocation;
        private readonly string _modName;

        public ConfigManager(string modName, string fileLocation)
        {
            _fileLocation = fileLocation;
            _modName = modName;
        }

        /// <summary>
        /// Retrieves the config from the serialized file source.
        /// </summary>
        /// <param name="config">The config class instance. Will return the default instance in case of an error.</param>
        /// <returns><c>True</c> if the config file was successfully found and desirialized; Otherwise <c>false</c>.</returns>
        public bool GetConfig(out T config)
        {
            string json = GetJsonStringAtLocation();

            if (string.IsNullOrEmpty(json))
            {
                var t = new T();
                config = t;
                return false;
            }
            else
            {
                config = JsonConvert.DeserializeObject<T>(json);
                return true;
            }
        }

        /// <summary>
        /// Writes the serialized file with values from the config instance.
        /// </summary>
        /// <param name="config">The config class instance.</param>
        /// <returns><c>True</c> if the config file was successfully serialized and written; Otherwise <c>false</c>.</returns>
        public bool SaveConfig(T config)
        {
            string json = JsonConvert.SerializeObject(config);

            try
            {
                File.WriteAllText(_fileLocation, json);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{_modName}] {ex}");
                return false;
            }
        }

        private string GetJsonStringAtLocation()
        {
            try
            {
                if (File.Exists(_fileLocation))
                {
                    return File.ReadAllText(_fileLocation);
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{_modName}] {ex}");
                return null;
            }
        }
    }

}

