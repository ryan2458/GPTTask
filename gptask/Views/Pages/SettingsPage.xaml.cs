using Wpf.Ui.Common.Interfaces;

namespace gptask.Views.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : INavigableView<ViewModels.SettingsViewModel>
    {
        public ViewModels.SettingsViewModel ViewModel
        {
            get;
        }

        public SettingsPage()
        {

        }

        public SettingsPage(ViewModels.SettingsViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();
        }
    }
}