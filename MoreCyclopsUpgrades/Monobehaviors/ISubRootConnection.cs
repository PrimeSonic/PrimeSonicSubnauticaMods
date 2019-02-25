namespace MoreCyclopsUpgrades.Monobehaviors
{
    interface ISubRootConnection
    {
        void ConnectToCyclops(SubRoot parentCyclops);

        SubRoot ParentCyclops { get; }
    }
}
