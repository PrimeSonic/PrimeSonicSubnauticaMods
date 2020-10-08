namespace CyclopsAutoZapper
{
    using SMLHelper.V2.Handlers;

    internal class DisplayTexts
    {
        private const string PowerLow = "azPowerLow";
        
        private const string DefCooling = "azDefCooldown";
        private const string DefCharged = "azDefCharged";
        private const string DefMissing = "azDefMis";

        private const string MothConnected = "azMothCon";
        private const string MothDisconnected = "azMothDisCon";

        private const string ShieldReady = "azShieldRed";
        private const string ShieldMissing = "azShieldMis";


        private DisplayTexts()
        { }

        public static readonly DisplayTexts Main = new DisplayTexts();

        private string _cyclopsPowerLow;

        private string _defenseCooldown;
        private string _defenseCharged;
        private string _defenseMissing;

        private string _mothConnect;
        private string _mothDisconnect;

        private string _shldConnect;
        private string _shldDisconnect;

        public string CyclopsPowerLow => _cyclopsPowerLow ??= Language.main.Get(PowerLow);

        public string DefenseCooldown => _defenseCooldown ??= Language.main.Get(DefCooling);
        public string DefenseCharged => _defenseCharged ??= Language.main.Get(DefCharged);
        public string DefenseMissing => _defenseMissing ??= Language.main.Get(DefMissing);

        public string SeamothConnected => _mothConnect ??= Language.main.Get(MothConnected);
        public string SeamothNotConnected => _mothDisconnect ??= Language.main.Get(MothDisconnected);

        public string ShieldConnected => _shldConnect ??= Language.main.Get(ShieldReady);
        public string ShieldNotConnected => _shldDisconnect ??= Language.main.Get(ShieldMissing);

        public void Patch()
        {
            LanguageHandler.Main.SetLanguageLine(PowerLow, "CYCLOPS\nPOWER\nLOW");

            LanguageHandler.Main.SetLanguageLine(DefCooling, "Defense System\n[Cooldown]");
            LanguageHandler.Main.SetLanguageLine(DefCharged, "Defense System\n[Charged]");
            LanguageHandler.Main.SetLanguageLine(DefMissing, "Defense System\n[Missing]");
            
            LanguageHandler.Main.SetLanguageLine(MothConnected, "Seamoth\n[Connected]");
            LanguageHandler.Main.SetLanguageLine(MothDisconnected, "Seamoth\n[Not Connected]");

            LanguageHandler.Main.SetLanguageLine(ShieldReady, "Shield\n[Connected]");
            LanguageHandler.Main.SetLanguageLine(ShieldMissing, "Shield\n[Not Connected]");
        }
    }
}
