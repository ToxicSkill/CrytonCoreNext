using System.Linq;
using System.Text;

namespace CrytonCoreNext.Parsers
{
    public class FileContentParser
    {
        public static char[] GetCharactersFromBytes(byte [] bytes, int take=1000) => Encoding.Unicode.GetChars((byte[])bytes.Take(take));

        public static byte[] GetBytesFromCharacters(char[] characters, int take = 1000) => Encoding.Unicode.GetBytes((char[])characters.Take(take));

        public static string GetStringFromBytes(byte[] bytes, int take = 1000) => Encoding.UTF8.GetString(bytes);
    }
}
