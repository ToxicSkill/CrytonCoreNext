using CrytonCoreNext.Enums;
using System.Collections.Generic;
using System.Linq;

namespace CrytonCoreNext.Helpers
{
    public static class EStrengthExtension
    {
        private static int _score;

        public static int GetStrengthScore(this string password)
        {
            _score = 0;

            password
                .SetStrengthScoreLowerCase()
                .SetStrengthScoreDecimalDigitNumber()
                .SetStrengthScoreUpperCase()
                .SetStrengthScorePunctuation()
                .SetStrengthScoreSymbol()
                .SetStrengthScoreSeparator()
                .SetStrengthScoreSpecialChar();

            return _score;
        }

        private static string SetStrengthScoreDecimalDigitNumber(this string password)
        {
            var distinctChars = password.Distinct().Where(character => char.IsDigit(character)).ToList().MoreThanOneAndNotInOrder();

            if (password.Any(character => char.IsDigit(character)))
                _score += distinctChars ? (int)EStrengthScore.DecimalDigitNumber * 2 : (int)EStrengthScore.DecimalDigitNumber;

            return password;
        }

        private static string SetStrengthScoreLowerCase(this string password)
        {
            var distinctChars = password.Distinct().Where(character => char.IsLower(character)).ToList().MoreThanOneAndNotInOrder();

            if (password.Any(c => char.IsLower(c)))
                _score += distinctChars ? (int)EStrengthScore.LowerCase * 2 : (int)EStrengthScore.LowerCase;

            return password;
        }

        private static string SetStrengthScoreUpperCase(this string password)
        {
            var distinctChars = password.Distinct().Where(character => char.IsUpper(character)).ToList().MoreThanOneAndNotInOrder();

            if (password.Any(character => char.IsUpper(character)))
                _score += distinctChars ? (int)EStrengthScore.UpperCase * 2 : (int)EStrengthScore.UpperCase;

            return password;
        }

        private static string SetStrengthScorePunctuation(this string password)
        {
            var distinctChars = password.Distinct().Where(character => char.IsPunctuation(character)).ToList().MoreThanOneAndNotInOrder();

            if (password.Any(character => char.IsPunctuation(character)))
                _score += distinctChars ? (int)EStrengthScore.Punctuation * 2 : (int)EStrengthScore.Punctuation;

            return password;
        }

        private static string SetStrengthScoreSymbol(this string password)
        {
            var distinctChars = password.Distinct().Where(character => char.IsSymbol(character)).ToList().MoreThanOneAndNotInOrder();

            if (password.Any(character => char.IsSymbol(character)))
                _score += distinctChars ? (int)EStrengthScore.Symbol * 2 : (int)EStrengthScore.Symbol;

            return password;
        }

        private static string SetStrengthScoreSeparator(this string password)
        {
            var distinctChars = password.Distinct().Where(character => char.IsSeparator(character)).ToList().MoreThanOneAndNotInOrder();

            if (password.Any(character => char.IsSeparator(character)))
                _score += distinctChars ? (int)EStrengthScore.Separator * 2 : (int)EStrengthScore.Separator;

            return password;
        }

        private static string SetStrengthScoreSpecialChar(this string password)
        {
            var distinctChars = password.Distinct().Where(character => character < ' ' || character > '~').ToList().MoreThanOneAndNotInOrder();

            if (password.Any(character => character < ' ' || character > '~'))
                _score += distinctChars ? (int)EStrengthScore.SpecialChar * 2 : (int)EStrengthScore.SpecialChar;

            return password;
        }

        private static bool MoreThanOneAndNotInOrder(this List<char> distincts)
        {
            var distinctCounter = 1;

            for (var item = 1; item < distincts.Count(); item++)
            {
                if (distincts[item] == distincts[item - 1] + 1)
                    distinctCounter++;
            }

            return distincts.Count() > distinctCounter;
        }
    }
}