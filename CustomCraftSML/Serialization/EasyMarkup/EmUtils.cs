namespace CustomCraft2SML.Serialization.EasyMarkup
{
    using System;

    public static class EmUtils
    {
        public static bool Deserialize(this IEmProperty emProperty, string serializedData)
        {
            try
            {
                emProperty.FromString(serializedData);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log($"Error on Deserialize for {emProperty.Key}:{Environment.NewLine}{ex}");
                return false;
            }
        }

        public static string Serialize(this IEmProperty emProperty)
        {
            return emProperty.PrintyPrint();
        }


    }
}
