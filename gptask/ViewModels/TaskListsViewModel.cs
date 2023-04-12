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
    public partial class TaskListsViewModel : ObservableObject, INavigationAware
    {
        private List<Task> tasks = new List<Task>();

        public List<Task> Tasks
        {
            get => tasks;
            set
            {
                tasks = value;
                this.OnPropertyChanged(nameof(Tasks));
            }
        }

        public TaskListsViewModel(List<Task> tasks)
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
