using CrytonCoreNext.Providers; 

namespace CrytonCoreNext.Crypting.Helpers
{
    public class DESHelper
    {
        public PasswordProvider PasswordProvider { get; set; }

        public DESHelper()
        {
            PasswordProvider = new PasswordProvider();
        }
    }
}
