using Grpc.Core;
using grpcServer;
using grpcMessageServer;

namespace grpcServer.Services;

public class MessageService : Message.MessageBase
{
    //Server Streming
    // public override async Task SendMessage(MessageRequest request, IServerStreamWriter<MessageResponse> responseStream, ServerCallContext context)
    // {
    //     System.Console.WriteLine($"Message : {request.Message} | Name : {request.Name}");

    //     for (int i = 0; i < 10; i++)
    //     {
    //         await Task.Delay(1000);
    //         await responseStream.WriteAsync(new MessageResponse {
    //             Message = "Merhaba" + i
    //         });
    //     }
    // }

    //Client Streaming

    // public override async Task<MessageResponse> SendMessage(IAsyncStreamReader<MessageRequest> requestStream, ServerCallContext context)
    // {
    //     while(await requestStream.MoveNext(context.CancellationToken)) {
    //         System.Console.WriteLine($"Message : {requestStream.Current.Message} | Name : {requestStream.Current.Name}");
    //     }

    //     return new MessageResponse {
    //         Message = "Veri alınmıştır."
    //     };
    // }

    //Bi-directional streaming
    public override async Task SendMessage(IAsyncStreamReader<MessageRequest> requestStream, IServerStreamWriter<MessageResponse> responseStream, ServerCallContext context)
    {
        var task1 = Task.Run(async () =>
        {
            while (await requestStream.MoveNext(context.CancellationToken))
            {
                System.Console.WriteLine($"Message : {requestStream.Current.Message} | Name : {requestStream.Current.Name}");
            }
        });

        for (int i = 0; i < 10; i++)
        {
            await Task.Delay(1000);
            await responseStream.WriteAsync(new MessageResponse { Message = "Mesaj " + i});
        }

        await task1;

    }
}
