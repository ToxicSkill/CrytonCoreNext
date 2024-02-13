using System.Windows;

namespace CrytonCoreNext.Dictionaries
{
    public static class Language
    {
        private static readonly int DictionaryIndex = 3;

        public static string Post(string key)
        {
            var app = Application.Current;
            if (app == null)
            {
                return string.Empty;
            }
            if (Application.Current.Resources.MergedDictionaries.Count < DictionaryIndex)
            {
                var dictionary = Application.Current.Resources.MergedDictionaries[DictionaryIndex];
                return dictionary.Contains(key) ? dictionary[key].ToString() ?? string.Empty : string.Empty;
            }
            return key;
        }
    }
}
