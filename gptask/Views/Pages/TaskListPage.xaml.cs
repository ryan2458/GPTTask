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
using gptask.Services;
using Wpf.Ui.Mvvm.Services;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Media.Animation;
using MenuItem = System.Windows.Controls.MenuItem;

namespace gptask.Views.Pages
{
    /// <summary>
    /// Interaction logic for TaskListPage.xaml
    /// </summary>
    public partial class TaskListPage : INavigableView<ViewModels.TaskListViewModel>
    {
        private IDictionary<int, ObservableCollection<TaskListItemModel>> lists = new Dictionary<int, ObservableCollection<TaskListItemModel>>();

        private INavigationService _navigationService;

        private IDataService _dataService;

        private string currentTag;

        private string currentList;

        public List<ListModel> ListModels { get; private set; }

        public TaskListPage(INavigationService navigationService, TaskListViewModel viewModel,
            IDataService dataService)
        {
            InitializeComponent();
            ViewModel = viewModel;

            Task.Run(async () => await LoadListsAndTasksAsync(dataService)).Wait();
            _navigationService = navigationService;
            _dataService = dataService;

            DataContext = ViewModel;
        }

        public void InitializeView()
        {
            _navigationService.GetNavigationControl().Navigated += Navigation_Navigated;
        }

        private async Task LoadListsAndTasksAsync(IDataService dataService)
        {
            // Fetch lists from the database
            ListModels = await dataService.GetAllListsAsync();

            // Fetch tasks for each list and add them to the lists collection
            foreach (var list in ListModels)
            {
                var tasks = await dataService.GetTaskListItemsAsync(list.Tag);

                for (int i = 0; i < tasks.Count; ++i)
                {
                    TaskListItemModel task = tasks[i];
                    if (task.ParentTaskId != null)
                    {
                        var parentTask = tasks.Find(t => t.Id == task.ParentTaskId);
                        parentTask!.Subtasks.Add(task);
                        tasks.Remove(task);
                        i -= 1;
                    }
                }

                lists.Add(list.Id, new ObservableCollection<TaskListItemModel>(tasks));
            }

            if (lists.Count > 0)
            {
                ViewModel.Tasks = lists.First().Value;
            }
        }

        private void Navigation_Navigated([System.Diagnostics.CodeAnalysis.NotNull] 
            INavigation sender, RoutedNavigationEventArgs e)
        {
            var nav = (e.Source as NavigationFluent);

            if (nav!.Current?.PageTag != "settings")
            {
                bool parsed = int.TryParse(nav?.Current?.PageTag, out int listId);

                if (parsed)
                {
                    ViewModel.Tasks = lists[listId];
                    currentTag = listId.ToString();
                    currentList = nav.Current.Content.ToString();
                }
            }
        }

        public TaskListViewModel ViewModel { get; private set; }

        public void AddList(int listId, List<TaskListItemModel> newList)
        {
            lists.Add(listId, new ObservableCollection<TaskListItemModel>(newList));
        }

        public void DeleteTask(TaskListItemModel taskListItemModel)
        {
            // Remove all corresponding subtasks
            for (int i = taskListItemModel.Subtasks.Count - 1; i >= 0; i--)
            {
                TaskListItemModel? subtask = taskListItemModel.Subtasks[i];
                Task.Run(async () => await _dataService.DeleteTaskListItemAsync(subtask.Id));
                taskListItemModel.Subtasks.RemoveAt(i);
            }

            ViewModel.DeleteTask(taskListItemModel);
            Task.Run(async () => await _dataService.DeleteTaskListItemAsync(taskListItemModel.Id));
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            string taskName = AddItemTextBox.Text;

            string listName = currentList;
            string listTag = currentTag;

            TaskListItemModel task = new TaskListItemModel(listName, listTag, taskName);

            ViewModel.AddTask(task);

            Task.Run(async () => await _dataService.AddOrUpdateTaskListItemAsync(task)).Wait();

            AddItemTextBox.Clear();
        }

        private void AddSubtaskButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Wpf.Ui.Controls.Button;
            var parentTask = button?.DataContext as TaskListItemModel;

            if (parentTask != null)
            {
                var addSubtaskTextBox = FindNameInParent("AddSubtaskTextBox", button!) as Wpf.Ui.Controls.TextBox;
                
                if (addSubtaskTextBox != null)
                {
                    string subtaskName = addSubtaskTextBox.Text;

                    if (!string.IsNullOrEmpty(subtaskName))
                    {
                        TaskListItemModel subtask = new TaskListItemModel(currentList, currentTag,
                            subtaskName, "", parentTask.Id);

                        parentTask.Subtasks.Add(subtask);

                        Task.Run(async () => await _dataService.AddOrUpdateTaskListItemAsync(subtask)).Wait();
                    }

                    addSubtaskTextBox.Clear();
                }
            }
        }

        private FrameworkElement FindNameInParent(string name, FrameworkElement element)
        {
            while (element != null && element.Parent != null)
            {
                element = element.Parent as FrameworkElement;
                if (element.FindName(name) is FrameworkElement foundElement)
                {
                    return foundElement;
                }
            }
            return null;
        }

        private void EditTaskHandler(object sender, RoutedEventArgs e)
        {

        }

        private void BreakDownTaskHandler(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Raised when the delete menu item in the right-click context menu on a task is clicked.
        /// </summary>
        private void DeleteTaskHandler(object sender, RoutedEventArgs e)
        {
            TaskListItemModel taskListItemModel = ((sender as MenuItem)!.DataContext as TaskListItemModel)!;

            if (taskListItemModel.ParentTaskId != null)
            {
                var parentTask = ViewModel.Tasks.Where(task => task.Id == taskListItemModel.ParentTaskId).FirstOrDefault();
                parentTask?.Subtasks.Remove(taskListItemModel);
            }

            DeleteTask(taskListItemModel);
        }
    }
}
