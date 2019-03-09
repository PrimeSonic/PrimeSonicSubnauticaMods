namespace MoreCyclopsUpgrades.Managers
{
    using System.Collections.Generic;
    using MoreCyclopsUpgrades.Modules;

    internal class PowerIconState
    {
        internal bool EvenCount { get; private set; } = true;

        private bool solar = false;
        private bool solarBattery = false;
        private bool thermal = false;
        private bool thermalBattery = false;
        private bool bio = false;
        private bool nuclear = false;

        internal IEnumerable<TechType> ActiveIcons
        {
            get
            {
                if (nuclear) yield return CyclopsModule.NuclearChargerID;
                if (bio) yield return TechType.BaseBioReactor; // placeholder

                if (solar) yield return CyclopsModule.SolarChargerID;
                if (solarBattery) yield return CyclopsModule.SolarChargerMk2ID;

                if (thermal) yield return TechType.CyclopsThermalReactorModule;
                if (thermalBattery) yield return CyclopsModule.ThermalChargerMk2ID;
            }
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

        internal bool Bio
        {
            get => bio;
            set => bio = UpdateIconCount(value, bio);
        }

        internal bool Nuclear
        {
            get => nuclear;
            set => nuclear = UpdateIconCount(value, nuclear);
        }

        private bool UpdateIconCount(bool newValue, bool originalValue)
        {
            if (newValue != originalValue)
                this.EvenCount = !this.EvenCount;

            return newValue;
        }
    }
}
