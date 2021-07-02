namespace EasyMarkup
{
    using System;
    using System.Globalization;
    using System.Threading;
    using Common;

    internal static class EmUtils
    {
        public static bool Deserialize<T>(this T emProperty, string serializedData) where T : EmProperty
        {
            // Accounting for CurrentCultureInfo became necessary with the jump to Unity2019 and/or .NET 4
            CultureInfo originalCulture = Thread.CurrentThread.CurrentCulture;
            bool success = false;
            try
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                success = emProperty.FromString(serializedData);
            }
            catch (EmException emEx)
            {
                success = false;
                QuickLogger.Error($"[EasyMarkup] Deserialize halted unexpectedly for {emProperty.Key}{Environment.NewLine}{emEx}");
            }
            catch (Exception ex)
            {
                success = false;
                QuickLogger.Error($"[EasyMarkup] Deserialize halted unexpectedly for {emProperty.Key}{Environment.NewLine}Error reported: {ex}");
            }
            finally
            {
                // To avoid any unexpected side-effect, we'll change this back once we're done reading the file.
                Thread.CurrentThread.CurrentCulture = originalCulture;
            }

            return success;
        }

        public static string Serialize<T>(this T emProperty, bool prettyPrint = true) where T : EmProperty
        {
            // Accounting for CurrentCultureInfo became necessary with the jump to Unity2019 and/or .NET 4
            CultureInfo originalCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            string serialized = prettyPrint ? emProperty.PrettyPrint() : emProperty.ToString();

            // To avoid any unexpected side-effect, we'll change this back once we're done writing the file.
            Thread.CurrentThread.CurrentCulture = originalCulture;

            return serialized;
        }

        public static bool DeserializeKeyOnly<T>(this T emProperty, string serializedData, out string foundKey) where T : EmProperty
        {
            try
            {
                return EmProperty.CheckKey(serializedData, out foundKey, emProperty.Key);
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"[EasyMarkup] DeserializeKeyOnly halted unexpectedly for {emProperty.Key}{Environment.NewLine}" +
                           $"Error reported: {ex}");
                foundKey = null;
                return false;
            }
        }

        public static string CommentText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return $"{EmProperty.SpChar_CommentBlock} {text} {EmProperty.SpChar_CommentBlock}";
        }

        public static string[] CommentTextLines(string[] textArray)
        {
            string[] commentedArray = new string[textArray.Length];

            for (int i = 0; i < textArray.Length; i++)
                commentedArray[i] = CommentText(textArray[i]);

            return commentedArray;
        }

        public static string[] CommentTextLinesCentered(string[] textArray)
        {
            string[] commentedArray = new string[textArray.Length];

            int maxSize = 0;
            for (int i = 0; i < textArray.Length; i++)
                maxSize = Math.Max(maxSize, textArray[i].Length);

            if (maxSize % 2 != 0)
                maxSize++;

            for (int i = 0; i < textArray.Length; i++)
            {
                int totalPadding = maxSize - textArray[i].Length;
                int leftPad;
                int rightPad;

                if (totalPadding % 2 == 0)
                {
                    rightPad = leftPad = totalPadding / 2;
                }
                else
                {
                    leftPad = (totalPadding - 1) / 2;
                    rightPad = (totalPadding - 1) / 2 + 1;
                }

                string paddedText = $"{new string(' ', leftPad)}{textArray[i]}{new string(' ', rightPad)}";

                commentedArray[i] = CommentText(paddedText);
            }

            return commentedArray;
        }
    }
}
