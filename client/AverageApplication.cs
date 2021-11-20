using System;
using System.Linq;
using System.Threading.Tasks;
using Average;
using Grpc.Core;

namespace client
{
    public class AverageApplication
    {
        private static AverageService.AverageServiceClient _client;
        
        public AverageApplication(ChannelBase channel)
        {
            _client = new AverageService.AverageServiceClient(channel);
        }
        
        public async Task ClientStreaming()
        {
            Console.WriteLine("START -- CLIENT STREAMING");
            
            var stream = _client.ComputeAverage();

            foreach (var number in Enumerable.Range(1, 4))
            {
                var request = new AverageRequest {Number = number};
                
                await stream.RequestStream.WriteAsync(request);
            }

            await stream.RequestStream.CompleteAsync();

            var response = await stream.ResponseAsync;

            Console.WriteLine(response.Result);
            Console.WriteLine("END -- CLIENT STREAMING\n");
        }
    }
}