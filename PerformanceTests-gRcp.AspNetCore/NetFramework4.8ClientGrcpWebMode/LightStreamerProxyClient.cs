﻿using System.Threading.Tasks;
using Grpc.Core;
using StreamingApi.Protos;

namespace SCM_GrpcNwtFramework
{
    public class LightStreamerProxyClient : ILightStreamerProxyClient
    {
        private Channel _channel;
        private StreamingApi.Protos.StreamingApi.StreamingApiClient _client;
        private AsyncClientStreamingCall<UpdateRequest, UpdateResponse> _updateRequestStreamWriter;

        public LightStreamerProxyClient(StreamingApi.Protos.StreamingApi.StreamingApiClient client)
        {
            _client = client;
            _updateRequestStreamWriter = client.UpdateStream();

        }

        public async Task Disconnect()
        {
            await _channel.ShutdownAsync();
        }

        public async Task<UpdateResponse> SendUpdate(UpdateRequest request)
        {
            return await _client.UpdateAsync(request);
        }

        public async Task SendUpdateStream(UpdateRequest request)
        {
            _client.Update(request);

            await _updateRequestStreamWriter.RequestStream.WriteAsync(request);
        }
    }
}