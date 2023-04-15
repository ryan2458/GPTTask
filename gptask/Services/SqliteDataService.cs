using gptask.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
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

            Task.Run(async () => await InitializeDatabase()).Wait();
        }

        private async Task InitializeDatabase()
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
            try
            {
                return await _db.Table<TaskListItemModel>().Where(x => x.ListTag == listTag).ToListAsync();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<List<TaskListItemModel>> GetSubTasksAsync(int parentId)
        {
            return await _db.Table<TaskListItemModel>().Where(x => x.ParentTaskId == parentId).ToListAsync();
        }

        // TODO: Don't return list.id.  Just extract that after this returns.
        public async Task<int> AddOrUpdateListAsync(ListModel list)
        {
            if (list.Id != 0)
            {
                await _db.UpdateAsync(list);
                return list.Id;
            }
            else
            {
                await _db.InsertAsync(list);
                return list.Id;
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
