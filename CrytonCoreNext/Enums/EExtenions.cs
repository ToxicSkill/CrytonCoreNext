using System;

namespace CrytonCoreNext.Enums
{
    public static class EExtensions
    {
        public enum Extensions : int
        {
            jpg = 0,
            jpeg,
            png,
            gif,
            tiff,
            pdf,
        }

        public static string EnumToString(Extensions extensions)
        {
            return extensions switch
            {
                Extensions.jpg => nameof(Extensions.jpg),
                Extensions.jpeg => nameof(Extensions.jpeg),
                Extensions.png => nameof(Extensions.png),
                Extensions.gif => nameof(Extensions.gif),
                Extensions.tiff => nameof(Extensions.tiff),
                Extensions.pdf => nameof(Extensions.pdf),
                _ => throw new ArgumentOutOfRangeException(nameof(extensions), extensions, null),
            };
        }
    }
}
