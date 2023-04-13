using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace gptask.Views.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INavigationWindow
    {
        public ViewModels.MainWindowViewModel ViewModel
        {
            get;
        }

        public MainWindow(ViewModels.MainWindowViewModel viewModel, IPageService pageService, INavigationService navigationService)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
            SetPageService(pageService);

            navigationService.SetNavigationControl(RootNavigation);
            
            foreach (NavigationItem item in ViewModel.NavigationItems)
            {
                item.Click += Item_Click;
            }

        }

        public void AddNewList()
        {

        }

        private void Item_Click(object sender, RoutedEventArgs e)
        {
            NavigationItem item = e.Source as NavigationItem;
            RootNavigation.Navigate(item.PageTag);
        }

        #region INavigationWindow methods

        public Frame GetFrame()
            => RootFrame;

        public INavigation GetNavigation()
            => RootNavigation;

        public bool Navigate(Type pageType)
        {
            return RootNavigation.Navigate(pageType);
        }

        public void SetPageService(IPageService pageService)
            => RootNavigation.PageService = pageService;

        public void ShowWindow()
            => Show();

        public void CloseWindow()
            => Close();

        #endregion INavigationWindow methods

        /// <summary>
        /// Raises the closed event.
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Make sure that closing this window will begin the process of closing the application.
            Application.Current.Shutdown();
        }

        private void AddListButton_Click(object sender, RoutedEventArgs e)
        {
            var newItem = new NavigationItem()
            {
                Content = "New List",
                PageTag = $"test",
                Icon = Wpf.Ui.Common.SymbolRegular.Airplane20,
                PageType = typeof(Views.Pages.TaskListPage)
            };

            newItem.Click += Item_Click;

            ViewModel.NavigationItems.Add(newItem);
        }
    }
}