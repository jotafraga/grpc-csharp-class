using System;
using System.IO;
using System.Threading.Tasks;
using Grpc.Core;

namespace client
{
    class Program
    {
        private const string Target = "127.0.0.1:50051";

        private static async Task Main(string[] args)
        {
            var clientCert = File.ReadAllText("ssl/client.crt");
            var clientKey = File.ReadAllText("ssl/client.key");
            var caCrt = File.ReadAllText("ssl/ca.crt");

            var channelCredntials = new SslCredentials(caCrt, new KeyCertificatePair(clientCert, clientKey));

            var channel = new Channel("localhost", 50051, channelCredntials);

            await channel.ConnectAsync().ContinueWith((task) =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                    Console.WriteLine("The client connected successfully");
            });

            var greetingApplication = new GreetingApplication(channel);
            //greetingApplication.Unary();
            //await greetingApplication.ServerStreaming();
            //await greetingApplication.ClientStreaming();
            //await greetingApplication.BiDirectional();
            greetingApplication.UnaryWithDeadLine();

            var primeNumberDecompositionApplication = new PrimeNumberDecompositionApplication(channel);
            //await primeNumberDecompositionApplication.ServerStreaming();

            var averageApplication = new AverageApplication(channel);
            //await averageApplication.ClientStreaming();

            var findMaxApplication = new FindMaximumApplication(channel);
            //await findMaxApplication.BiDirectional();

            var sqrtApplication = new SqrtApplication(channel);
            //sqrtApplication.Unary();

            channel.ShutdownAsync().Wait();
            Console.ReadKey();
        }
    }
}