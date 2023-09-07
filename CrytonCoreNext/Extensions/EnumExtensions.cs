using CrytonCoreNext.Enums;

namespace CrytonCoreNext.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Rceturn enum as string formated in specific way
        /// </summary>
        /// <param name="metainfo"></param>
        /// <param name="pdf"></param>
        /// <returns>If <paramref name="pdf"/> is true returns metadata key. If <paramref name="pdf"/> is false returns sentence string</returns>
        public static string ToPdfInformationString(this string metainfo, bool pdf)
        {
            return pdf ? $"PDF.{metainfo}" : metainfo.ToString().ToSentenceCase();            
        }

        public static EDirection Opposite(this EDirection direction)
        {
            if (direction == EDirection.Left)
            {
                return EDirection.Right;
            }
            else if (direction == EDirection.Right)
            {
                return EDirection.Left;
            }
            return direction;
        }
    }
}
