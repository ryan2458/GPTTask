using gptask.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gptask.Services
{
    public interface IDataService
    {
        Task<List<ListModel>> GetAllListsAsync();
        Task<List<TaskListItemModel>> GetTaskListItemsAsync(string listTag);
        Task<List<TaskListItemModel>> GetSubTasksAsync(int parentId);
        Task<int> AddOrUpdateListAsync(ListModel list);
        Task<int> AddOrUpdateTaskListItemAsync(TaskListItemModel taskListItem);
        Task DeleteListAsync(int listId);
        Task DeleteTaskListItemAsync(int taskListItemId);
    }
}
