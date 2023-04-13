using gptask.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gptask.Services
{
    public class SqliteDataService : IDataService
    {
        private readonly SQLiteAsyncConnection _db;

        public SqliteDataService()
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TaskList.db");
            _db = new SQLiteAsyncConnection(dbPath);

            InitializeDatabase();
        }

        private async void InitializeDatabase()
        {
            await _db.CreateTableAsync<ListModel>();
            await _db.CreateTableAsync<TaskListItemModel>();
        }

        public async Task<List<ListModel>> GetAllListsAsync()
        {
            return await _db.Table<ListModel>().ToListAsync();
        }

        public async Task<List<TaskListItemModel>> GetTaskListItemsAsync(string listTag)
        {
            return await _db.Table<TaskListItemModel>().Where(x => x.ListTag == listTag).ToListAsync();
        }

        public async Task<List<TaskListItemModel>> GetSubTasksAsync(int parentId)
        {
            return await _db.Table<TaskListItemModel>().Where(x => x.ParentTaskId == parentId).ToListAsync();
        }

        public async Task<int> AddOrUpdateListAsync(ListModel list)
        {
            if (list.Id != 0)
            {
                return await _db.UpdateAsync(list);
            }
            else
            {
                return await _db.InsertAsync(list);
            }
        }

        public async Task<int> AddOrUpdateTaskListItemAsync(TaskListItemModel taskListItem)
        {
            if (taskListItem.Id != 0)
            {
                return await _db.UpdateAsync(taskListItem);
            }
            else
            {
                return await _db.InsertAsync(taskListItem);
            }
        }

        public async Task DeleteListAsync(int listId)
        {
            await _db.DeleteAsync<ListModel>(listId);
        }

        public async Task DeleteTaskListItemAsync(int taskListItemId)
        {
            await _db.DeleteAsync<TaskListItemModel>(taskListItemId);
        }
    }
}
