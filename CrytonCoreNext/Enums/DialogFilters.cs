namespace CrytonCoreNext.Enums
{
    public static class EDialogFilters
    {
        public enum DialogFilters
        {
            Json = 0,
            Txt,
            Images,
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
                case DialogFilters.Json:
                    return nameof(DialogFilters.Json) + " file(s) |*." +
                        Enums.EExtensions.EnumToString(EExtensions.Extensions.json) + ";";
                case DialogFilters.Txt:
                    return nameof(DialogFilters.Txt) + " file(s) |*." +
                        Enums.EExtensions.EnumToString(EExtensions.Extensions.txt) + ";";
                default:
                    return string.Empty;
            }
        }
    }
}
