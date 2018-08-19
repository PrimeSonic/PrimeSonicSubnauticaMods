namespace Common
{
    using System;
    using System.Diagnostics;
    using System.Reflection;

    internal static class QuickLogger
    {
        public static void Message(string msg, bool showOnScreen = false)
        {
            string name = Assembly.GetCallingAssembly().GetName().Name;

            Console.WriteLine($"[{name}] : {msg}");

            if (showOnScreen)
                ErrorMessage.AddMessage(msg);
        }

        public static void Debug(string msg, bool showOnScreen = false)
        {
            string name = Assembly.GetCallingAssembly().GetName().Name;

            Console.WriteLine($"[{name}] DEBUG: {msg}");

            if (showOnScreen)
                ErrorMessage.AddDebug(msg);
        }

        public static void Error(string msg, bool showOnScreen = false)
        {
            string name = Assembly.GetCallingAssembly().GetName().Name;

            Console.WriteLine($"[{name}] ERROR: {msg}");

            if (showOnScreen)
                ErrorMessage.AddError(msg);
        }

        public static void Error(Exception ex)
        {
            string name = Assembly.GetCallingAssembly().GetName().Name;

            Console.WriteLine($"[{name}] ERROR: {ex.ToString()}");
        }

        public static void Warning(string msg, bool showOnScreen = false)
        {
            string name = Assembly.GetCallingAssembly().GetName().Name;

            Console.WriteLine($"[{name}] WARN: {msg}");

            if (showOnScreen)
                ErrorMessage.AddWarning(msg);
        }

        public static string GetAssemblyVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.FileVersion;
        }
    }
}
