namespace MoreCyclopsUpgrades.Managers
{
    using System.Collections.Generic;
    using MoreCyclopsUpgrades.API.Charging;

    internal interface IChargeManager
    {
        ICollection<CyclopsCharger> Chargers { get; }

        int GetTotalReservePower();
        void InitializeChargers();
    }
}