using CrytonCoreNext.Abstract;

namespace CrytonCoreNext.ViewModels
{
    public class InformationPopupViewModel : ViewModelBase
    {
        public string InformationString { get; init; }

        public InformationPopupViewModel(string informationString = "")
        {
            InformationString = informationString;
        }
    }
}