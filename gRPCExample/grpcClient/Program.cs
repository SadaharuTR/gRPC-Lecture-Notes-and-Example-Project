using Grpc.Net.Client;
using grpcMessageClient;
using grpcServer;

//Öncelikle bu iletişim sağlayacak bir channel oluşturmamız lazım.
var channel = GrpcChannel.ForAddress("https://localhost:5001"); //server-launchsettings.json'dan değiştirilebilir.

//Server 5001'de ayağa kalktı. Buradaki gRPC servisine bağlan demiş olduk.

//Şimdi Client'ı oluşturalım.
//var greetClient = new Greeter.GreeterClient(channel);
//Artık bu greetClient üzerinden gerekli request ve response işlemlerini yapacağız.

//Request gönder

/*
HelloReply result = await greetClient.SayHelloAsync(new HelloRequest
{
    Name = "Cagatay'dan selamlar Request'i"
});
*/
//Bu istek sonucunda bize server'dan HelloReply döneceğini biliyoruz. SayHelloAsync'e baktığımızda da bunu onaylayabiliriz.
//HelloReply helloReply ile de sunucudan gelecek mesajı elde edebiliriz.

//message.proto'da çalışacağımız için greet.proto ile ilgili satırları yorum satırı yaptık.

//Unary
// var messageClient = new Message.MessageClient(channel);
// MessageResponse response = await messageClient.SendMessageAsync(new MessageRequest{
//     Message = "Merhaba Dünyalı",
//     Name = "Çağatay"
// });
//System.Console.WriteLine(response.Message);

//Server Streaming

// var messageClient = new Message.MessageClient(channel);

// var response = messageClient.SendMessage(new MessageRequest {
//     Message = "Merhaba",
//     Name = "Çağatay"
// });

// CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

// while(await response.ResponseStream.MoveNext(cancellationTokenSource.Token)){
//     System.Console.WriteLine(response.ResponseStream.Current.Message);
// }

//Client Streaming

// var messageClient = new Message.MessageClient(channel);

// var request = messageClient.SendMessage();

// for (int i = 0; i < 10; i++)
// {
//     await Task.Delay(1000);
//     await request.RequestStream.WriteAsync(new MessageRequest {
//         Name = "Çağatay",
//         Message = "Mesaj " + i
//     });
// }

// //stream datanın sonlandığnı da belirtmemiz lazım.
// await request.RequestStream.CompleteAsync();

// System.Console.WriteLine((await request.ResponseAsync).Message);

//Bi directional streaming

var messageClient = new Message.MessageClient(channel);
var request = messageClient.SendMessage();

var task1 = Task.Run(async() => {
    for (int i = 0; i < 10; i++)
    {
        await Task.Delay(1000);
        await request.RequestStream.WriteAsync(
            new MessageRequest {
                Name = "Çağatay",
                Message = "Mesaj " + i
            }
        );
    }
});
CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
while (await request.ResponseStream.MoveNext(
    cancellationTokenSource.Token
))
{
    System.Console.WriteLine(request.ResponseStream.Current.Message);
}

await task1;
await request.RequestStream.CompleteAsync();