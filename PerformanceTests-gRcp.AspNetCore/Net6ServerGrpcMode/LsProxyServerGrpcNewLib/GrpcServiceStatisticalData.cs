namespace LsProxyServerGrpcNewLib
{
    public class GrpcServiceStatisticalData : IGrpcServiceStatisticalData
    {
        private static ReaderWriterLockSlim? _dataAccessLock;

        public GrpcServiceStatisticalData()
        {
            _dataAccessLock = new ReaderWriterLockSlim();
        }
        public long HandledRequests { get; set; }
        public long TimeDiff { get; set; }
        public long MinLatency { get; set; }
        public long MaxLatency { get; set; }

        public void EnterWrite()
        {
            _dataAccessLock?.EnterWriteLock();
        }

        public void ExitWrite()
        {
            _dataAccessLock?.ExitWriteLock();
        }
    }
}
