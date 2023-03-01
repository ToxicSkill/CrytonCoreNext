using CrytonCoreNext.ViewModels;
using Wpf.Ui.Common.Interfaces;

namespace CrytonCoreNext.Views
{
    /// <summary>
    /// Interaction logic for PeopleView.xaml
    /// </summary>
    public partial class CryptingView : INavigableView<CryptingViewModel>
    {
        public CryptingViewModel ViewModel
        {
            get;
        }

        public CryptingView(CryptingViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = ViewModel;
            //RegisterAnimations();
        }

        //private void RegisterAnimations()
        //{
        //    var pathAnimations = this.PathsAnimations.FindVisualChildren<Path>().ToList();
        //    ViewModel.RegisterAnimations(pathAnimations);
        //}

        //private void UiPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    ViewModel.SetRandomPathAnimation();
        //}
    }
}
