namespace CyclopsBioReactor
{
    using SMLHelper.V2.Json;
    using SMLHelper.V2.Options.Attributes;

    [Menu("Cyclops BioReactor Options")]
    public class CyBioConfig : ConfigFile
    {
        [Toggle( Label = "Show energy levels on display")]
        public bool EnergyOnDisplay = true;
    }
}
