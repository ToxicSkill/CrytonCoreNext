using System.Windows;

namespace CrytonCoreNext.Dictionaries
{
    public static class Language
    {
        public static string Post(string key)
        {
            if (Application.Current == null)
            {
                return string.Empty;
            }
            return Application.Current.Resources.MergedDictionaries.Count > 0 ?
                Application.Current.Resources.MergedDictionaries[0][key].ToString() ?? string.Empty :
                string.Empty;
        }
    }
}
