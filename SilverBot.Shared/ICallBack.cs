namespace SilverBot.Shared
{
    /// <summary>
    /// A simple callback interface, a timer of sorts
    /// </summary>
    public interface ICallBack
    {
        /// <summary>
        /// Adds a Callback to the current queue
        /// </summary>
        /// <param name="task">the task to run that returns if and when to run it next, return null if it does not need to be called again</param>
        /// <param name="when">when to first run the task</param>
        public void Add(Func<Task<DateTime?>> task, DateTime when, string? name = null,CancellationTokenSource? source=null);
        
        public void AddOnce(Func<Task> task, DateTime when, string? name = null, CancellationTokenSource? source=null);
        /// <summary>
        /// Adds a Callback to the current queue, double overload which takes millisecond offsets
        /// </summary>
        /// <param name="task">the task to run that returns if and when to run it next, return null, NAN, +inf, -inf, if it does not need to be called again</param>
        /// <param name="when">when to first run the task</param>
        public void Add(Func<Task<double?>> task, double when, string? name = null,CancellationTokenSource? source=null);
        /// <summary>
        /// Gets the number of scheduled callbacks
        /// </summary>
        public int NumberOfCallBacks { get; }
        
        /// <summary>
        /// Gets the next callback's event time, null or MaxValue if empty, can be the same as LastCallBackTime
        /// </summary>
        public DateTime? NextCallBackTime { get; }
        /// <summary>
        /// Gets the last callback's event time, null or MinValue if empty, can be the same as NextCallBackTime
        /// </summary>
        public DateTime? LastCallBackTime { get; }
        public int RunningTaskCount { get; }

    }
}