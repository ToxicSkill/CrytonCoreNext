using CrytonCoreNext.Enums;
namespace CrytonCoreNext.Interfaces
{
    public interface IPasswordProvider
    {
        string SetPassword(string password);

        string GetPassword();

        bool ValidatePassword();

        EStrength GetPasswordValidationStrength();

        EStrength GetPasswordStrenght();

        void SetPasswordValidationStrenght(EStrength strength);
    }
}
