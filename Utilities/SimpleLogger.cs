namespace Common
{
    using System;
    using System.IO;
    
    internal class SimpleLogger
    {
        private readonly string _modFolder;
        private readonly string _modName;
        private const string _fileName = "log.txt";
        private readonly string _path;

        internal SimpleLogger(string modFolder, string modName)
        {
            _modFolder = modFolder;
            _modName = modName;

            _path = $"{_modFolder}/{_fileName}";
        }

        public void Log(string msg)
        {
            WriteLog(msg);
        }

        private void WriteLog(string msg)
        {
            string logLine = $"[{_modName}] - {DateTime.Now.ToString()} - {msg}{Environment.NewLine}";

            if (File.Exists(_path))
            {
                var writer = File.AppendText(_path);
                writer.WriteLine(logLine);
                writer.Close();
            }
            else
            {
                File.WriteAllText(_path, logLine);
            }
        }
    }
}
