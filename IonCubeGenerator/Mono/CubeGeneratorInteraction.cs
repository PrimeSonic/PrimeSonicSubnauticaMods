namespace IonCubeGenerator.Mono
{
    using IonCubeGenerator.Buildable;
    using UnityEngine;

    internal partial class CubeGeneratorMono : HandTarget, IHandTarget
    {
        // TODO - Will we need this?
        private bool pdaIsOpen = false;

        public void OnHandClick(GUIHand hand)
        {
            if (!this.IsConstructed)
                return;

            Player main = Player.main;
            PDA pda = main.GetPDA();
            Inventory.main.SetUsedStorage(_cubeContainer, false);
            pda.Open(PDATab.Inventory, null, new PDA.OnClose(IonGenOnPdaClose), 4f);

            pdaIsOpen = true;
        }

        public void OnHandHover(GUIHand hand)
        {
            if (!this.IsConstructed)
                return;

            string text;
            if (_cubeContainer.count == MaxAvailableSpaces)
            {
                text = CubeGeneratorBuildable.OnHoverTextFull();
            }
            else if (GameModeUtils.RequiresPower() && this.AvailablePower < EnergyConsumptionPerSecond)
            {
                text = CubeGeneratorBuildable.OnHoverTextNoPower();
            }
            else if (isGenerating && timeToNextCube > 0f)
            {
                int percent = Mathf.RoundToInt(NextCubePercentage);
                text = CubeGeneratorBuildable.OnHoverTextProgress(percent);
            }
            else
            {
                text = CubeGeneratorBuildable.BuildableName;
            }

            HandReticle main = HandReticle.main;
            main.SetInteractText(text);
            main.SetIcon(HandReticle.IconType.Hand, 1f);
        }

        internal void IonGenOnPdaClose(PDA pda)
        {
            // TODO - Will we need this?
            pdaIsOpen = false;
        }
    }
}
