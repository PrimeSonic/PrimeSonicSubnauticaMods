namespace IonCubeGenerator.Mono
{
    using IonCubeGenerator.Buildable;

    internal partial class CubeGeneratorMono : HandTarget, IHandTarget
    {
        // TODO - Will we need this?
        private bool pdaIsOpen = false;

        public void OnHandClick(GUIHand hand)
        {

            if (!_buildable.constructed)
                return;

            Player main = Player.main;
            PDA pda = main.GetPDA();
            Inventory.main.SetUsedStorage(_cubeContainer, false);
            pda.Open(PDATab.Inventory, null, new PDA.OnClose(IonGenOnPdaClose), 4f);

            pdaIsOpen = true;
        }

        public void OnHandHover(GUIHand hand)
        {
            if (!_buildable.constructed)
                return;

            HandReticle main = HandReticle.main;

            string text = CubeGeneratorBuildable.OnHoverText();

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
