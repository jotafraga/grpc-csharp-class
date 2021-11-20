using Greet;
using Grpc.Core;
using System;
using System.IO;
using Prime;
using Average;
using Max;
using Sqrt;
using System.Collections.Generic;
using Grpc.Reflection;
using Grpc.Reflection.V1Alpha;

namespace server
{
    class Program
    {
        private const int Port = 50051;

        static void Main(string[] args)
        {
            Server server = null;

            try
            {
                var serverCert = File.ReadAllText("ssl/server.crt");
                var serverKey = File.ReadAllText("ssl/server.key");
                var keyPair = new KeyCertificatePair(serverCert, serverKey);
                var caCrt = File.ReadAllText("ssl/ca.crt");

                var credentials = new SslServerCredentials(new List<KeyCertificatePair>() { keyPair }, caCrt, true);

                var reflectionServiceImpl = new ReflectionServiceImpl(
                    GreetingService.Descriptor,
                    PrimeNumberService.Descriptor,
                    AverageService.Descriptor,
                    FindMaxService.Descriptor,
                    SqrtService.Descriptor,
                    ServerReflection.Descriptor);

                server = new Server()
                {
                    Services = { 
                        GreetingService.BindService(new GreetingServiceImpl()), 
                        PrimeNumberService.BindService(new PrimeNumberServiceImpl()),
                        AverageService.BindService(new AverageServiceImpl()),
                        FindMaxService.BindService(new FindMaxServiceImpl()),
                        SqrtService.BindService(new SqrtServiceImpl()),
                        ServerReflection.BindService(reflectionServiceImpl)
                    },
                    Ports = {new ServerPort("localhost", Port, ServerCredentials.Insecure) } //credentials) } -- TO USE CREDENTIALS, IT'S NECESSARY TO REMOVE REFLECTION
                };

                server.Start();
                Console.WriteLine("The server is listening on the port: " + Port);
                Console.ReadKey();
            }
            catch (IOException e)
            {
                Console.WriteLine("The server failed to start: " + e.Message);
                throw;
            }
            finally
            {
                if (server != null)
                {
                    server.ShutdownAsync().Wait();
                }
            }
        }
    }
}