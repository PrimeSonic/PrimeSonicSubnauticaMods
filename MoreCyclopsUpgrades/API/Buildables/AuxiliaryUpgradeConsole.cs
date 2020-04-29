namespace MoreCyclopsUpgrades.API.Buildables
{
    using System.Collections.Generic;
    using MoreCyclopsUpgrades.AuxConsole;
    using MoreCyclopsUpgrades.Managers;

    // This partial class file contains all members of AuxiliaryUpgradeConsole intended for public/external use

    /// <summary>
    /// The core functionality of an Auxiliary Upgrade Console.<para/>
    /// Handles basic player interaction, save data, and connecting with the Cyclops sub.
    /// </summary>
    /// <seealso cref="HandTarget" />
    /// <seealso cref="IHandTarget" />
    /// <seealso cref="IProtoEventListener" />
    /// <seealso cref="ICyclopsBuildable" /> 
    public abstract partial class AuxiliaryUpgradeConsole : HandTarget, IHandTarget, ICyclopsBuildable, IUpgradeSlots
    {
        /// <summary>
        /// The total number of upgrade slots. This value is constant.
        /// </summary>
        public const int TotalSlots = 6;

        /// <summary>
        /// A read-only collection of the upgrade slot names. These will be used to up upgrade slots in <see cref="Modules"/>.
        /// </summary>
        public static readonly IEnumerable<string> SlotNames = new string[TotalSlots]
        {
            "Module1",
            "Module2",
            "Module3",
            "Module4",
            "Module5",
            "Module6"
        };

        /// <summary>
        /// Gets the text to display when the player's cursor hovers over this upgrade console.<para/>
        /// By default, this will display the same text as the original AuxUpgradeConsole.
        /// </summary>
        /// <value>
        /// The on hover text to display.
        /// </value>
        protected virtual string OnHoverText => AuxCyUpgradeConsole.OnHoverText;

        /// <summary>
        /// Invoked after <see cref="OnEquip(string, InventoryItem)"/> has finished handling the added item.
        /// </summary>
        public virtual void OnSlotEquipped(string slot, InventoryItem item) { }

        /// <summary>
        /// Invoked after <see cref="OnUnequip(string, InventoryItem)"/> has finished handling the removed item.
        /// </summary>
        public virtual void OnSlotUnequipped(string slot, InventoryItem item) { }

        /// <summary>
        /// The root object container for the <see cref="Equipment"/> modules.
        /// </summary>
        public ChildObjectIdentifier ModulesRoot;

        /// <summary>
        /// Gets the equipment modules.
        /// </summary>
        public Equipment Modules { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this buildable is connected to the Cyclops.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this buildable is connected to cyclops; otherwise, <c>false</c>.
        /// </value>
        /// <see cref="ICyclopsBuildable"/>
        /// <seealso cref="BuildableManager{BuildableMono}.ConnectWithManager(BuildableMono)" />
        public bool IsConnectedToCyclops => ParentCyclops != null && UpgradeManager != null;

        /// <summary>
        /// Gets the upgrade slots for this upgrade console.
        /// </summary>
        /// <remarks>Value not initialized until after <see cref="Awake"/> is run.</remarks>
        public IEnumerable<UpgradeSlot> UpgradeSlots => UpgradeSlotArray;

        /// <summary>
        /// Called when the player hovers over the upgrade console.
        /// </summary>
        /// <param name="guiHand">The GUI hand.</param>
        /// <see cref="IHandTarget"/>
        public virtual void OnHandHover(GUIHand guiHand)
        {
            if (!this.Buildable.constructed)
                return;

            HandReticle main = HandReticle.main;
            main.SetInteractText(this.OnHoverText);
            main.SetIcon(HandReticle.IconType.Hand, 1f);
        }

        /// <summary>
        /// Called when the player clicks the upgrade console.
        /// </summary>
        /// <param name="guiHand">The GUI hand.</param>
        /// <see cref="IHandTarget"/>
        public virtual void OnHandClick(GUIHand guiHand)
        {
            OpenEquipmentScreen();
        }

        /// <summary>
        /// Opens the player's PDA and displays the equipment module slots.<para/>
        /// Upgrade Console must be fully constructed to access the equipment slots.
        /// </summary>
        protected void OpenEquipmentScreen()
        {
            if (!this.Buildable.constructed)
                return;

            PdaOverlayManager.StartConnectingToPda(this.Modules);

            Player main = Player.main;
            global::PDA pda = main.GetPDA();
            Inventory.main.SetUsedStorage(this.Modules, false);
            pda.Open(PDATab.Inventory, null, new global::PDA.OnClose((closingPdaEvent) => PdaOverlayManager.DisconnectFromPda()), -1f);
        }
    }
}
