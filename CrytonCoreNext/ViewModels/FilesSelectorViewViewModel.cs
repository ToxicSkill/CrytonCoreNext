using CrytonCoreNext.Abstract;

namespace CrytonCoreNext.ViewModels
{
    public class FilesSelectorViewViewModel : ViewModelBase
    {
        public FilesSelectorListingViewViewModel InProgressItemListingViewModel { get; }
        public FilesSelectorListingViewViewModel CompletedItemListingViewModel { get; }

        public FilesSelectorViewViewModel(FilesSelectorListingViewViewModel inProgressItemListingViewModel, FilesSelectorListingViewViewModel completedItemListingViewModel)
        {
            InProgressItemListingViewModel = inProgressItemListingViewModel;
            CompletedItemListingViewModel = completedItemListingViewModel;
        }
    }
}
