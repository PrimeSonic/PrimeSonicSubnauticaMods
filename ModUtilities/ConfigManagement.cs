namespace ModUtilities
{
    using System;
    using System.IO;
    using Newtonsoft.Json;

    public interface IConfigManager<T>
    {
        /// <summary>
        /// Retrieves the config from the serialized file source.
        /// </summary>
        /// <param name="config">The config class instance. Will return the default instance in case of an error.</param>
        /// <returns><c>True</c> if the config file was successfully found and desirialized; Otherwise <c>false</c>.</returns>
        bool GetConfig(out T config);

        /// <summary>
        /// Writes the serialized file with values from the config instance.
        /// </summary>
        /// <param name="config">The config class instance.</param>
        /// <returns><c>True</c> if the config file was successfully serialized and written; Otherwise <c>false</c>.</returns>
        bool SaveConfig(T config);

    }

    public class ConfigManager<T> : IConfigManager<T>
    {
        private readonly string _fileLocation;


        public ConfigManager(string fileLocation)
        {
            _fileLocation = fileLocation;            
        }

        public bool GetConfig(out T config)
        {
            string json = GetJsonStringAtLocation();

            if (string.IsNullOrEmpty(json))
            {
                config = default(T);
                return false;
            }
            else
            {
                config = JsonConvert.DeserializeObject<T>(json);
                return true;
            }
        }

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
                Console.WriteLine(ex.ToString());
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
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
    }

}

