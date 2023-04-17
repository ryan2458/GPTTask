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
        /// <summary>
        /// A reference to the database.
        /// </summary>
        private IDataService _dataService;

        /// <summary>
        /// A reference to the task list page.
        /// This is where our TaskListItemModel item information is displayed.
        /// </summary>
        private TaskListPage _taskListPage;

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        public ViewModels.MainWindowViewModel ViewModel
        {
            get;
        }

        /// <summary>
        /// Constructs a new MainWindow
        /// </summary>
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
                SetupNavigationItem(navigationItem, model);
            }
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

        /// <summary>
        /// Raised when the AddList button is clicked.
        /// Brings up a textbox for a new list to be created.
        /// </summary>
        private void AddListButton_Click(object sender, RoutedEventArgs e)
        {
            AddListPopup.IsOpen = true;
            AddListTextBox.Focus();
        }

        /// <summary>
        /// Raised when a key is pressed in the AddListTextBox control when adding a list.
        /// Only succeeds if the user presses enter.
        /// </summary>
        private void AddListTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var newListModel = new ListModel();

                newListModel.Name = AddListTextBox.Text;
                int listId = -1;
                Task.Run(async () => listId = await _dataService.AddOrUpdateListAsync(newListModel)).Wait();

                string newListTag = $"{newListModel.Id}";
                newListModel.Tag = newListTag;

                Task.Run(async () => await _dataService.AddOrUpdateListAsync(newListModel)).Wait();

                _taskListPage.AddList(listId, new List<TaskListItemModel>());

                var newItem = new NavigationItem()
                {
                    Content = newListModel.Name,
                    PageTag = newListTag,
                    Icon = Wpf.Ui.Common.SymbolRegular.Airplane20,
                    PageType = typeof(Views.Pages.TaskListPage)
                };

                ViewModel.NavigationItems.Add(newItem);
                SetupNavigationItem(newItem, newListModel);
                AddListPopup.IsOpen = false;
                AddListTextBox.Clear();
            }
        }

        /// <summary>
        /// Raised when the textbox for creating a list loses focus.
        /// </summary>
        private void AddListTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            AddListPopup.IsOpen = false;
            AddListTextBox.Clear();
        }

        /// <summary>
        /// Deletes a list.
        /// </summary>
        /// <param name="sender">the sender of the delete list click event.</param>
        /// <param name="e">event args</param>
        /// <param name="listModel">the list model associated with this list.</param>
        /// <exception cref="NotImplementedException"></exception>
        private void DeleteListClick(object sender, RoutedEventArgs e, ListModel listModel)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Edits a list's name.
        /// </summary>
        /// <param name="sender">the sender of the edit list click event.</param>
        /// <param name="e">event args</param>
        /// <param name="listModel">the list model associated with this list.</param>
        /// <exception cref="NotImplementedException"></exception>
        private void EditListClick(object sender, RoutedEventArgs e, ListModel listModel)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handles navigation for when a list item is clicked.
        /// </summary>
        /// <param name="sender">the sender of the click event.</param>
        /// <param name="e">event args, should be the navigation item.</param>
        private void Item_Click(object sender, RoutedEventArgs e)
        {
            NavigationItem item = e.Source as NavigationItem;
            RootNavigation.Navigate(item.PageTag);
        }

        /// <summary>
        /// Sets up a navigation for full functionality.
        /// </summary>
        /// <param name="item">The navigation item we're setting up.</param>
        /// <param name="listModel">The list model that corresponds to this navigation item.</param>
        private void SetupNavigationItem(NavigationItem item, ListModel listModel)
        {
            item.Click += Item_Click;

            // Create and add context menu to each list.
            ContextMenu menu = new ContextMenu();

            var editMenuItem = new System.Windows.Controls.MenuItem() { Header = "Edit" };
            var deleteMenuItem = new System.Windows.Controls.MenuItem() { Header = "Delete" };

            editMenuItem.Click += (s, e) => EditListClick(s, e, listModel);
            deleteMenuItem.Click += (s, e) => DeleteListClick(s, e, listModel);

            menu.Items.Add(editMenuItem);
            menu.Items.Add(deleteMenuItem);
            item.ContextMenu = menu;
        }
    }
}