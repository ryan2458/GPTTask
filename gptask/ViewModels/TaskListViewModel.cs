using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using gptask.Models;
using Wpf.Ui.Common.Interfaces;
using System.Windows.Controls;
using System.ComponentModel;

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
            this.OnPropertyChanged(nameof(Tasks));
        }

        public void OnNavigatedFrom()
        {
        }

        public void OnNavigatedTo()
        {
        }
    }
}
