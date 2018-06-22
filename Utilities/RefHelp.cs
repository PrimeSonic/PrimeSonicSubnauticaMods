namespace Common
{
    using System.Reflection;

    public static class RefHelp
    {
        public static object GetPrivateField<T>(this T instance, string fieldName, BindingFlags bindingFlags = BindingFlags.Default)
        {
            FieldInfo fieldInfo = typeof(T).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | bindingFlags);
            return fieldInfo.GetValue(instance);
        }

        public static void SetPrivateField<T>(this T instance, string fieldName, object value, BindingFlags bindingFlags = BindingFlags.Default)
        {
            FieldInfo fieldInfo = typeof(T).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | bindingFlags);
            fieldInfo.SetValue(instance, value);
        }

        public static void CloneFieldsInto<T>(this T original, T copy)
        {
            FieldInfo[] fieldsInfo = typeof(T).GetFields(BindingFlags.Instance);

            foreach (FieldInfo fieldInfo in fieldsInfo)
            {
                if (fieldInfo.GetType().IsClass)
                {
                    var origValue = fieldInfo.GetValue(original);
                    var copyValue = fieldInfo.GetValue(copy);

                    origValue.CloneFieldsInto(copyValue);                    
                }
                else
                {
                    var value = fieldInfo.GetValue(original);
                    fieldInfo.SetValue(copy, value);
                }                
            }
        }
    }
}
