using System.Windows;

namespace CrytonCoreNext.Dictionaries
{
    public static class Language
    {
        private static int DictionaryIndex = 3;

        public static string Post(string key)
        {
            var app = Application.Current;
            if (app == null)
            {
                return string.Empty;
            }
            var dictionary = Application.Current.Resources.MergedDictionaries[DictionaryIndex];
            return dictionary.Contains(key) ? dictionary[key].ToString() ?? string.Empty : string.Empty;
        }
    }
}
