using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LSProxyService_Grpc_console
{
    public class GrpcStatisticsDataProcessor : IGrpcStatisticsDataProcessor
    {
        private IGrpcServiceStatisticalData _serviceStatisticalData;
        private int _displayIntervalMs;

        public GrpcStatisticsDataProcessor(IGrpcServiceStatisticalData serviceStatisticalData)
        {
            _serviceStatisticalData = serviceStatisticalData;
            _displayIntervalMs = 1000;
        }

        public void Start()
        {
            string filename = $"{DateTime.Now.ToString("yyyy-MM-dd_HH-MM-ss")}.csv";
            File.WriteAllText(filename, "Requests,Average Latency,Minimum Latency,Maximum Latency\n");
            Task.Run(() =>
            {
                long handledRequests = 0;
                long timeDiff = 0;
                long minLatency = 0;
                long maxLatency = 0;

                while (true)
                {
                    Thread.Sleep(_displayIntervalMs);
                    _serviceStatisticalData.EnterWrite();

                    try
                    {
                        handledRequests = _serviceStatisticalData.HandledRequests;
                        timeDiff = _serviceStatisticalData.TimeDiff;
                        minLatency = _serviceStatisticalData.MinLatency;
                        maxLatency = _serviceStatisticalData.MaxLatency;
                        _serviceStatisticalData.HandledRequests = _serviceStatisticalData.TimeDiff = _serviceStatisticalData.MaxLatency = 0;
                        _serviceStatisticalData.MinLatency = long.MaxValue;
                    }
                    finally
                    {
                        _serviceStatisticalData.ExitWrite();
                    }

                    if (handledRequests > 0)
                    {
                        File.AppendAllText(filename, $"{handledRequests},{timeDiff / handledRequests},{minLatency},{maxLatency}\n");
                        Console.WriteLine($"requests: {handledRequests}, average latency: {timeDiff / handledRequests}, min latency: {minLatency}, max latency: {maxLatency}");
                    }
                }
            });
        }
    }
}
