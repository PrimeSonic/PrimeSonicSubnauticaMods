namespace CustomBatteries.Items
{
    using UnityEngine;

    internal class WorldEntitiesCache
    {
        private GameObject _precursorionbattery;
        private GameObject _precursorionpowercell;
        private GameObject _battery;
        private GameObject _powercell;

        public GameObject IonBattery()
        {
            return _precursorionbattery ??= Resources.Load<GameObject>("worldentities/tools/precursorionbattery");
        }

        public GameObject IonPowerCell()
        {
            return _precursorionpowercell ??= Resources.Load<GameObject>("worldentities/tools/precursorionpowercell");
        }

        public GameObject Battery()
        {
            return _battery ??= Resources.Load<GameObject>("worldentities/tools/battery");
        }

        public GameObject PowerCell()
        {
            return _powercell ??= Resources.Load<GameObject>("worldentities/tools/powercell");
        }
    }
}
