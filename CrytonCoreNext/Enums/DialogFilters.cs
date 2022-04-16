namespace CrytonCoreNext.Enums
{
    public static class EDialogFilters
    {
        public enum DialogFilters
        {
            Images = 0,
            Pdf,
            All
        }

        public static string ExtensionToFilter(DialogFilters filter)
        {
            switch (filter)
            {
                case DialogFilters.All:
                    return nameof(DialogFilters.All) + " files (*.*)|*.*";
                case DialogFilters.Images:
                    return nameof(DialogFilters.Images) + " file(s) |*." +
                        Enums.EExtensions.EnumToString(EExtensions.Extensions.jpg) + ";*." +
                        Enums.EExtensions.EnumToString(EExtensions.Extensions.jpeg) + ";*." +
                        Enums.EExtensions.EnumToString(EExtensions.Extensions.png) + ";*." +
                        Enums.EExtensions.EnumToString(EExtensions.Extensions.tiff) + ";*." +
                        Enums.EExtensions.EnumToString(EExtensions.Extensions.gif) + ";";
                case DialogFilters.Pdf:
                    return nameof(DialogFilters.Pdf) + " file(s) |*." +
                        Enums.EExtensions.EnumToString(EExtensions.Extensions.pdf) + ";";
                default:
                    return string.Empty;
            }
        }
    }
}
