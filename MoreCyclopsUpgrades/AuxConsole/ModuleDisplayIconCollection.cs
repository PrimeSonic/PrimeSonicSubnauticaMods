namespace MoreCyclopsUpgrades.AuxConsole
{
    using System.Collections.Generic;
    using UnityEngine;

    internal class ModuleDisplayIconCollection : Dictionary<UpgradeConsole, ModuleIconDisplay>
    {
        public static readonly ModuleDisplayIconCollection Registered = new ModuleDisplayIconCollection();

        public static void Register(UpgradeConsole upgradeConsole, Canvas canvas1, Canvas canvas2, Canvas canvas3,
                                  Canvas canvas4, Canvas canvas5, Canvas canvas6)
        {
            Registered.Add(upgradeConsole, new ModuleIconDisplay(canvas1, canvas2, canvas3, canvas4, canvas5, canvas6));
        }

        public static bool IsRegistered(UpgradeConsole upgradeConsole)
        {
            return Registered.ContainsKey(upgradeConsole);
        }

        public static bool TryGetRegistered(UpgradeConsole upgradeConsole, out ModuleIconDisplay consoleIcons)
        {
            return Registered.TryGetValue(upgradeConsole, out consoleIcons);
        }

        public static GameObject GetModulePlug(UpgradeConsole upgradeConsole, string slot)
        {
            switch (slot)
            {
                case "Module1":
                    return upgradeConsole.module1;
                case "Module2":
                    return upgradeConsole.module2;
                case "Module3":
                    return upgradeConsole.module3;
                case "Module4":
                    return upgradeConsole.module4;
                case "Module5":
                    return upgradeConsole.module5;
                case "Module6":
                    return upgradeConsole.module6;
                default:
                    return null;
            }
        }
    }
}
