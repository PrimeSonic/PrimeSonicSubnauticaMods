namespace IonCubeGenerator.Mono
{
    using IonCubeGenerator.Buildable;

    internal partial class CubeGeneratorMono : HandTarget, IHandTarget
    {
        public void OnHandClick(GUIHand hand)
        {
            if (!this.IsConstructed)
                return;

            Player main = Player.main;
            PDA pda = main.GetPDA();
            Inventory.main.SetUsedStorage(_cubeContainer, false);
            pda.Open(PDATab.Inventory, null, null, 4f);
        }

        public void OnHandHover(GUIHand hand)
        {
            if (!this.IsConstructed)
                return;

            string text;
            if (CurrentCubeCount == MaxAvailableSpaces)
            {
                text = CubeGeneratorBuildable.OnHoverTextFull();
            }
            else if (GameModeUtils.RequiresPower() && this.AvailablePower < EnergyConsumptionPerSecond)
            {
                text = CubeGeneratorBuildable.OnHoverTextNoPower();
            }
            else if (isGenerating && timeToNextCube > 0f)
            {
                text = CubeGeneratorBuildable.OnHoverTextProgress(this.NextCubePercentage);
            }
            else
            {
                text = CubeGeneratorBuildable.BuildableName;
            }

            HandReticle main = HandReticle.main;
            main.SetInteractText(text);
            main.SetIcon(HandReticle.IconType.Hand, 1f);
        }
    }
}
