using System.Windows;

namespace CrytonCoreNext.Dictionaries
{
    public static class Language
    {
        public static string Post(string key)
        {
            var app = Application.Current;
            if (app == null)
            {
                return string.Empty;
            }
            var resources = Application.Current.Resources;
            if (resources == null)
            {
                return string.Empty;
            }
            var mergedDictionaries = resources.MergedDictionaries;
            if (mergedDictionaries.Count != 3)
            {
                return string.Empty;
            }
            var dictionary = Application.Current.Resources.MergedDictionaries[2];
            return dictionary.Contains(key) ? dictionary[key].ToString() ?? string.Empty : string.Empty;
        }
    }
}
