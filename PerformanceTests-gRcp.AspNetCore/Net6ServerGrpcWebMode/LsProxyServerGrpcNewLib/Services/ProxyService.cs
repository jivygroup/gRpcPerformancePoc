using Grpc.Core;
using StreamingApi.Protos;

namespace LsProxyServerGrpcNewLib.Services
{
    public class ProxyService : StreamingApi.Protos.StreamingApi.StreamingApiBase
    {
        private readonly IGrpcServiceStatisticalData _grpcServiceStatisticalData;

        public ProxyService(IGrpcServiceStatisticalData grpcServiceStatisticalData)
        {
            _grpcServiceStatisticalData = grpcServiceStatisticalData;
        }

        public override Task<UpdateResponse> Update(UpdateRequest request, ServerCallContext context)
        {
            HandleUpdateRequest(request);
            return Task.FromResult(new UpdateResponse());
        }
        public override async Task<UpdateResponse> UpdateStream(IAsyncStreamReader<UpdateRequest> requestStream, ServerCallContext serverCallContext)
        {
            try
            {
                while (await requestStream.MoveNext(serverCallContext.CancellationToken))
                {
                    var request = requestStream.Current;
                    HandleUpdateRequest(request);
                }
            }
            catch(OperationCanceledException ex)
            {
                Console.WriteLine($"error during UpdateStream handling: {ex.Message}");
            }
            return new UpdateResponse();
        }

        private void HandleUpdateRequest(UpdateRequest request)
        {
            try
            {
                var currentLatency = (long)DateTime.Now.Subtract(new DateTime(long.Parse(request.Items["time"]))).TotalMilliseconds;

                _grpcServiceStatisticalData.EnterWrite();
                try
                {
                    if (currentLatency > _grpcServiceStatisticalData.MaxLatency)
                    {
                        _grpcServiceStatisticalData.MaxLatency = currentLatency;
                    }
                    else if (currentLatency < _grpcServiceStatisticalData.MinLatency)
                    {
                        _grpcServiceStatisticalData.MinLatency = currentLatency;
                    }
                    _grpcServiceStatisticalData.TimeDiff += currentLatency;
                    _grpcServiceStatisticalData.HandledRequests++;
                }
                finally
                {
                    _grpcServiceStatisticalData.ExitWrite();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error serializing datetime: {ex.Message}");
                Thread.Sleep(1000);
                throw;
            }
        }
    }
}