using gptask.Models;
using gptask.Services;
using gptask.Views.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wpf.Ui.Common;
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
        private IDataService _dataService;

        private TaskListPage _taskListPage;

        public ViewModels.MainWindowViewModel ViewModel
        {
            get;
        }

        public MainWindow(ViewModels.MainWindowViewModel viewModel, IPageService pageService, 
            INavigationService navigationService,
            IDataService dataService,
            TaskListPage taskListPage)
        {
            ViewModel = viewModel;
            DataContext = this;

            _dataService = dataService;
            _taskListPage = taskListPage;

            InitializeComponent();
            SetPageService(pageService);

            navigationService.SetNavigationControl(RootNavigation);

            _taskListPage.InitializeView();

            foreach (ListModel model in taskListPage.ListModels)
            {
                NavigationItem navigationItem = new NavigationItem()
                {
                    Content = model.Name,
                    PageTag = model.Tag,
                    Icon = SymbolRegular.List24,
                    PageType = typeof(Views.Pages.TaskListPage)
                };

                ViewModel.NavigationItems.Add(navigationItem);
            }


            foreach (NavigationItem item in ViewModel.NavigationItems)
            {
                item.Click += Item_Click;
            }


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
            AddListPopup.IsOpen = true;
            AddListTextBox.Focus();
        }

        private async void AddListTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var newListModel = new ListModel();

                newListModel.Name = AddListTextBox.Text;
                int listId = await _dataService.AddOrUpdateListAsync(newListModel);

                string newListTag = $"{newListModel.Id}";
                newListModel.Tag = newListTag;

                await _dataService.AddOrUpdateListAsync(newListModel);

                _taskListPage.AddList(listId, new List<TaskListItemModel>());

                var newItem = new NavigationItem()
                {
                    Content = newListModel.Name,
                    PageTag = newListTag,
                    Icon = Wpf.Ui.Common.SymbolRegular.Airplane20,
                    PageType = typeof(Views.Pages.TaskListPage)
                };

                ViewModel.NavigationItems.Add(newItem);
                newItem.Click += Item_Click;
                AddListPopup.IsOpen = false;
                AddListTextBox.Clear();
            }
        }

        private void AddListTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            AddListPopup.IsOpen = false;
            AddListTextBox.Clear();
        }
    }
}