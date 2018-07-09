namespace CustomCraft2SML
{
    using System;

    internal static class Logger
    {
        private const string MsgPrefix = "[CustomCraft2SML] ";

        internal static void Log(string msg)
        {
            Console.WriteLine(MsgPrefix + msg);
        }

        internal static void Log(params string[] msgs)
        {
            foreach (string msg in msgs)
            {
                Console.WriteLine(MsgPrefix + msg);
            }            
        }

    }
}
