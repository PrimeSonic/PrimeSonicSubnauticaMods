namespace Common
{
    using System;
    using System.Diagnostics;
    using System.Reflection;

    internal static class QuickLogger
    {
        internal static bool DebugLogsEnabled = false;

        public static void Info(string msg, bool showOnScreen = false, Assembly callingAssembly = null)
        {
            string name = (callingAssembly ?? Assembly.GetCallingAssembly()).GetName().Name;

            Console.WriteLine($"[{name}:INFO] {msg}");

            if (showOnScreen)
                ErrorMessage.AddMessage(msg);
        }

        public static void Debug(string msg, bool showOnScreen = false, Assembly callingAssembly = null)
        {
            if (!DebugLogsEnabled)
                return;

            string name = (callingAssembly ?? Assembly.GetCallingAssembly()).GetName().Name;

            Console.WriteLine($"[{name}:DEBUG] {msg}");

            if (showOnScreen)
                ErrorMessage.AddDebug(msg);
        }

        public static void Error(string msg, bool showOnScreen = false, Assembly callingAssembly = null)
        {
            string name = (callingAssembly ?? Assembly.GetCallingAssembly()).GetName().Name;

            Console.WriteLine($"[{name}:ERROR] {msg}");

            if (showOnScreen)
                ErrorMessage.AddError(msg);
        }

        public static void Error(string msg, Exception ex, Assembly callingAssembly = null)
        {
            string name = Assembly.GetCallingAssembly().GetName().Name;

            Console.WriteLine($"[{name}:ERROR] {msg}{Environment.NewLine}{ex.ToString()}");
        }

        public static void Error(Exception ex, Assembly callingAssembly = null)
        {
            string name = (callingAssembly ?? Assembly.GetCallingAssembly()).GetName().Name;

            Console.WriteLine($"[{name}:ERROR] {ex.ToString()}");
        }

        public static void Warning(string msg, bool showOnScreen = false, Assembly callingAssembly = null)
        {
            string name = (callingAssembly ?? Assembly.GetCallingAssembly()).GetName().Name;

            Console.WriteLine($"[{name}:WARN] {msg}");

            if (showOnScreen)
                ErrorMessage.AddWarning(msg);
        }

        public static string GetAssemblyVersion()
        {
            return GetAssemblyVersion(Assembly.GetExecutingAssembly());
        }

        public static string GetAssemblyVersion(Assembly assembly)
        {            
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);

            if (fvi.FilePrivatePart > 0)
            {
                return $"{fvi.FileMajorPart}.{fvi.FileMinorPart}.{fvi.FileBuildPart}.{fvi.FilePrivatePart}";
            }
            else if (fvi.FileBuildPart > 0)
            {
                return $"{fvi.FileMajorPart}.{fvi.FileMinorPart}.{fvi.FileBuildPart}";
            }
            else if (fvi.FileMinorPart > 0)
            {
                return $"{fvi.FileMajorPart}.{fvi.FileMinorPart}";
            }

            return $"{fvi.FileMajorPart}";
        }
    }
}
