using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SilverBotDS.ProgramExtensions
{
    public class TaskService
    {
        private readonly Dictionary<string, Tuple<Task, CancellationTokenSource>> RunningTasks = new();
        private readonly Dictionary<Guid, Tuple<Task, CancellationTokenSource>> RunningTasksOfSecondRow = new();


        public void CancelTasks()
        {
            foreach (var task in RunningTasksOfSecondRow)
            {
                task.Value.Item2.Cancel();
            }

            foreach (var task in RunningTasks)
            {
                task.Value.Item2.Cancel();
            }
        }

        public int NumberOfRunningTasks()
        {
            return RunningTasks.Count + RunningTasksOfSecondRow.Count;
        }

        public void ClearDeadTasks()
        {
            foreach (var task in RunningTasks.Where(task => task.Value.Item1.IsCompleted))
            {
                RunningTasks.Remove(task.Key);
            }

            foreach (var task in RunningTasksOfSecondRow.Where(task => task.Value.Item1.IsCompleted))
            {
                RunningTasksOfSecondRow.Remove(task.Key);
            }
        }

        public void AddMain(string taskName, Tuple<Task, CancellationTokenSource> task)
        {
            RunningTasks.Add(taskName, task);
        }

        public void AddSecondaryTask(Guid a, Tuple<Task, CancellationTokenSource> b)
        {
            RunningTasksOfSecondRow.Add(a, b);
        }
    }
}