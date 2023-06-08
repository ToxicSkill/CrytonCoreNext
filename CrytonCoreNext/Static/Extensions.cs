using CrytonCoreNext.Dictionaries;

namespace CrytonCoreNext.Static
{
    public static class Extensions
    {
        public static string Jpg { get => nameof(Jpg).ToLower(); }

        public static string Jpeg { get => nameof(Jpeg).ToLower(); }

        public static string Png { get => nameof(Png).ToLower(); }

        public static string Gif { get => nameof(Gif).ToLower(); }

        public static string Tiff { get => nameof(Tiff).ToLower(); }

        public static string Pdf { get => nameof(Pdf).ToLower(); }

        public static string PdfAndImages { get => nameof(PdfAndImages).ToLower(); }

        public static string Txt { get => nameof(Txt).ToLower(); }

        public static string Json { get => nameof(Json).ToLower(); }

        public static string Jpegs { get => nameof(Jpegs).ToLower(); }

        public static string Bmp { get => nameof(Bmp).ToLower(); }

        public enum DialogFilters
        {
            Json = 0,
            Txt,
            Images,
            Pdf,
            PdfAndImages,
            All
        }

        public static string FilterToPrompt(DialogFilters filter)
        {
            switch (filter)
            {
                case DialogFilters.All:
                    return nameof(DialogFilters.All) + " files (*.*)|*.*";
                case DialogFilters.Images:
                    return nameof(DialogFilters.Images) + " file(s) |*." +
                       Jpg + ";*." +
                       Jpeg + ";*." +
                       Png + ";*." +
                       Tiff + ";*." +
                       Bmp + ";*." +
                       Gif + ";";
                case DialogFilters.Pdf:
                    return nameof(DialogFilters.Pdf) + " file(s) |*." +
                       Pdf + ";";
                case DialogFilters.PdfAndImages:
                    return nameof(DialogFilters.Images) + "and" + nameof(DialogFilters.Pdf) +
                        " file(s) |*." +
                       Pdf + ";" +
                       Jpg + ";*." +
                       Jpeg + ";*." +
                       Png + ";*." +
                       Tiff + ";*." +
                       Bmp + ";*." +
                       Gif + ";";
                case DialogFilters.Json:
                    return nameof(DialogFilters.Json) + " file(s) |*." +
                       Json + ";";
                case DialogFilters.Txt:
                    return nameof(DialogFilters.Txt) + " file(s) |*." +
                       Txt + ";";
                default:
                    return string.Empty;
            }
        }

        public static string ToDescription(this CryptingStatus.Status status)
        {
            return status == CryptingStatus.Status.Encrypted ?
                Language.Post("Encrypt") :
                Language.Post("Decrypt");
        }
    }
}
