using Grpc.Core;
using Prime;
using System.Threading.Tasks;
using static Prime.PrimeNumberService;

namespace server
{
    public class PrimeNumberServiceImpl : PrimeNumberServiceBase
    {
        public override async Task PrimeNumberDecomposition(PrimeNumberDecompositionRequest request, IServerStreamWriter<PrimeNumberDecompositionResponse> responseStream, ServerCallContext context)
        {
            int number = request.Number;
            int divisor = 2;

            while(number > 1)
            {
                if(number % divisor == 0)
                {
                    number /= divisor;
                    await responseStream.WriteAsync(new PrimeNumberDecompositionResponse() { PrimeFactor = divisor});
                } 
                else
                {
                    divisor++;
                }
            }
        }
    }
}