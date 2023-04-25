using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using gptask.Models;
using Wpf.Ui.Common.Interfaces;
using System.Threading.Tasks;
using gptask.Services;
using Wpf.Ui.Mvvm.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace gptask.ViewModels
{
    public partial class TaskListViewModel : ObservableObject, INavigationAware
    {
        private IDictionary<int, ObservableCollection<TaskListItemModel>> lists = new Dictionary<int, ObservableCollection<TaskListItemModel>>();

        private IDataService _dataService = null;

        private ObservableCollection<TaskListItemModel> tasks = new ObservableCollection<TaskListItemModel>();

        public List<ListModel> ListModels { get; private set; }

        public ObservableCollection<TaskListItemModel> Tasks
        {
            get => tasks;
            set
            {
                tasks = value;
                this.OnPropertyChanged(nameof(Tasks));
            }
        }

        public TaskListViewModel(IDataService dataService)
        {
            _dataService = dataService;

            Task.Run(async () => await LoadListsAndTasksAsync(dataService)).Wait();
        }

        public void ChangeList(int listId)
        {
            Tasks = lists[listId];
        }

        public void AddList(int listId, List<TaskListItemModel> newList)
        {
            lists.Add(listId, new ObservableCollection<TaskListItemModel>(newList));
        }

        public void AddTask(string taskName, string listName, string listTag)
        {
            TaskListItemModel task = new TaskListItemModel(listName, listTag, taskName);
            Tasks.Add(task);
            Task.Run(async () => await _dataService.AddOrUpdateTaskListItemAsync(task)).Wait();
        }

        public void UpdateTask(TaskListItemModel task)
        {
            Task.Run(async () => await _dataService.AddOrUpdateTaskListItemAsync(task)).Wait();
        }


        /// <summary>
        /// Adds a subtask to a top-level task.
        /// </summary>
        /// <param name="parentTask">the top-level task.</param>
        /// <param name="subtaskName">the name of the subtask.</param>
        /// <param name="list">the list we're adding the task to.</param>
        /// <param name="tag">the tag associated with this task.</param>
        public void AddSubtask(TaskListItemModel parentTask, string subtaskName, string list, string tag)
        {
            TaskListItemModel subtask = new TaskListItemModel(list, tag,
                            subtaskName, "", parentTask.Id);
            parentTask.Subtasks.Add(subtask);
            Task.Run(async () => await _dataService.AddOrUpdateTaskListItemAsync(subtask)).Wait();
        }

        public void DeleteTask(TaskListItemModel taskListItemModel)
        {
            for (int i = taskListItemModel.Subtasks.Count - 1; i >= 0; i--)
            {
                TaskListItemModel? subtask = taskListItemModel.Subtasks[i];
                Task.Run(async () => await _dataService.DeleteTaskListItemAsync(subtask.Id));
                taskListItemModel.Subtasks.RemoveAt(i);
            }

            Task.Run(async () => await _dataService.DeleteTaskListItemAsync(taskListItemModel.Id));
            Tasks.Remove(taskListItemModel);
        }

        public void OnNavigatedFrom()
        {
        }

        public void OnNavigatedTo()
        {
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
                Tasks = lists.First().Value;
            }
        }
    }
}
