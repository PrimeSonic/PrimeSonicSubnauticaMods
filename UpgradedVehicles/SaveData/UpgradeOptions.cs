namespace UpgradedVehicles.SaveData
{
    using Common;
    using SMLHelper.V2.Handlers;
    using SMLHelper.V2.Options;

    internal class UpgradeOptions : ModOptions
    {
        private readonly ConfigSaveData SaveData = new ConfigSaveData();

        internal float SeamothBonusSpeedMultiplier
        {
            get
            {
                switch (SaveData.SeamothBonusSpeedSetting)
                {
                    case BonusSpeedStyles.Disabled:
                        return 0f;
                    case BonusSpeedStyles.Slower:
                        return 0.05f;
                    case BonusSpeedStyles.Normal:
                        return 0.15f;
                    case BonusSpeedStyles.Faster:
                        return 0.25f;
                    default: // Error
                        return 0.20f;
                }
            }
        }

        internal float ExosuitBonusSpeedMultiplier
        {
            get
            {
                switch (SaveData.ExosuitBonusSpeedSetting)
                {
                    case BonusSpeedStyles.Disabled:
                        return 0f;
                    case BonusSpeedStyles.Slower:
                        return 0.10f;
                    case BonusSpeedStyles.Normal:
                        return 0.20f;
                    case BonusSpeedStyles.Faster:
                        return 0.25f;
                    default: // Error
                        return 0.15f;
                }
            }
        }

        public UpgradeOptions() : base("Upgraded Vehicles Settings")
        {
            ChoiceChanged += OnBonusSpeedStyleChanged;
        }

        public void Initialize()
        {
            QuickLogger.Message("Initializing save data and options");
            SaveData.InitializeSaveFile();
            OptionsPanelHandler.RegisterModOptions(this);
        }

        public override void BuildModOptions()
        {
            AddChoiceOption(ConfigSaveData.SeamothBonusSpeedID, "Seamoth Bonus Speed", ConfigSaveData.SpeedSettingLabels, SaveData.SeamothBonusSpeedIndex);
            AddChoiceOption(ConfigSaveData.ExosuitBonusSpeedID, "Prawn Suit Bonus Speed", ConfigSaveData.SpeedSettingLabels, SaveData.ExosuitBonusSpeedIndex);
        }

        private void OnBonusSpeedStyleChanged(object sender, ChoiceChangedEventArgs args)
        {
            switch (args.Id)
            {
                case ConfigSaveData.SeamothBonusSpeedID:
                    SaveData.SeamothBonusSpeedIndex = args.Index;
                    break;

                case ConfigSaveData.ExosuitBonusSpeedID:
                    SaveData.ExosuitBonusSpeedIndex = args.Index;
                    break;

                default:
                    return;
            }

            VehicleUpgrader.SetBonusSpeedMultipliers(this.SeamothBonusSpeedMultiplier, this.ExosuitBonusSpeedMultiplier);
        }
    }
}
