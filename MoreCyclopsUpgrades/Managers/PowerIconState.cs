namespace MoreCyclopsUpgrades.Managers
{
    using MoreCyclopsUpgrades.Modules;
    using System.Collections.Generic;

    internal class PowerIconState
    {
        internal struct PowerIcon
        {
            public TechType TechType;
            public float Value;

            public PowerIcon(TechType upgrade) : this()
            {
                TechType = upgrade;
            }
        }

        internal bool EvenCount { get; private set; } = true;

        private PowerIcon nuclearIcon = new PowerIcon(CyclopsModule.NuclearChargerID);
        private PowerIcon bioIcon = new PowerIcon(CyclopsModule.BioReactorBoosterID);
        private PowerIcon solarIcon = new PowerIcon(CyclopsModule.SolarChargerID);
        private PowerIcon solar2Icon = new PowerIcon(CyclopsModule.SolarChargerMk2ID);
        private PowerIcon thermalIcon = new PowerIcon(TechType.CyclopsThermalReactorModule);
        private PowerIcon thermal2Icon = new PowerIcon(CyclopsModule.ThermalChargerMk2ID);


        private bool solar = false;
        private bool solarBattery = false;
        private bool thermal = false;
        private bool thermalBattery = false;
        private bool bio = false;
        private bool nuclear = false;

        internal IEnumerable<PowerIcon> ActiveIcons
        {
            get
            {
                if (nuclear)
                    yield return nuclearIcon;

                if (bio)
                    yield return bioIcon;

                if (solar)
                    yield return solarIcon;

                if (solarBattery)
                    yield return solar2Icon;

                if (thermal)
                    yield return thermalIcon;

                if (thermalBattery)
                    yield return thermal2Icon;
            }
        }


        internal float SolarStatus
        {
            get => solarIcon.Value;
            set => solarIcon.Value = value;
        }

        internal bool Solar
        {
            get => solar;
            set => solar = UpdateIconCount(value, solar);
        }

        internal bool SolarBattery
        {
            get => solarBattery;
            set => solarBattery = UpdateIconCount(value, solarBattery);
        }

        internal float SolarBatteryCharge
        {
            get => solar2Icon.Value;
            set => solar2Icon.Value = value;
        }

        internal float ThermalStatus
        {
            get => thermalIcon.Value;
            set => thermalIcon.Value = value;
        }

        internal bool Thermal
        {
            get => thermal;
            set => thermal = UpdateIconCount(value, thermal);
        }

        internal bool ThermalBattery
        {
            get => thermalBattery;
            set => thermalBattery = UpdateIconCount(value, thermalBattery);
        }

        internal float ThermalBatteryCharge
        {
            get => thermal2Icon.Value;
            set => thermal2Icon.Value = value;
        }

        internal bool Bio
        {
            get => bio;
            set => bio = UpdateIconCount(value, bio);
        }

        internal float BioCharge
        {
            get => bioIcon.Value;
            set => bioIcon.Value = value;
        }

        internal bool Nuclear
        {
            get => nuclear;
            set => nuclear = UpdateIconCount(value, nuclear);
        }

        internal float NuclearCharge
        {
            get => nuclearIcon.Value;
            set => nuclearIcon.Value = value;
        }

        private bool UpdateIconCount(bool newValue, bool originalValue)
        {
            if (newValue != originalValue)
                this.EvenCount = !this.EvenCount;

            return newValue;
        }
    }
}
