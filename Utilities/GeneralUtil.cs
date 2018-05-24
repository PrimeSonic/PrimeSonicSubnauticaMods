namespace Common
{
    using System;
    using UnityEngine;
    using System.Reflection;

    /// <summary>
    /// A collection of commong utility methods useful across many mods.
    /// </summary>
    /// <remarks>
    /// For credits/attributions, see documentation on each method.
    /// </remarks>
    public static class GeneralUtil
    {
        /// <summary>
        /// Creates a new component type instance in a <see cref="GameObject"/> instance and copyies over all specified field values from the original.
        /// </summary>
        /// <typeparam name="T">
        /// The component's class type.
        /// </typeparam>
        /// <param name="preFabTemplate">
        /// The original preFab template instance. Field values from this object will be copied into the GameObject's new instance.
        /// </param>
        /// <param name="target">
        /// The GameObject to receive a new component instance of the prefab template's type.
        /// </param>
        /// <param name="fieldBindings">
        /// The BindingFlags used to find the fields in the new component instance to update with values from the original component prefab.
        /// By default this searched public instance fields.
        /// </param>
        /// <returns>A reference to the new component instance that is now part of the destination <see cref="GameObject"/>.</returns>
        /// <remarks>
        /// Orignal code by https://github.com/RandyKnapp/SubnauticaModSystem
        /// </remarks>        
        public static T CopyComponentInto<T>(this T preFabTemplate, GameObject target, BindingFlags fieldBindings = BindingFlags.Public | BindingFlags.Instance) where T : Component
        {
            // The Type of the template prefab
            Type type = preFabTemplate.GetType();

            // A new component is adde to the target GameObject
            Component copy = target.AddComponent(type);

            // Get the fields to update.
            FieldInfo[] fields = type.GetFields(fieldBindings);

            foreach (FieldInfo field in fields)
            {
                field.SetValue(copy, field.GetValue(preFabTemplate));
            }

            return copy as T;
        }
    }
}
