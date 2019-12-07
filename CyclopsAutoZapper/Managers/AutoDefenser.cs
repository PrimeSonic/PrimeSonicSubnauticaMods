namespace CyclopsAutoZapper.Managers
{
    internal class AutoDefenser : Zapper
    {
        protected override float TimeBetweenUses => 7.0f;

        private VehicleDockingBay dockingBay;
        private VehicleDockingBay DockingBay => dockingBay ?? (dockingBay = Cyclops.GetComponentInChildren<VehicleDockingBay>());

        private SeaMoth seaMoth;

        private SeaMoth DockedSeamoth => this.DockingBay?.dockedVehicle as SeaMoth;

        public bool SeamothInBay
        {
            get
            {
                seaMoth = this.DockedSeamoth;
                return seaMoth != null;
            }
        }

        public bool HasSeamothWithElectricalDefense
        {
            get
            {
                Equipment modules = this.DockedSeamoth?.modules;

                if (modules == null)
                    return false;

                return modules.GetCount(TechType.SeamothElectricalDefense) > 0;
            }
        }

        public AutoDefenser(TechType autoDefense, SubRoot cyclops)
            : base(autoDefense, cyclops)
        {
        }

        protected override bool AbleToZap()
        {
            if (!base.AbleToZap())
                return false;

            if (!this.SeamothInBay)
                return false;

            if (!this.HasSeamothWithElectricalDefense)
                return false;

            return true;
        }
    }
}
