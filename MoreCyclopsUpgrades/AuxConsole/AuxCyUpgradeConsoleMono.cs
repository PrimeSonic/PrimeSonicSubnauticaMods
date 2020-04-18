namespace MoreCyclopsUpgrades.AuxConsole
{
    using MoreCyclopsUpgrades.API.Buildables;
    using UnityEngine;

    internal class AuxCyUpgradeConsoleMono : AuxiliaryUpgradeConsole
    {
        public override void OnSlotEquipped(string slot, InventoryItem item)
        {
            if (ModuleDisplay1 == null)
                AddModuleSpriteHandlers();

            SetModuleVisibility(slot, true);
        }

        public override void OnSlotUnequipped(string slot, InventoryItem item)
        {
            if (ModuleDisplay1 == null)
                AddModuleSpriteHandlers();

            SetModuleVisibility(slot, false);
        }

        private void AddModuleSpriteHandlers()
        {
            const float topRowY = 1.15f;//-0.109f;
            const float botRowY = 1.075f;//-0.239f;

            const float leftColX = 0.15f;//0.159f;
            const float middColX = 0f;//0f;
            const float rightColX = -0.15f;//-0.152f;

            const float topRowZ = 0.12f;// 1.146f;
            const float botRowZ = 0.270f;//1.06f;

            var rotation = Quaternion.Euler(60f, 180, 0);

            ModuleDisplay1 = IconCreator.CreateModuleDisplay(this, new Vector3(rightColX, botRowY, botRowZ), rotation);
            ModuleDisplay2 = IconCreator.CreateModuleDisplay(this, new Vector3(middColX, botRowY, botRowZ), rotation);
            ModuleDisplay3 = IconCreator.CreateModuleDisplay(this, new Vector3(leftColX, botRowY, botRowZ), rotation);
            ModuleDisplay4 = IconCreator.CreateModuleDisplay(this, new Vector3(rightColX, topRowY, topRowZ), rotation);
            ModuleDisplay5 = IconCreator.CreateModuleDisplay(this, new Vector3(middColX, topRowY, topRowZ), rotation);
            ModuleDisplay6 = IconCreator.CreateModuleDisplay(this, new Vector3(leftColX, topRowY, topRowZ), rotation);
        }

        private void SetModuleVisibility(string slot, bool visible)
        {
            GameObject canvasObject;
            switch (slot)
            {
                case "Module1":
                    canvasObject = ModuleDisplay1;
                    break;
                case "Module2":
                    canvasObject = ModuleDisplay2;
                    break;
                case "Module3":
                    canvasObject = ModuleDisplay3;
                    break;
                case "Module4":
                    canvasObject = ModuleDisplay4;
                    break;
                case "Module5":
                    canvasObject = ModuleDisplay5;
                    break;
                case "Module6":
                    canvasObject = ModuleDisplay6;
                    break;
                default:
                    return;
            }

            if (canvasObject == null)
                return;

            uGUI_Icon icon = canvasObject.GetComponent<uGUI_Icon>();

            if (icon == null)
                return; // Safety exit

            if (visible)
            {
                TechType techType = this.Modules.GetTechTypeInSlot(slot);

                if (techType == TechType.None)
                    return; // Safety exit

                icon.sprite = SpriteManager.Get(techType);
            }
            else
            {
                icon.sprite = null; // Clear the sprite when empty
            }

            canvasObject.SetActive(visible);
            icon.enabled = visible;
        }

        public GameObject ModuleDisplay1;
        public GameObject ModuleDisplay2;
        public GameObject ModuleDisplay3;
        public GameObject ModuleDisplay4;
        public GameObject ModuleDisplay5;
        public GameObject ModuleDisplay6;
    }
}
