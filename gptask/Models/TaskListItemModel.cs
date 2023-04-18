using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System.Collections.ObjectModel;

namespace gptask.Models
{
    public class TaskListItemModel : ObservableObject
    {
        /// <summary>
        /// Backing field for Subtasks property.
        /// </summary>
        private ObservableCollection<TaskListItemModel> subtasks = new ObservableCollection<TaskListItemModel>();

        /// <summary>
        /// Backing field for Checked property.
        /// </summary>
        private bool @checked = false;

        /// <summary>
        /// Gets the Task id, not to be used for identifying the list this task belongs to.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } = 0;

        /// <summary>
        /// Gets the name of the list this task belongs to.
        /// </summary>
        public string ListName { get; set; }

        /// <summary>
        /// Gets the tag of the list this task belongs to.
        /// </summary>
        public string ListTag { get; set; }

        /// <summary>
        /// Gets the name of the task.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the content of the task.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets whether the task is checked or unchecked.
        /// </summary>
        public bool Checked 
        {
            get => @checked;
            set
            {
                @checked = value;
                OnPropertyChanged(nameof(@checked));
            }
        }

        /// <summary>
        /// Gets or sets the id of the parent task, if it exists.
        /// </summary>
        public int? ParentTaskId { get; set; }

        [Ignore]
        public ObservableCollection<TaskListItemModel> Subtasks 
        {
            get => subtasks;
            set
            {
                subtasks = value;
                OnPropertyChanged(nameof(Subtasks));
            }
        }

        public TaskListItemModel()
        {
        }

        public TaskListItemModel(string listName, string listTag = "", string name = "", 
            string content = "", int? parentTaskId = null)
        {
            ListName = listName;
            ListTag = listTag;
            Name = name;
            Content = content;
            ParentTaskId = parentTaskId;
        }
    }
}
