using NUnit.Framework;
using gptask.ViewModels;
using gptask.Services;
using System.Xaml;
using gptask.Models;

namespace GPTaskUnitTests
{
    public class TaskListUnitTests
    {
        TaskListViewModel viewModel;

        [SetUp]
        public void Setup()
        {
            viewModel = new TaskListViewModel(new SqliteDataService("Test.db"));
        }

        [Test]
        public void TestAddTask()
        {
            viewModel.AddTask("testTask", "testList", "testTag");

            Assert.Multiple(() =>
            {
                Assert.That(viewModel.Tasks.First().Name, Is.EqualTo("testTask"));
                Assert.That(viewModel.Tasks.First().ListName, Is.EqualTo("testList"));
                Assert.That(viewModel.Tasks.First().ListTag, Is.EqualTo("testTag"));
            });

            viewModel.Tasks.Clear();
        }

        [Test]
        public void TestDeleteTask()
        {
            viewModel.AddTask("testTask", "testList", "testTag");
            TaskListItemModel task = viewModel.Tasks.First();
            viewModel.DeleteTask(task);

            Assert.That(viewModel.Tasks.Count, Is.EqualTo(0));
        }

        [Test]
        public void TestAddSubtask()
        {
            TestAddTask();

            
        }
    }
}