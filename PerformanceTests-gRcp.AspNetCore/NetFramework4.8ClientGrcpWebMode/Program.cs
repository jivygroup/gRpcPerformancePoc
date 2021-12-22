﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using StreamingApi.Protos;

namespace SCM_GrpcNwtFramework
{
    partial class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine();
            Arguments arguments = new Arguments(args);

            if (arguments.Initialized)
            {
                string limitationMsg = arguments.UseLimiter ?
                    $"limitation of max {arguments.MaxRequestsPerTimeUnit} requests per {arguments.TimeUnitSecs} seconds":
                    "no limitation";
                Console.WriteLine($"\nExecuting {arguments.TotalRequests} requests with StreamMode={arguments.IsStreamMode} using {arguments.Threads} threads with {limitationMsg} ...");

                var result = Run(arguments.Host, arguments.Port, arguments.Threads, arguments.TotalRequests, arguments.MaxRequestsPerTimeUnit, TimeSpan.FromSeconds(arguments.TimeUnitSecs), arguments.UseLimiter, arguments.IsStreamMode).Result;
                
                Console.WriteLine($"Time Taken = {result.Item1}, Total Request Calls = {result.Item2}, Total Errors: {result.Item3}\n");
                //grpcClient.Disconnect().Wait();
                Thread.Sleep(1000);
            }

            Console.ReadKey();
        }

        private static ILightStreamerProxyClient GetGrpcClient(string host, int port, ChannelCredentials channelCredentials)
        {
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWebText, new HttpClientHandler());
            var channel = GrpcChannel.ForAddress($"http://{host}:{port}", new GrpcChannelOptions
            {
                HttpClient = new HttpClient(handler)
            });

            var channelClient = new StreamingApi.Protos.StreamingApi.StreamingApiClient(channel);
            return new LightStreamerProxyClient(channelClient);
        }

        private static async Task<(TimeSpan, long, long)> Run(
            string host,
            int port,
            int threads,
            int requests,
            int maxRequestsPerTimeUnit,
            TimeSpan timeUnit,
            bool useLimiter,
            bool isStreamMode)
        {
            long totalRequestCalls = 0;
            long totalErrors = 0;

            List<Task> tasks = new List<Task>();
            int requestsPerThread = requests / threads;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            InHouseThrottler throttler = new InHouseThrottler(maxRequestsPerTimeUnit, timeUnit);

            for (int i = 0; i < threads; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    int totalThreadCalls = 0;
                    ILightStreamerProxyClient grpcClient = GetGrpcClient(host, port, ChannelCredentials.Insecure);
                    UpdateRequest request = GetMeasuredRequest();

                    System.Timers.Timer timer = new System.Timers.Timer(5000);
                    timer.Elapsed += delegate (object sender, ElapsedEventArgs e)
                    {
                        Console.WriteLine($"TID: {Thread.CurrentThread.ManagedThreadId} total calls: {totalThreadCalls}");
                    };
                    timer.Start();

                    for (int requestIndex = 0; requestIndex < requestsPerThread; requestIndex++)
                    {
                        if (useLimiter)
                        {
                            throttler.Increment();
                        }
                        request.Items["time"] = DateTime.Now.Ticks.ToString();
                        try
                        {
                            if (isStreamMode)
                            {
                               await grpcClient.SendUpdateStream(request);
                            }
                            else
                            {
                                _ = await grpcClient.SendUpdate(request);
                            }
                            Interlocked.Increment(ref totalRequestCalls);
                            totalThreadCalls++;
                        }
                        catch (Exception ex)
                        {
                            Interlocked.Increment(ref totalErrors);
                            Console.WriteLine(ex.Message);
                            Thread.Sleep(500);
                        }
                    }
                    timer.Stop();
                    Console.WriteLine($"TID: {Thread.CurrentThread.ManagedThreadId} total calls: {totalThreadCalls}");
                }));
            }
            await Task.WhenAll(tasks);

            sw.Stop();
            return (sw.Elapsed, totalRequestCalls, totalErrors);
        }


        private static UpdateRequest GetMeasuredRequest()
        {
            UpdateRequest request = new UpdateRequest { ItemName = "pattern", SubcriptionId = "subscriptionId", IsSnapshot = false};
            request.Items["key1"] = "value1";
            request.Items["key2"] = "value2";
            request.Items["key3"] = "value3";
            request.Items["key4"] = "value4";
            request.Items["key5"] = "value5";

            return request;
        }

    }
}
