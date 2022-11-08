using System;

namespace CrytonCoreNext.Helpers
{
    public static class GCHelper
    {
        public static void Collect()
        {
            GC.Collect();
        }
    }
}
