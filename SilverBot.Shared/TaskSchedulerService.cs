using System.Diagnostics;
using System.Runtime.InteropServices.JavaScript;

namespace SilverBot.Shared
{
    public class ScheduledTask
    {
        public ScheduledTask(DateTime? nextCall, Func<Task<DateTime?>> function, string? name = null,
            CancellationTokenSource? source = null)
        {
            Function = function;
            NextCall = nextCall;
            Id = Guid.NewGuid();
            Name = name;
            TokenSource = source;
        }

        public Func<Task<DateTime?>> Function { get; }
        public DateTime? NextCall { get; set; }
        public Guid Id { get; }
        public CancellationTokenSource? TokenSource { get; set; }
        public string? Name { get; set; }
    }

    public class TaskSchedulerService : ICallBack
    {
        public PriorityQueue<ScheduledTask, DateTime> Queue = new();
        private DateTime _nextCall = DateTime.MaxValue;
        private Timer _timer;
        public int NumberOfCallBacks => Queue.Count;
        public DateTime? NextCallBackTime => _nextCall;
        private Guid _lastCallBackId = Guid.Empty;

        public DateTime? LastCallBackTime { get; private set; } = DateTime.MinValue;

        //This might come to bite me one day, might need to look into locks and other stuff
        public int RunningTaskCount { get; private set; }

        public TaskSchedulerService()
        {
            _timer = new Timer(CheckQueue, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }
        
        public void Add(Func<Task<DateTime?>> task, DateTime when, string? name = null,
            CancellationTokenSource? source = null)
        {
            if (when < _nextCall)
            {
                _nextCall = when;
            }

            var element = new ScheduledTask(when, task, name, source);
            if (when > LastCallBackTime)
            {
                LastCallBackTime = when;
                _lastCallBackId = element.Id;
            }

            Queue.Enqueue(element, when);
        }
        public void AddOnce(Func<Task> task, DateTime when, string? name = null,
            CancellationTokenSource? source = null)
        {
            var taskDateTime = WrapTask(task);
            Add(taskDateTime, when, name, source);
        }
        public void Add(Func<Task<double?>> task, double when, string? name = null,
            CancellationTokenSource? source = null)
        {
            var whenDateTime = ConvertUlongToDateTime(when);
            var taskDateTime = WrapTask(task);
            Add(taskDateTime, whenDateTime, name, source);
        }

        private void ReAdd(ScheduledTask task, DateTime when)
        {
            if (when < _nextCall)
            {
                _nextCall = when;
            }

            task.NextCall = when;
            if (when > LastCallBackTime)
            {
                LastCallBackTime = when;
                _lastCallBackId = task.Id;
            }

            Queue.Enqueue(task, when);
        }

        private void CheckQueue(object? state)
        {
            if (DateTime.Now < _nextCall || Queue.Count <= 0 || !Queue.TryDequeue(out var task, out var when))
            {
                return;
            }

            if (task.Id == _lastCallBackId)
            {
                LastCallBackTime = DateTime.MinValue;
                _lastCallBackId = Guid.Empty;
            }
            if (task.TokenSource?.Token is { } token)
            {
                Task.Run(async () =>
                {
                    RunningTaskCount++;
                    var result = await task.Function();
                    if (result is { } dt)
                    {
                        ReAdd(task, dt);
                    }
                    RunningTaskCount--;
                }, token);
            }
            else
            {
                Task.Run(async () =>
                {
                    RunningTaskCount++;
                    var result = await task.Function();
                    if (result is { } dt)
                    {
                        ReAdd(task, dt);
                    }
                    RunningTaskCount--;
                });
            }
           
            if (Queue.Count == 0)
            {
                _nextCall = DateTime.MaxValue;
                LastCallBackTime = DateTime.MinValue;
            }
            else if (Queue.TryPeek(out _, out var time))
            {
                _nextCall = time;
            }
        }

        private static DateTime ConvertUlongToDateTime(double when) => DateTime.Now.AddMilliseconds(when);

        private static Func<Task<DateTime?>> WrapTask(Func<Task> task) =>
            async () =>
            {
                await task.Invoke();
                return null;
            };

        private static Func<Task<DateTime?>> WrapTask(Func<Task<double?>> task) =>
            async () =>
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