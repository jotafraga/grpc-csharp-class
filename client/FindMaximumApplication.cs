using System;
using System.Linq;
using System.Threading.Tasks;
using Max;
using Grpc.Core;

namespace client
{
    public class FindMaximumApplication
    {
        private static FindMaxService.FindMaxServiceClient _client;

        public FindMaximumApplication(ChannelBase channel)
        {
            _client = new FindMaxService.FindMaxServiceClient(channel);
        }

        public async Task BiDirectional()
        {
            var stream = _client.FindMaximum();

            var responseReaderTask = Task.Run(async () =>
            {
                while (await stream.ResponseStream.MoveNext())
                {
                    Console.WriteLine("Received: " + stream.ResponseStream.Current.Max);
                }
            });

            int[] numbers = { 1, 5, 3, 6, 2, 20 };

            foreach (var number in numbers)
            {
                Console.WriteLine("Sending: " + number);
                await stream.RequestStream.WriteAsync(new FindMaxRequest
                {
                    Number = number
                });
            }

            await stream.RequestStream.CompleteAsync();
            await responseReaderTask;
        }
    }
}
