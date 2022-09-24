﻿namespace UpgradedVehicles.SaveData
{
    using Common;
    using SMLHelper.V2.Handlers;
    using SMLHelper.V2.Options;

    internal class UpgradeOptions : ModOptions, IUpgradeOptions
    {
        private readonly ConfigSaveData SaveData = new ConfigSaveData();

        public bool DebugLogsEnabled => SaveData.DebugLogsEnabled;

        public float SeamothBonusSpeedMultiplier
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
                        return 0.125f;
                }
            }
        }

#if BELOWZERO
        public float SeatruckBonusSpeedMultiplier
        {
            get
            {
                switch (SaveData.SeaTruckBonusSpeedSetting)
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
                        return 0.125f;
                }
            }
        }
#endif

        public float ExosuitBonusSpeedMultiplier
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
                        return 0.30f;
                    default: // Error
                        return 0.15f;
                }
            }
        }

        internal int SeamothBonusSpeedIndex
        {
            get => (int)SaveData.SeamothBonusSpeedSetting;
            set => SaveData.SeamothBonusSpeedSetting = (BonusSpeedStyles)value;
        }
#if BELOWZERO
        internal int SeaTruckBonusSpeedIndex
        {
            get => (int)SaveData.SeaTruckBonusSpeedSetting;
            set => SaveData.SeaTruckBonusSpeedSetting = (BonusSpeedStyles)value;
        }
#endif

        internal int ExosuitBonusSpeedIndex
        {
            get => (int)SaveData.ExosuitBonusSpeedSetting;
            set => SaveData.ExosuitBonusSpeedSetting = (BonusSpeedStyles)value;
        }

        public UpgradeOptions() : base("Upgraded Vehicles Options")
        {
            ChoiceChanged += OnBonusSpeedStyleChanged;
        }

        public void Initialize()
        {
            QuickLogger.Info("Initializing save data and options");
            SaveData.InitializeSaveFile();
            OptionsPanelHandler.RegisterModOptions(this);
        }

        public override void BuildModOptions()
        {
            AddChoiceOption(ConfigSaveData.SeamothBonusSpeedID, "Seamoth Bonus Speed", ConfigSaveData.SpeedSettingLabels, this.SeamothBonusSpeedIndex);
#if BELOWZERO
            AddChoiceOption(ConfigSaveData.SeamothBonusSpeedID, "SeaTruck Bonus Speed", ConfigSaveData.SpeedSettingLabels, this.SeaTruckBonusSpeedIndex);
#endif
            AddChoiceOption(ConfigSaveData.ExosuitBonusSpeedID, "Prawn Suit Bonus Speed", ConfigSaveData.SpeedSettingLabels, this.ExosuitBonusSpeedIndex);
        }

        private void OnBonusSpeedStyleChanged(object sender, ChoiceChangedEventArgs args)
        {
            switch (args.Id)
            {
                case ConfigSaveData.SeamothBonusSpeedID:
                    this.SeamothBonusSpeedIndex = args.Index;
                    break;

#if BELOWZERO
                case ConfigSaveData.SeaTruckBonusSpeedID:
                    this.SeaTruckBonusSpeedIndex = args.Index;
                    break;
#endif

                case ConfigSaveData.ExosuitBonusSpeedID:
                    this.ExosuitBonusSpeedIndex = args.Index;
                    break;

                default:
                    return;
            }

            VehicleUpgrader.SetBonusSpeedMultipliers(this);
            SaveData.Save();
        }
    }
}
