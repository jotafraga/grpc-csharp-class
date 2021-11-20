using Grpc.Core;
using Sqrt;
using System;

namespace client
{
    class SqrtApplication
    {
        private static SqrtService.SqrtServiceClient _client;

        public SqrtApplication(ChannelBase channel)
        {
            _client = new SqrtService.SqrtServiceClient(channel);
        }

        public void Unary()
        {
            int number = 16;
            //int errorNumber -1;

            try
            {
                var response = _client.Sqrt(new SqrtRequest() { Number = number });

                Console.WriteLine(response.SquareRoot);
            } 
            catch(RpcException e)
            {
                Console.WriteLine("Error: " + e.Status.Detail);
            }
        }
    }
}
