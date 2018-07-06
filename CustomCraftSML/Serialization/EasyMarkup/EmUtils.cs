namespace CustomCraft2SML.Serialization.EasyMarkup
{
    using System;

    public static class EmUtils
    {
        public static bool Deserialize(this EmProperty emProperty, string serializedData)
        {
            try
            {
                return emProperty.FromString(serializedData);
            }
            catch (Exception ex)
            {
                Logger.Log($"Deserialize halted unexpectedly for {emProperty.Key}{Environment.NewLine}" +
                           $"Error reported: {ex}");
                return false;
            }
        }

        public static string Serialize(this EmProperty emProperty)
        {
            return emProperty.PrintyPrint();
        }


    }
}
