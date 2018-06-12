namespace CustomCraftSML.Serialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using Oculus.Newtonsoft.Json;
    using SMLHelper.Patchers;

    internal class ModifiedSize : SerialDefinition
    {
        internal TechType ItemID
        {
            get => (Properties["ItemID"] as TechTypeValue).GetTypedValue1();
            set => (Properties["ItemID"] as TechTypeValue).Value1 = value.ToString();
        }

        internal int AmountCrafted
        {
            get => (Properties["AmountCrafted"] as SingleValue<int>).GetTypedValue1();
            set => (Properties["AmountCrafted"] as SingleValue<int>).Value1 = value.ToString();
        }

        internal List<IngredientHelper> Ingredients
        {
            get;
            set;
        }

        internal ModifiedSize()
            : base("Size", new Dictionary<string, ValueType>()
            {
                { "ItemID", new TechTypeValue() },
                { "AmountCrafted", new SingleValue<int>() },
                { "Ingredients", new DoubleList<string,int>() },
                { "LinkedItemIDs", new SingleList<string>() },
            })
        {

        }
    }
}
