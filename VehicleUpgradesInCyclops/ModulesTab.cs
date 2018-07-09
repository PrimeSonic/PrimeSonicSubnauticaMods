namespace VehicleUpgradesInCyclops
{
    using System.Collections.Generic;

    internal class ModulesTab
    {
        internal readonly string TabID;
        internal readonly string TabName;
        private readonly string SpriteCategoryName;

        public ModulesTab(string tabID, string tabName, string spriteCategoryName, IList<TechType> craftNodes)
        {
            TabID = tabID;
            TabName = tabName;
            SpriteCategoryName = spriteCategoryName;
            CraftNodes = craftNodes;
        }

        internal Atlas.Sprite TabSprite => SpriteManager.Get(SpriteManager.Group.Category, SpriteCategoryName);

        internal IList<TechType> CraftNodes { get; private set; }

    }
}
