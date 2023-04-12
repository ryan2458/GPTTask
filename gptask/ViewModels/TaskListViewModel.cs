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
        private List<TaskListItem> tasks = new List<TaskListItem>();

        public List<TaskListItem> Tasks
        {
            get => tasks;
            set
            {
                tasks = value;
                this.OnPropertyChanged(nameof(Tasks));
            }
        }

        public TaskListViewModel(List<TaskListItem> tasks)
        {
            this.Tasks = tasks;
        }

        public void OnNavigatedFrom()
        {
        }

        public void OnNavigatedTo()
        {
        }
    }
}
