using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using TR.AppServer.Common.Interfaces;
using TR.AppServer.Logging;
using Timer = System.Timers.Timer;

namespace SystemTestService
{
    public class ConcurrentFlushQueue<T> : IDisposable
    {
        private volatile bool _flushing;
        private volatile bool _stopping;
        private readonly int _countThreshold;
        private volatile int _flushQueueCount;
        private volatile ConcurrentQueue<T> _flushQueue;
        private static readonly object FlipLock = new object();

        private readonly ILogger _logger;

        /// <summary>
        /// Flush interval timer
        /// </summary>
        private readonly Timer _timer;

        private event EventHandler<FlushQueueThresholdBreachedEventArgs<T>> ThresholdBreached;

        public ConcurrentFlushQueue(int countThreashold, TimeSpan timeThreshold)
        {
            _flushQueue = new ConcurrentQueue<T>();
            _countThreshold = countThreashold;
            _logger = Logger.Default;
            _timer = new Timer
            {
                Interval = timeThreshold.TotalMilliseconds,
                Enabled = true,
            };
            _timer.Elapsed += _timer_Elapsed;
        }

        public int Count
        {
            get { return _flushQueueCount; }
        }

        public bool Ready
        {
            get { return _flushQueueCount < _countThreshold; }
        }


        protected virtual void OnThresholdBreached(FlushQueueThresholdBreachedEventArgs<T> e)
        {
            var handler = ThresholdBreached;
            if (handler != null) handler(this, e);
        }

        public void RegisterFlushHandler(EventHandler<FlushQueueThresholdBreachedEventArgs<T>> handler)
        {
            ThresholdBreached += handler;
        }

        public void EnQueue(T item, int count = 1)
        {
            if (_stopping)
            {
                return;
            }

            _flushQueue.Enqueue(item);

            // double-checked lock - if Q is full and no flush in progress, flip and flush
            if (!_flushing && _flushQueueCount >= _countThreshold)
            {
                lock (FlipLock)
                {
                    if (_flushQueueCount >= _countThreshold)
                    {
                        FlushAsyncInternal();
                    }
                }
            }
            else //flush is in progress OR we still got room, just increment the count
            {
                // it's possible this count will be a bit off by 1 or 2 items, but whatever, not a big deal
                Interlocked.Add(ref _flushQueueCount, count);
            }
        }

        private void FlushAsyncInternal()
        {
            _flushing = true;
            _timer.Stop(); // we don't want the timer going off while we're already flushing

            // flip the Q
            var oldQ = _flushQueue;
            _flushQueue = new ConcurrentQueue<T>();
            Interlocked.Exchange(ref _flushQueueCount, 0);

            var eventArgs = new FlushQueueThresholdBreachedEventArgs<T>(oldQ);

            // schedule run on separate thread and release lock
            Task.Run(() =>
            {
                try
                {
                    OnThresholdBreached(eventArgs);
                }
                catch (Exception ex)
                {
                    _logger.LogError("ConcurrentFlushQ - FlushHandler exception {0}", ex.Message);
                }
                finally
                {
                    _flushing = false;
                    _timer.Start();
                }
            });
        }

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_stopping || _flushing)
            {
                return;
            }

            FlushAsyncInternal();
        }

        public void Dispose()
        {
            _timer.Stop();
            _stopping = true;

            // flush any items pending
            var eventArgs = new FlushQueueThresholdBreachedEventArgs<T>(_flushQueue);
            try
            {
                OnThresholdBreached(eventArgs);
            }
            finally
            {
                _timer.Dispose(); // release resources held by timer
            }
        }

        public void UpdateTimerInterval(TimeSpan interval)
        {
            var ms = interval.TotalMilliseconds;
            if (ms > 0)
            {
                _timer.Interval = ms;
            }
            else
            {
                _logger.LogWarn("ConcurrentFlushQ.UpdateTimerInterval - attempt to update timer interval failed - value: {0}ms", ms);
            }
        }
    }

    public class FlushQueueThresholdBreachedEventArgs<T> : EventArgs
    {
        public ConcurrentQueue<T> ItemsToFlush { get; private set; }

        public FlushQueueThresholdBreachedEventArgs(ConcurrentQueue<T> items)
        {
            ItemsToFlush = items ?? new ConcurrentQueue<T>();
        }
    }
}
