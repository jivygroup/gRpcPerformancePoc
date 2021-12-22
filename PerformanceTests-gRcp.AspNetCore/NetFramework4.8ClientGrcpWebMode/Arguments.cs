using System;
using System.IO;
using CommandLineParser.Arguments;

namespace SCM_GrpcNwtFramework
{
    partial class Program
    {
        public class Arguments
        {
            public Arguments(string[] args)
            {
                InitializeArgs(args);
            }

            private void InitializeArgs(string[] args)
            {
                Initialized = false;

                CommandLineParser.CommandLineParser parser = new CommandLineParser.CommandLineParser();
                ValueArgument<string> host = new ValueArgument<string>('h', nameof(host), "target host") { Optional = false };
                ValueArgument<int> port = new ValueArgument<int>('p', nameof(port), "target host's port") { Optional = false };
                ValueArgument<int> requests = new ValueArgument<int>('r', nameof(requests), "total number of requests to execute") { Optional = false };
                ValueArgument<int> threads = new ValueArgument<int>('t', nameof(threads), "Number of threads (total requests will be split evenly between them)") { Optional = false };
                ValueArgument<bool> isStreamMode = new ValueArgument<bool>('s', nameof(isStreamMode), "Is stream mode") { Optional = false };
                ValueArgument<bool> useLimiter = new ValueArgument<bool>('l', nameof(useLimiter), "Use requests limiter") { Optional = false };
                ValueArgument<int> timeUnit = new ValueArgument<int>('u', nameof(timeUnit), "Time unit in seconds") { Optional = !useLimiter.Value };
                ValueArgument<int> maxRequestsPerTimeUnit = new ValueArgument<int>('m', nameof(maxRequestsPerTimeUnit), "Maximum number of requests per time unit") { Optional = !useLimiter.Value };

                parser.Arguments.Add(host);
                parser.Arguments.Add(port);
                parser.Arguments.Add(isStreamMode);
                parser.Arguments.Add(requests);
                parser.Arguments.Add(threads);
                parser.Arguments.Add(useLimiter);
                parser.Arguments.Add(maxRequestsPerTimeUnit);
                parser.Arguments.Add(timeUnit);

                try
                {
                    parser.ParseCommandLine(args);
                    Host = host.Value;
                    Port = port.Value;
                    TotalRequests = requests.Value;
                    Threads = threads.Value;
                    MaxRequestsPerTimeUnit = maxRequestsPerTimeUnit.Value;
                    TimeUnitSecs = timeUnit.Value;
                    UseLimiter = useLimiter.Value;
                    IsStreamMode = isStreamMode.Value;
                    ValidateArgs();
                    Initialized = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    parser.ShowUsage();
                }
            }

            private void ValidateArgs()
            {
                if ((UseLimiter) && ((TimeUnitSecs <= 0) || MaxRequestsPerTimeUnit <= 0))
                {
                    throw new InvalidDataException("Invalid input - useLimiter is true but includes invalid data");
                }
            }

            public bool Initialized { get; private set; }

            public string Host { get; set; }
            public int Port { get; set; }
            public bool IsStreamMode { get; set; }
            public bool UseLimiter { get; set; }
            public int TotalRequests { get; set; }
            public int Threads { get; set; }
            public int MaxRequestsPerTimeUnit { get; set; }
            public int TimeUnitSecs { get; set; }
        }
    }
}
