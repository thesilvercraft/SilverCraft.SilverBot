using DSharpPlus.CommandsNext;

namespace SilverBot.Shared
{
    public interface ISpecialModuleRegistration
    {
        public Task Register(CommandsNextExtension commandsNextExtension);
    }

    /// <summary>
    /// A simple callback interface, a timer of sorts
    /// </summary>
    public interface ICallBack
    {
        /// <summary>
        /// Adds a Callback to the current queue
        /// </summary>
        /// <param name="task">the task to run that returns when to run it next</param>
        /// <param name="when">when to run the task</param>
        public void Add(Func<Task<DateTime?>> task, DateTime when);

        public void Add(Func<Task<double?>> task, double when);
    }

    public class TaskSchedulerService : ICallBack
    {
        private PriorityQueue<Func<Task<DateTime?>>, DateTime> Queue = new();
        private DateTime NextCall = DateTime.MaxValue;
        private Timer timer;
        public int NumberOfCallBacks => Queue.Count;

        public TaskSchedulerService()
        {
            timer = new Timer(CheckQueue, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        public void Add(Func<Task<DateTime?>> task, DateTime when)
        {
            if (when < NextCall)
            {
                NextCall = when;
            }

            Queue.Enqueue(task, when);
        }

        public void Add(Func<Task<double?>> task, double when)
        {
            var whenDateTime = ConvertUlongToDateTime(when);
            var taskDateTime = WrapTask(task);
            Add(taskDateTime, whenDateTime);
        }

        private void CheckQueue(object? state)
        {
            if (DateTime.Now < NextCall || Queue.Count <= 0 || !Queue.TryDequeue(out var task, out var when))
            {
                return;
            }

            Task.Run(async () =>
            {
                var result = await task();
                if (result is { } dt)
                {
                    Add(task, dt);
                }
            });
            if (Queue.Count <= 0 || !Queue.TryPeek(out _, out NextCall))
            {
                NextCall = DateTime.MaxValue;
            }
        }

        private static DateTime ConvertUlongToDateTime(double when)
        {
            return DateTime.Now.AddMilliseconds(when);
        }

        private static Func<Task<DateTime?>> WrapTask(Func<Task<double?>> task)
        {
            return async () =>
            {
                var res = await task.Invoke();
                if (res is { } r && !double.IsNaN(r) && !double.IsNegativeInfinity(r) && !double.IsPositiveInfinity(r))
                {
                    return DateTime.Now.AddMilliseconds(r);
                }

                return null;
            };
        }
    }
}