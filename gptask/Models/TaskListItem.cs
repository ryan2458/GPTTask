using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gptask.Models
{
    public class TaskListItem
    {
        private static int id = 1;

        public int Id { get; }

        public string ListName { get; }

        public string Name { get; }

        public string Content { get; }

        public bool Checked { get; set; } = false;

        public TaskListItem(string listName, string name = "", string content = "")
        {
            ListName = listName;
            Name = name;
            Content = content;

            Id = id++;
        }
    }
}
