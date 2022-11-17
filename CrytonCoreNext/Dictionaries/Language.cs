using System.Windows;

namespace CrytonCoreNext.Dictionaries
{
    public static class Language
    {
        public static string Post(string key)
        {
            return Application.Current.Resources.MergedDictionaries[0][key].ToString() ?? string.Empty;
        }
    }
}
