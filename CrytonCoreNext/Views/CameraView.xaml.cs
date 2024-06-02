using CrytonCoreNext.ViewModels;
using Wpf.Ui.Controls;

namespace CrytonCoreNext.Views
{
    public partial class CameraView : INavigableView<CameraViewModel>
    {
        public CameraViewModel ViewModel
        {
            get;
        }

        public CameraView(CameraViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = ViewModel;
            //Background.Content = new FluentWaves();
        }

        private void UiPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ViewModel.SetNavigationControl(CameraNavigationView);
            //Task.Run(ViewModel.OnLoaded);
        }
    }
}