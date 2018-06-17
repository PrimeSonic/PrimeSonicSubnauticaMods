namespace UpgradedVehicles
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;

    internal class SeaMothMk2
    {
        public static TechType TechTypeID { get; private set; }


        private static GameObject GetGameObject()
        {
            GameObject prefab = Resources.Load<GameObject>("WorldEntities/Tools/SeaMoth");
            GameObject obj = UnityEngine.Object.Instantiate(prefab);

            obj.GetComponent<PrefabIdentifier>().ClassId = NameID;
            obj.GetComponent<TechTag>().type = TechTypeID;

            var life = obj.GetComponent<LiveMixin>();
            life.data.maxHealth = 400f; // Double the normal health but still less than the ExoSuit's 600
            life.health = 400f; // Might not be needed, should test

            return obj;
        }
    }
}
