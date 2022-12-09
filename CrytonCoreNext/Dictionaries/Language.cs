using System.Windows;

namespace CrytonCoreNext.Dictionaries
{
    public static class Language
    {
        public static string Post(string key)
        {
            var dictionary = Application.Current.Resources.MergedDictionaries[0];
            return dictionary.Contains(key) ? dictionary[key].ToString() ?? string.Empty : string.Empty;
        }
    }
}
