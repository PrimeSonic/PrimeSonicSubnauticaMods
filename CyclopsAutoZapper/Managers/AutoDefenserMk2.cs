namespace CyclopsAutoZapper.Managers
{
    internal class AutoDefenserMk2 : Zapper
    {
        protected override float TimeBetweenUses => 6.0f;

        public AutoDefenserMk2(TechType upgradeTechType, SubRoot cyclops)
            : base(upgradeTechType, cyclops)
        {
        }
    }
}
