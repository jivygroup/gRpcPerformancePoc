namespace LsProxyServerGrpcNewLib
{
    public interface IGrpcServiceStatisticalData
    {
        long HandledRequests { get; set; }
        long TimeDiff { get; set; }
        long MinLatency { get; set; }
        long MaxLatency { get; set; }

        public void EnterWrite();

        public void ExitWrite();
    }
}
