namespace UpgradedVehicles
{
    using System;
    using System.IO;
    using System.Text;
    using Common;
    using Common.EasyMarkup;

    internal class EmUnlockConfig : EmYesNo
    {
        private const string ConfigFile = "./QMods/UpgradedVehicles/Config.txt";

        internal bool ForceUnlockAtStart
        {
            get
            {
                if (!Initialized)
                    Initialize();

                return this.Value;
            }
        }

        internal bool Initialized { get; private set; } = false;

        public EmUnlockConfig() : base("ForceUnlockAtStart", false)
        {
        }

        private void WriteConfigFile()
        {
            File.WriteAllLines(ConfigFile, new[]
            {
                "# ----------------------------------------------------------------------------- #",
                "# Changes to this config file will only take effect next time you boot the game #",
                "#                 This config file was built using EasyMarkup                   #",
                "# ----------------------------------------------------------------------------- #",
                "",
                this.PrettyPrint(),
                "",
                "# Here's what this configuration does: #",
                "",
                $"# 'Force Unlock At Start' #",
                "# When this is set to 'NO', the new vehicles and items added by this mod will be unlocked normally as you discover the prerequisit blueprints. #",
                "# Set this to 'YES' if you really want to have these new blueprints unlocked at the start of the game. #",
                "# If you installed this mod on a game you had currently in progress and you are having issues unlocking the new vehicles, set this to 'YES' #",
                "# ----------------------------------------------------------------------------- #",
            }, Encoding.UTF8);
        }

        internal void Initialize()
        {
            try
            {
                LoadFromFile();
            }
            catch (Exception ex)
            {
                QuickLogger.Error("EXCEPTION LOADING {ConfigKey}: " + ex.ToString());
                WriteConfigFile();
            }
            finally
            {
                Initialized = true;

                if (this.Value)
                    QuickLogger.Message("ForceUnlockAtStart was enabled. New items will start unlocked.");
                else
                    QuickLogger.Message("New items set to normal unlock requirements.");
            }
        }

        private void LoadFromFile()
        {
            if (!File.Exists(ConfigFile))
            {
                QuickLogger.Message("Mod config file not found. Writing default file.");
                WriteConfigFile();
                return;
            }

            string text = File.ReadAllText(ConfigFile, Encoding.UTF8);

            bool readCorrectly = this.FromString(text);

            if (!readCorrectly || !this.HasValue)
            {
                QuickLogger.Warning("Mod config file contained error. Writing default file.");
                WriteConfigFile();
                return;
            }
        }
    }
}
