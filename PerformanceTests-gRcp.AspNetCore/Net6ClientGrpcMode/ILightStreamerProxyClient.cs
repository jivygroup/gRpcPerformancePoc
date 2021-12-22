using StreamingApi.Protos;

namespace SCM_GrpcNetCore
{
    public interface ILightStreamerListener
    {
        void Start();
        void Stop();
    }

    public interface ILightStreamerProxyClient
    {
        Task<UpdateResponse> SendUpdate(UpdateRequest request);
        Task SendUpdateStream(UpdateRequest request);
        Task Disconnect();
    }
}