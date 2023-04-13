﻿using SQLite;

namespace gptask.Models
{
    public class TaskListItemModel
    {
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
        public bool Checked { get; set; } = false;

        public int? ParentTaskId { get; set; }

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
