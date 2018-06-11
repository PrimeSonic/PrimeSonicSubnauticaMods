namespace CustomCraftSML.Serialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using Oculus.Newtonsoft.Json;

    internal class ModifiedSize
    {
        private static readonly Regex ModSizeRegex = new Regex(@"\(\s?TechType:\s?([a-zA-Z]+);\s?Width:\s?(\d);\s?Height:\s?(\d)\s?\)", RegexOptions.Compiled);

        internal TechType InventoryItem { get; set; }
        internal int Width { get; set; }
        internal int Height { get; set; }

        internal static IList<ModifiedSize> Parse(string[] serializedStrings)
        {
            var list = new List<ModifiedSize>(serializedStrings.Length);

            foreach (string item in serializedStrings)
            {
                var modSize = Parse(item);

                if (modSize != null)
                    list.Add(modSize);
            }

            if (list.Count > 0)
                return list;

            return null;
        }

        internal static ModifiedSize Parse(string serialized)
        {
            Match match = ModSizeRegex.Match(serialized);

            if (match.Success)
            {
                if (match.Groups.Count < 4)
                {
                    Logger.Log($"Error parsing Modified size string: {serialized}");
                    return null;
                }

                string techTypeGroup = match.Groups[1].Value;
                string widthGroup = match.Groups[2].Value;
                string heightGroup = match.Groups[3].Value;

                TechType techType;
                int width;
                int height;

                try
                {
                    techType = (TechType)Enum.Parse(typeof(TechType), techTypeGroup);
                    width = int.Parse(widthGroup);
                    height = int.Parse(heightGroup);

                }
                catch (Exception ex)
                {
                    Logger.Log($"Error parsing Modified size string: {serialized}", ex.ToString());
                    return null;
                }

                return new ModifiedSize
                {
                    InventoryItem = techType,
                    Width = width,
                    Height = height
                };
            }

            Logger.Log($"Error parsing Modified size string: {serialized}", "");
            return null;
        }

        public override string ToString()
        {
            return $"(TechType:{InventoryItem};Width:{Width};Height:{Height})";
        }
    }
}
