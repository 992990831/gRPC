using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using client;

namespace client
{
    class Program
    {
        static void Main(string[] args)
        {
            GetServerProgress();
        }

        static void GetServerProgress()
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new client.Charging.ChargingClient(channel); //GrpcGreeterClient.Greeter.GreeterClient(channel);
            var request = new client.ProgressRequest { Continue = true };
            var cancellationToken = new CancellationTokenSource();

            try
            {
                AsyncServerStreamingCall<ProgressResponse> response = 
                    client.ReportProgress(request, 
                      cancellationToken: cancellationToken.Token);

                // loop through each object from the ResponseStream
                while (response.ResponseStream.MoveNext().Result)
                {
                    // fetch the object currently pointed
                    var current = response.ResponseStream.Current;
                    if(current.Percentage >= 100)
                    {
                        cancellationToken.Cancel();
                    }

                    // print it
                    Console.WriteLine($"{current.Percentage}%");
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                Console.WriteLine("Operation Cancelled.");
            }

            Console.ReadLine();
        }
    }
}
