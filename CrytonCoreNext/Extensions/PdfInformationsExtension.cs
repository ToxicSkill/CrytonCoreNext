using PdfiumViewer;
using System.Linq;

namespace CrytonCoreNext.Extensions
{
    public static class PdfInformationsExtension
    {
        public static object GetPropertyValue(this PdfInformation? obj, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return "";
            }
            return obj.GetType().GetProperties()
               .Single(pi => pi.Name == propertyName)
               .GetValue(obj, null);
        }
    }
}
