using SMLHelper.V2.Options;

namespace IonCubeGenerator.Configuration
{
    internal class MenuConfiguration : ModOptions
    {
        //private ModModes _modMode;
        private const string EnableAudioID = "IONEnableAudio";
        //private const string AllowFoodToggle = "DSSAllowFood";


        internal MenuConfiguration() : base("ION Cube Generator Settings")
        {
            this.ToggleChanged += OnToggleChanged;
        }

        private void OnToggleChanged(object sender, ToggleChangedEventArgs e)
        {
            switch (e.Id)
            {
                case EnableAudioID:
                    ModConfiguration.Singleton.AllowSFX = e.Value;
                    break;
            }

            ModConfiguration.Singleton.SaveModConfiguration();
        }
        
        public override void BuildModOptions()
        {
            AddToggleOption(EnableAudioID, "Enable SFX", ModConfiguration.Singleton.AllowSFX);
        }
    }
}