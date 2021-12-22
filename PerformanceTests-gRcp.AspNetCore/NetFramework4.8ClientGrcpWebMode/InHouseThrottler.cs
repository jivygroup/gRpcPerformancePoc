using System;
using System.Threading;

namespace SCM_GrpcNwtFramework
{
    public class InHouseThrottler
    {
        long _maxCallsForTimeLimit;
        TimeSpan _timeLimit;

        DateTime _lastSetTime;
        long _currentCallsCount;
        private static readonly object _callsLock = new object();

        public InHouseThrottler(int maxCallsForTimeLimit, TimeSpan timeLimit)
        {
            _maxCallsForTimeLimit = maxCallsForTimeLimit;
            _timeLimit = timeLimit;
            _lastSetTime = DateTime.Now;
        }
        public void Increment()
        {
            long counter = Interlocked.Read(ref _currentCallsCount);
            if (counter >= _maxCallsForTimeLimit)
            {
                lock(_callsLock)
                {
                    if (_currentCallsCount >= _maxCallsForTimeLimit)
                    {
                        TimeSpan wait = _timeLimit - DateTime.Now.Subtract(_lastSetTime);
                        if (wait.TotalMilliseconds > 0)
                        {
                            Thread.Sleep(wait);
                        }
                        Interlocked.Exchange(ref _currentCallsCount, 0);
                        _lastSetTime = DateTime.Now;
                    }
                }
            }
            Interlocked.Increment(ref _currentCallsCount);
        }
    }
}
