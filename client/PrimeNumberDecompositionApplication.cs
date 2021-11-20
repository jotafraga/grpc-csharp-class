using System;
using System.Threading.Tasks;
using Grpc.Core;
using Prime;

namespace client
{
    public class PrimeNumberDecompositionApplication
    {
        private static PrimeNumberService.PrimeNumberServiceClient _client;
        
        public PrimeNumberDecompositionApplication(ChannelBase channel)
        {
            _client = new PrimeNumberService.PrimeNumberServiceClient(channel);
        }

        public async Task ServerStreaming()
        {
            Console.WriteLine("START -- SERVER STREAMING");
            var request = new PrimeNumberDecompositionRequest() { Number = 120 };
            
            var response = _client.PrimeNumberDecomposition(request);
            
            while (await response.ResponseStream.MoveNext())
            {
                Console.WriteLine(response.ResponseStream.Current.PrimeFactor);
                await Task.Delay(200);
            }
            Console.WriteLine("END -- SERVER STREAMING\n");
        }
    }
}