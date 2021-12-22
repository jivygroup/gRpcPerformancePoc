# gRpcPerformancePoc
A simple server and client applications to test gRcp performance 

To run the serevr just double click on it or run it from VS.

To run the client you need to pass to it a few command line arguments

for example : -h localhost -p 23456 -s true -r 1000000 -t 1 -l false (you can just copy paste this line)

The meaning of each argument is : 


        -h, --host... target host

        -p, --port... target host's port

        -s, --isStreamMode... Is stream mode

        -r, --requests... total number of requests to execute

        -t, --threads... Number of threads (total requests will be split evenly between them)

        -l, --useLimiter... Use requests limiter

        -m, --maxRequestsPerTimeUnit[optional]... Maximum number of requests per time unit

        -u, --timeUnit[optional]... Time unit in seconds
