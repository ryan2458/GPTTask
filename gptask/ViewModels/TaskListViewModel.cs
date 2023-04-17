using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using gptask.Models;
using Wpf.Ui.Common.Interfaces;

namespace gptask.ViewModels
{
    public partial class TaskListViewModel : ObservableObject, INavigationAware
    {
        private ObservableCollection<TaskListItemModel> tasks = new ObservableCollection<TaskListItemModel>();

        public ObservableCollection<TaskListItemModel> Tasks
        {
            get => tasks;
            set
            {
                tasks = value;
                this.OnPropertyChanged(nameof(Tasks));
            }
        }

        public void AddTask(TaskListItemModel newTask)
        {
            Tasks.Add(newTask);
        }

        public void DeleteTask(TaskListItemModel taskToDelete)
        {
            Tasks.Remove(taskToDelete);
        }

        public void OnNavigatedFrom()
        {
        }

        public void OnNavigatedTo()
        {
        }
    }
}
