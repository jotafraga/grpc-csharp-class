using System;
using System.Linq;
using System.Threading.Tasks;
using Greet;
using Grpc.Core;

namespace client
{
    public class GreetingApplication
    {
        private static GreetingService.GreetingServiceClient _client;
        
        public GreetingApplication(ChannelBase channel)
        {
            _client = new GreetingService.GreetingServiceClient(channel);
        }
        
        private static readonly Greeting Greeting = new Greeting()
        {
            FirstName = "João",
            LastName = "Fraga"
        };

        public void Unary()
        {
            var request = new GreetingRequest { Greeting = Greeting };
            var response = _client.Greet(request);
            Console.WriteLine(response.Result);
        }
        
        public async Task ServerStreaming()
        {
            var request = new GreetManyTimesRequest() {Greeting = Greeting};
            
            var response = _client.GreetManyTimes(request);

            while (await response.ResponseStream.MoveNext())
            {
                Console.WriteLine(response.ResponseStream.Current.Result);
                await Task.Delay(200);
            }
        }
        public async Task ClientStreaming()
        {
            var request = new LongGreetRequest {Greeting = Greeting};
            
            var stream = _client.LongGreet();

            foreach (var i in Enumerable.Range(1, 10))
            {
                await stream.RequestStream.WriteAsync(request);
            }

            await stream.RequestStream.CompleteAsync();

            var response = await stream.ResponseAsync;

            Console.WriteLine(response.Result);
        }

        public async Task BiDirectional()
        {
            var stream = _client.GreetEveryone();

            var responseReaderTask = Task.Run(async () =>
            {
                while (await stream.ResponseStream.MoveNext())
                {
                    Console.WriteLine("Received: " + stream.ResponseStream.Current.Result);
                }
            });

            Greeting[] greetings =
            {
                new Greeting {FirstName = "John", LastName = "Fraga"},
                new Greeting {FirstName = "Joãozinho", LastName = "Fragoso"},
                new Greeting {FirstName = "Jota", LastName = "Fraga"},
                new Greeting {FirstName = "Jotinha", LastName = "Fraguinha"},
                new Greeting {FirstName = "Ronaldinho", LastName = "Gaúcho"}
            };

            foreach (var greeting in greetings)
            {
                Console.WriteLine("Sending: " + greeting);
                await stream.RequestStream.WriteAsync(new GreetEveryoneRequest
                {
                    Greeting = greeting
                });
            }

            await stream.RequestStream.CompleteAsync();
            await responseReaderTask;
        }

        public void UnaryWithDeadLine()
        {
            try
            {
                var request = new GreetingRequest { Greeting = Greeting };
                var response = _client.Greet(request, deadline: DateTime.UtcNow.AddMilliseconds(300));
                //var response = _client.Greet(request, deadline: DateTime.UtcNow.AddMilliseconds(100)); -- DEADLINE

                Console.WriteLine(response.Result);
            }
            catch(RpcException e) when (e.StatusCode == StatusCode.DeadlineExceeded)
            {
                Console.WriteLine("Error: " + e.Status.Detail);
            }
        }
    }
}