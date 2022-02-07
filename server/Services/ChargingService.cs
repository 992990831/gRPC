using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace server
{
    public class ChargingService : Charging.ChargingBase
    {
        public override async Task ReportProgress(
              ProgressRequest request, 
              IServerStreamWriter<ProgressResponse> responseStream, 
              ServerCallContext context)
        {
            int percentage = 0;           
            try
            {                
                while (!context.CancellationToken.IsCancellationRequested)
                {
                    Thread.Sleep(5000);
                    percentage += 20;
                    await responseStream.WriteAsync(new ProgressResponse { 
                     Percentage = percentage
                    });
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                Console.WriteLine("Operation Cancelled.");
            }

            Console.WriteLine("Processing Complete.");
        }
    }
}
