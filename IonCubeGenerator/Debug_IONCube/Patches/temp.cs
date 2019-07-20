#if DEBUG
using UnityEngine;

namespace IonCubeGenerator.Debug_IONCube.Patches
{
    public static class temp
    {
        public static GameObject Prefab { get; set; }
        public static Animator Animator { get; set; }
        public static AnimatorStateInfo AnimatiorState { get; set; }
        public static GameObject Slot { get; set; }
    }
}
#endif