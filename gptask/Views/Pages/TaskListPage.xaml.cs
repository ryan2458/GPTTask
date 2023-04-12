using gptask.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;
using Wpf.Ui.Mvvm.Interfaces;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using gptask.Models;

namespace gptask.Views.Pages
{
    /// <summary>
    /// Interaction logic for TaskListPage.xaml
    /// </summary>
    public partial class TaskListPage : INavigableView<ViewModels.TaskListViewModel>
    {
        List<List<TaskListItem>> lists = new List<List<TaskListItem>>();

        public TaskListPage(INavigationService navigationService)
        {
            List<TaskListItem> tasks1 = new List<TaskListItem>();
            tasks1.Add(new TaskListItem("List", "1", "stuff"));
            tasks1.Add(new TaskListItem("List", "1", "stuff"));
            tasks1.Add(new TaskListItem("List", "1", "stuff"));
            tasks1.Add(new TaskListItem("List", "1", "stuff"));
            tasks1.Add(new TaskListItem("List", "1", "stuff"));

            List<TaskListItem> tasks2 = new List<TaskListItem>();
            tasks2.Add(new TaskListItem("Test", "2", "stuff"));
            tasks2.Add(new TaskListItem("Test", "2", "stuff"));
            tasks2.Add(new TaskListItem("Test", "2", "stuff"));
            tasks2.Add(new TaskListItem("Test", "2", "stuff"));
            tasks2.Add(new TaskListItem("Test", "2", "stuff"));
            tasks2.Add(new TaskListItem("Test", "2", "stuff"));
            tasks2.Add(new TaskListItem("Test", "2", "stuff"));
            tasks2.Add(new TaskListItem("Test", "2", "stuff"));
            tasks2.Add(new TaskListItem("Test", "2", "stuff"));
            tasks2.Add(new TaskListItem("Test", "2", "stuff"));

            lists.Add(tasks1);
            lists.Add(tasks2);

            ViewModel = new TaskListViewModel(tasks1);
            InitializeComponent();

            navigationService.GetNavigationControl().Navigated += Navigation_Navigated;
        }

        private void Navigation_Navigated([System.Diagnostics.CodeAnalysis.NotNull] 
            INavigation sender, RoutedNavigationEventArgs e)
        {
            var nav = (e.Source as NavigationFluent);
            ViewModel.Tasks = lists[nav!.SelectedPageIndex];
        }

        public TaskListViewModel ViewModel { get; private set; }
    }
}
