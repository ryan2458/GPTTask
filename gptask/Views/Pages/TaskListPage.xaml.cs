﻿using gptask.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using gptask.Models;
using gptask.Services;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using GPTTextCompletions.Speech;
using MenuItem = System.Windows.Controls.MenuItem;
using GPTTextCompletions.ChatGPT;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System;

namespace gptask.Views.Pages
{
    /// <summary>
    /// Interaction logic for TaskListPage.xaml
    /// </summary>
    public partial class TaskListPage : INavigableView<ViewModels.TaskListViewModel>
    {
        private INavigationService _navigationService;

        private string currentTag;

        private string currentList;

        private SpeechListener speechListener = new SpeechListener();

        public TaskListViewModel ViewModel { get; private set; }

        public TaskListPage(INavigationService navigationService, TaskListViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;

            _navigationService = navigationService;

            speechListener.Engine.SpeechRecognized += Engine_SpeechRecognized;

            DataContext = ViewModel;
        }

        public void InitializeView()
        {
            _navigationService.GetNavigationControl().Navigated += Navigation_Navigated;
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
                    ViewModel.ChangeList(listId);
                    currentTag = listId.ToString();
                    currentList = nav.Current.Content.ToString();
                    AddTaskGrid.Visibility = Visibility.Visible;
                }
                else
                {
                    AddTaskGrid.Visibility = Visibility.Hidden;
                }
            }
        }

        public void AddList(int listId, List<TaskListItemModel> newList)
        {
            ViewModel.AddList(listId, newList);
        }

        public void DeleteTask(TaskListItemModel taskListItemModel)
        {
            ViewModel.DeleteTask(taskListItemModel);
        }

        /// <summary>
        /// Adds a subtask to a top-level task.
        /// </summary>
        /// <param name="parentTask">the top-level task.</param>
        /// <param name="subtaskName">the name of the subtask.</param>
        public void AddSubtask(TaskListItemModel parentTask, string subtaskName)
        {
            ViewModel.AddSubtask(parentTask, subtaskName, currentList, currentTag);
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            string taskName = AddItemTextBox.Text;
            AddTask(taskName, currentList, currentTag);
            AddItemTextBox.Clear();
        }

        private void AddTask(string taskName, string listName, string listTag)
        {
            ViewModel.AddTask(taskName, listName, listTag);
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
                        AddSubtask(parentTask, subtaskName);
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

        /// <summary>
        /// Raised when the break down task button is clicked.
        /// </summary>
        private void BreakDownTaskHandler(object sender, RoutedEventArgs e)
        {
            var task = (sender as MenuItem)?.DataContext as TaskListItemModel;

            if (task != null)
            {
                listProgressRing.Visibility = Visibility.Visible;
                listView.Visibility = Visibility.Hidden;
                AddTaskGrid.Visibility = Visibility.Hidden;

                Task.Run(async () => await BreakDownTaskAsync(task));
            }
        }

        /// <summary>
        /// Breaks a task down on a background thread.
        /// </summary>
        /// <param name="task">The task we're breaking down.</param>
        private async Task BreakDownTaskAsync(TaskListItemModel task)
        {
            GptCaller caller = new GptCaller();
            string subtasks = string.Empty;
            subtasks = await caller.PromptAsync(task.Name);

            // This is how we update the UI from a different thread.
            this.Dispatcher.Invoke(() =>
            {
                listProgressRing.Visibility = Visibility.Hidden;
                listView.Visibility = Visibility.Visible;
                AddTaskGrid.Visibility = Visibility.Visible;

                IEnumerable<string> subtaskArray = subtasks.Split('\n').Where(s => !string.IsNullOrEmpty(s));

                foreach (string subtask in subtaskArray)
                {
                    AddSubtask(task, subtask);
                }
            });
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

        private void ListenButton_Toggle(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton toggle)
            {
                if (toggle.IsChecked ?? false)
                {
                    speechListener.Listen();
                }
                else
                {
                    speechListener.Stop();
                }
            }
        }

        private void Engine_SpeechRecognized(object? sender, System.Speech.Recognition.SpeechRecognizedEventArgs e)
        {
            AddItemTextBox.Text += e.Result.Text;
            ListenButton.IsChecked = false;
        }

        private void TaskCheckedHandler(object sender, RoutedEventArgs e)
        {
            TaskCheckedUncheckedHandler(sender, e);
        }

        private void TaskUncheckedHandler(object sender, RoutedEventArgs e)
        {
            TaskCheckedUncheckedHandler(sender, e);
        }

        private void TaskCheckedUncheckedHandler(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox box)
            {
                TaskListItemModel task = (box.DataContext as TaskListItemModel)!;

                if (task != null)
                {
                    UpdateTask(task);

                    if (task.ParentTaskId == null)
                    {
                        foreach (var subtask in task.Subtasks)
                        {
                            subtask.Checked = task.Checked;
                            UpdateTask(subtask);
                        }
                    }
                }
            }
        }

        private void UpdateTask(TaskListItemModel task)
        {
            ViewModel.UpdateTask(task);
        }
    }
}
