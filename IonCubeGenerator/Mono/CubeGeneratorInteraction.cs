namespace IonCubeGenerator.Mono
{
    using Common;
    using UnityEngine;

    internal partial class CubeGeneratorMono : MonoBehaviour
    {
        private void ChangeStorageState()
        {
            QuickLogger.Debug($"Storage Button Clicked", true);

            if (!this.IsConstructed)
                return;

            Player main = Player.main;
            PDA pda = main.GetPDA();
            Inventory.main.SetUsedStorage(_cubeContainer, false);
            pda.Open(PDATab.Inventory, null, null, 4f);
        }
    }
}
