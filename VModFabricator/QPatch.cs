using System;

namespace VModFabricator
{
    public class QPatch
    {
        public static void Patch()
        {
#if DEBUG
            try
            {
                Console.WriteLine($"[VModFabricator] Start patching");
#endif
                VModFabricatorModule.Patch();

#if DEBUG
                Console.WriteLine($"[VModFabricator] Finish patching");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[VModFabricator] EXCEPTION on Patch: " + ex.ToString());
            }
#endif
        }
    }
}
