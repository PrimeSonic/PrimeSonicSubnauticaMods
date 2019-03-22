namespace MoreCyclopsUpgrades.Managers
{
    using MoreCyclopsUpgrades.Modules;
    using System.Collections.Generic;

    internal class PowerIconState
    {
        internal struct PowerIcon
        {
            public readonly TechType TechType;
            public float Value;
            public float MaxValue;
            public readonly float MinValue;
            public readonly NumberFormat Format;

            public PowerIcon(TechType techType, float minValue, NumberFormat format) : this()
            {
                TechType = techType;
                Format = format;
                MinValue = minValue;
            }

            public PowerIcon(TechType techType, NumberFormat format) : this(techType, 0f, format)
            {
                TechType = techType;
                Format = format;
            }
        }

        internal bool EvenCount { get; private set; } = true;

        private PowerIcon nuclearIcon = new PowerIcon(CyclopsModule.NuclearChargerID, NumberFormat.Amount) { MaxValue = 6000f };
        private PowerIcon bioIcon = new PowerIcon(CyclopsModule.BioReactorBoosterID, NumberFormat.Amount) { MaxValue = 200f };
        private PowerIcon solarIcon = new PowerIcon(CyclopsModule.SolarChargerID, NumberFormat.Sun) { MaxValue = 90f };
        private PowerIcon solar2Icon = new PowerIcon(CyclopsModule.SolarChargerMk2ID, NumberFormat.Amount) { MaxValue = 100f };
        private PowerIcon thermalIcon = new PowerIcon(TechType.CyclopsThermalReactorModule, 25f, NumberFormat.Temperature) { MaxValue = 100f };
        private PowerIcon thermal2Icon = new PowerIcon(CyclopsModule.ThermalChargerMk2ID, NumberFormat.Amount) { MaxValue = 100f };

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

        internal float SolarBatteryCapacity
        {
            get => solar2Icon.MaxValue;
            set => solar2Icon.MaxValue = value;
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

        internal float ThermalBatteryCapacity
        {
            get => thermal2Icon.MaxValue;
            set => thermal2Icon.MaxValue = value;
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

        internal float BioCapacity
        {
            get => bioIcon.MaxValue;
            set => bioIcon.MaxValue = value;
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

        internal float NuclearCapacity
        {
            get => nuclearIcon.MaxValue;
            set => nuclearIcon.MaxValue = value;
        }

        private bool UpdateIconCount(bool newValue, bool originalValue)
        {
            if (newValue != originalValue)
                this.EvenCount = !this.EvenCount;

            return newValue;
        }
    }
}
