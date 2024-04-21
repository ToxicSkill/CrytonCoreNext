using System;

namespace CrytonCoreNext.Helpers
{
    public static class AIImageValueHelper
    {
        public static double ConvertForRange(this double value, double min,
                                        double max, double newMin,
                                        double newMax)
        {
            var minMemoy = min;
            var minNewMemoy = newMin;
            if (minMemoy < 0)
            {
                minMemoy = Math.Abs(minMemoy);
            }
            if (minNewMemoy < 0)
            {
                minNewMemoy = Math.Abs(minNewMemoy);
            }
            var scale = (value + minMemoy) / (minMemoy + max);
            return (scale * (minNewMemoy + newMax)) - minNewMemoy;
        }
    }
}
