namespace IonCubeGenerator
{
    using Harmony;
    using IonCubeGenerator.Buildable;
    using IonCubeGenerator.Craftables;

    // Adapted from https://github.com/kylinator25/SubnauticaMods/blob/master/AlienRifle/PDAScannerUnlockPatch.cs
    [HarmonyPatch(typeof(PDAScanner))]
    [HarmonyPatch(nameof(PDAScanner.Unlock))] // TODO - Figure this out
    public static class PDAScannerUnlockPatch
    {
        [HarmonyPrefix]
        public static void Prefix(PDAScanner.EntryData entryData)
        {
            if (entryData.key == TechType.PrecursorPrisonIonGenerator)
            {
                if (!KnownTech.Contains(CubeGeneratorBuildable.TechTypeID))
                {
                    KnownTech.Add(AlienEletronicsCase.TechTypeID);
                    KnownTech.Add(AlienIngot.TechTypeID);
                    KnownTech.Add(CubeGeneratorBuildable.TechTypeID);
                    ErrorMessage.AddMessage(CubeGeneratorBuildable.BlueprintUnlockedMsg());
                }
            }
        }
    }
}
