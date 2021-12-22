using StreamingApi.Protos;
using System.Threading.Tasks;

namespace SCM_Grpc_Console
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