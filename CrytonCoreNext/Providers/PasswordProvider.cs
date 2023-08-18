using CrytonCoreNext.Enums;
using CrytonCoreNext.Helpers;
using CrytonCoreNext.Interfaces;

namespace CrytonCoreNext.Providers
{
    public class PasswordProvider : IPasswordProvider
    {
        private const int MaxPasswordLenght = 24;

        private string _password;

        private EStrength _passwordStrenght;

        private EStrength _validationPasswordStrenght;

        public PasswordProvider()
        {
            _password = "";
            _passwordStrenght = EStrength.None;
            _validationPasswordStrenght = EStrength.Reasonable;
        }

        public string GetPassword() => _password;

        public string SetPassword(string password)
        {
            if (password.Length <= MaxPasswordLenght)
            {
                _password = password;
                _passwordStrenght = PasswordStrength(_password);
            }
            return _password;
        }

        public bool ValidatePassword()
        {
            return 
                !string.IsNullOrEmpty(_password) && 
                !string.IsNullOrWhiteSpace(_password) && 
                _passwordStrenght >= _validationPasswordStrenght;
        }

        public EStrength GetPasswordValidationStrength()
        {
            return _validationPasswordStrenght;
        }

        public EStrength GetPasswordStrenght()
        {
            return _passwordStrenght;
        }

        public void SetPasswordValidationStrenght(EStrength strength)
        {
            _validationPasswordStrenght = strength;
        }

        private static EStrength PasswordStrength(string password)
        {
            var score = password.GetStrengthScore();

            if (score == 0)
                return EStrength.None;

            if (score <= 2)
                return EStrength.VeryWeak;

            if (score > 2 && score <= 4)
                return EStrength.Weak;

            if (score > 4 && score <= 6)
                return EStrength.Reasonable;

            if (score > 6 && score <= 8)
                return EStrength.Strong;

            if (score > 8)
                return EStrength.VeryStrong;

            return EStrength.VeryWeak;
        }
    }
}
