namespace Common
{
    using System;
    using System.Reflection;

    internal static class QuickLogger
    {
        private static readonly AssemblyName ModName = Assembly.GetExecutingAssembly().GetName();

        internal static bool DebugLogsEnabled = false;

        public static void Info(string msg, bool showOnScreen = false, AssemblyName callingAssembly = null)
        {
            Console.WriteLine($"[{(callingAssembly ?? ModName).Name}:INFO] {msg}");

            if (showOnScreen)
                ErrorMessage.AddMessage(msg);
        }

        public static void Debug(string msg, bool showOnScreen = false, AssemblyName callingAssembly = null)
        {
            if (!DebugLogsEnabled)
                return;

            Console.WriteLine($"[{(callingAssembly ?? ModName).Name}:DEBUG] {msg}");

            if (showOnScreen)
                ErrorMessage.AddDebug(msg);
        }

        public static void Error(string msg, bool showOnScreen = false, AssemblyName callingAssembly = null)
        {
            Console.WriteLine($"[{(callingAssembly ?? ModName).Name}:ERROR] {msg}");

            if (showOnScreen)
                ErrorMessage.AddError(msg);
        }

        public static void Error(string msg, Exception ex, AssemblyName callingAssembly = null)
        {
            Console.WriteLine($"[{(callingAssembly ?? ModName).Name}:ERROR] {msg}{Environment.NewLine}{ex.ToString()}");
        }

        public static void Error(Exception ex, AssemblyName callingAssembly = null)
        {
            Console.WriteLine($"[{(callingAssembly ?? ModName).Name}:ERROR] {ex.ToString()}");
        }

        public static void Warning(string msg, bool showOnScreen = false, AssemblyName callingAssembly = null)
        {
            Console.WriteLine($"[{(callingAssembly ?? ModName).Name}:WARN] {msg}");

            if (showOnScreen)
                ErrorMessage.AddWarning(msg);
        }

        /// <summary>
        /// Creates the version string in format "#.#.#" or "#.#.# rev:#"
        /// </summary>
        public static string GetAssemblyVersion()
        {
            Version version = ModName.Version;

            //      Major Version
            //      Minor Version
            //      Build Number
            //      Revision

            if (version.Revision > 0)
            {
                return $"{version.Major}.{version.Minor}.{version.Build} rev:{version.Revision}";
            }

            if (version.Build > 0)
            {
                return $"{version.Major}.{version.Minor}.{version.Build}";
            }

            if (version.Minor > 0)
            {
                return $"{version.Major}.{version.Minor}.0";
            }

            return $"{version.Major}.0.0";
        }

        public static string GetAssemblyName()
        {
            return ModName.Name;
        }
    }
}
