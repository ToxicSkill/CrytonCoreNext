using CrytonCoreNext.BackgroundUI;
using CrytonCoreNext.ViewModels;
using System.Threading.Tasks;
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
            Background.Content = new FluentWaves();
        }

        private void UiPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            //Task.Run(ViewModel.OnLoaded);
        }

        private void UiPage_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Task.Run(ViewModel.OnUnloaded);
        }
    }
}