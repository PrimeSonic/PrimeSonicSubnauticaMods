namespace CustomCraftSML.Serialization.EasyMarkup
{

    using System.Collections.Generic;

    public class EmPropertyCollectionList : EmProperty
    {
        protected EmPropertyCollection Definitions;

        public EmPropertyCollection this[int index] => Collections[index];

        public List<EmPropertyCollection> Collections;

        public EmPropertyCollectionList(string key, ICollection<EmProperty> definitions)
        {
            Key = key;

            Definitions = new EmPropertyCollection(key, definitions);

            foreach (EmProperty property in definitions)
                Definitions[property.Key] = property;
        }

        public override string ToString()
        {
            var val = $"{Key}:";
            foreach (EmPropertyCollection collection in Collections)
            {
                val += "(";

                foreach (var key in collection.Keys)
                {
                    val += $"{collection[key]}";
                }

                val += "),";
            }

            return val.TrimEnd(',') + ";";
        }

        protected override string ExtractValue(StringBuffer fullString)
        {
            if (Collections == null)
                Collections = new List<EmPropertyCollection>();

            string serialValues = "(";

            int openParens = 0;

            var buffer = new StringBuffer();

            do
            {
                switch (fullString.PeekStart())
                {
                    case ';' when openParens == 0: // End of ComplexList                        
                    case ',' when openParens == 0 && fullString.Count > 0: // End of a nested property belonging to this collection
                        fullString.RemoveFromStart();

                        var collection = (EmPropertyCollection)Definitions.Copy();
                        collection.FromString($"{Key}:{buffer.ToString()};");
                        Collections.Add(collection);
                        buffer.Clear();
                        serialValues += $"{collection.SerializedValue},";
                        break;
                    case '(':
                        openParens++;
                        goto default;
                    case ')':
                        openParens--;
                        goto default;
                    default:

                        buffer.AddFromEnd(fullString.RemoveFromStart());
                        break;
                }
            } while (fullString.Count > 0);

            return serialValues.TrimEnd(',') + ")";
        }

        internal override EmProperty Copy()
        {
            var definitions = new List<EmProperty>(Definitions.Keys.Count);
            foreach (EmProperty item in Definitions.Values)
                definitions.Add(item.Copy());

            return new EmPropertyCollectionList(Key, definitions);
        }
    }

}