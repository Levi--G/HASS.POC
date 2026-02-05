using SharpSchedule;

namespace HASSPOC.Services
{
    internal class SchedulingService : IDisposable
    {
        private Scheduler scheduler = new() { Precision = 5000, MinPrecision = 60000 };
        //private List<ScheduledTask> tasks = [];

        public SchedulingService()
        {
            scheduler.OnRunningBehind += Scheduler_OnRunningBehind;
            scheduler.OnError += Scheduler_OnError;
        }

        private void Scheduler_OnError(object? sender, Exception e)
        {
            //to log
        }

        private void Scheduler_OnRunningBehind(object? sender, TimeSpan e)
        {
            //to log
        }

        public ScheduledTask Schedule(Func<CancellationToken, ValueTask> ToRun, DateTime Start, TimeSpan? Interval = null)
        {
            var task = new ScheduledTask(scheduler, ToRun, Start, Interval);
            //tasks.Add(task);
            return task;
        }

        public ScheduledTask Schedule(Action<CancellationToken> ToRun, DateTime Start, TimeSpan? Interval = null)
        {
            var task = new ScheduledTask(scheduler, (c) => { ToRun(c); return ValueTask.CompletedTask; }, Start, Interval);
            //tasks.Add(task);
            return task;
        }

        public void Start()
        {
            scheduler.StartThread();
        }

        public void Stop()
        {
            scheduler.SignalStop();
        }

        public void Dispose()
        {
            scheduler.StopAndBlock();
        }
    }

    internal class ScheduledTask
    {
        private readonly ScheduleItem scheduleItem;
        CancellationTokenSource cts = new();
        Func<CancellationToken, ValueTask> ToRun;
        public bool IsRunning { get; private set; }
        public event Action<Exception>? OnException;

        public ScheduledTask(Scheduler scheduler, Func<CancellationToken, ValueTask> ToRun, DateTime Start, TimeSpan? Interval = null)
        {
            scheduleItem = new(Run, Start, Interval) { CanSkip = false };
            scheduler.Schedule(scheduleItem);
            this.ToRun = ToRun;
        }

        async Task Run()
        {
            if (cts.IsCancellationRequested)
            {
                return;
            }
            if (IsRunning)
            {
                cts.Cancel();
                cts = new CancellationTokenSource();
            }
            try
            {
                IsRunning = true;
                await ToRun(cts.Token);
            }
            catch (Exception ex)
            {
                OnException?.Invoke(ex);
            }
            finally
            {
                IsRunning = false;
            }
        }

        public void Cancel()
        {
            cts.Cancel();
            scheduleItem.Dispose();
        }
    }
}
