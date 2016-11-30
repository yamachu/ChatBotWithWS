using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ChatBotWithWS.Models;
using Newtonsoft.Json;
using ChatBotWithWS.Models.ChatCommands;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using ChatBotWithWS.Models.Entities;

namespace ChatBotWithWS.Services
{
    public interface IChatService
    {
        void DebugLog(string message);
        Task HandleConnection(HttpContext context);
    }

    // ref https://github.com/aspnet/WebSockets/tree/dev/samples/EchoApp
    //     http://dotnetthoughts.net/using-websockets-in-aspnet-core/
    public class ChatService: IChatService
    {
        private Subject<ChatTransferModel> MessageSubject;
        private const int BufferSize = 1024 * 4;

        public ChatService()
        {
            MessageSubject = new Subject<ChatTransferModel>();
        }

        // dummy
        public void DebugLog(string message)
        {
            System.Console.WriteLine(message);
        }

        async public Task HandleConnection(HttpContext context)
        {
            WeakReference<WebSocket> WeakSocket;
            {
                var accepted = await context.WebSockets.AcceptWebSocketAsync();
                if (accepted.State != WebSocketState.Open) return;
                WeakSocket = new WeakReference<WebSocket>(accepted);
            }

            var user = new ChatUser(context.GetHashCode());
            var noneToken = CancellationToken.None;
            
            #region TODO
            user.Name = System.Guid.NewGuid().ToString().Substring(0, 5);
            user.Room = "Global";
            #endregion

            var MyBroadcastStream = MessageSubject
            .Where(o => !o.Target.HasValue || o.Target.HasValue && o.Target.Value == user.UserHash)
            .Subscribe(async s => {
                var response = JsonConvert.SerializeObject(s);
                var sendingData = Encoding.UTF8.GetBytes(response);
                var token = CancellationToken.None;
                WebSocket me;
                if (WeakSocket.TryGetTarget(out me)) {
                    try {
                        System.Console.WriteLine($"Send to {user.UserHash}");
                        System.Console.WriteLine($"{s.Text}");
                        await me.SendAsync(new ArraySegment<byte>(sendingData, 0, sendingData.Length), s.SocketMessageType, true, token);
                    } catch (Exception ex){
                        System.Console.Error.WriteLine("In Subsctibe");
                        System.Console.Error.WriteLine(ex.StackTrace);
                    }
                } else {
                    System.Console.Error.WriteLine("No reference");
                }
            });

            WebSocket socket;
            if (!WeakSocket.TryGetTarget(out socket)) {
                MyBroadcastStream.Dispose();
                return;
            }
            while (socket?.State == WebSocketState.Open)
            {
                var buffer = new ArraySegment<Byte>(new Byte[BufferSize]);   
                WebSocketReceiveResult received;

                try {
                    if (!WeakSocket.TryGetTarget(out socket)) {
                        MyBroadcastStream.Dispose();
                        return;
                    }
                    received = await socket.ReceiveAsync(buffer, noneToken);
                } catch (Exception ex){
                    System.Console.Error.WriteLine("Unexpected Disconnect");
                    System.Console.Error.WriteLine(ex.StackTrace);
                    MyBroadcastStream.Dispose();
                    return;
                }

                switch (received.MessageType)
                {
                    case WebSocketMessageType.Text:
                    var request = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);

                    ChatRecieveModel json_d;
                    #region Broadcast
                    try {
                        json_d = JsonConvert.DeserializeObject<Models.Entities.ChatRecieveModel>(request);
                        if (json_d == null) {
                            throw new NullReferenceException();
                        }
                    } catch(Exception ex) {
                        System.Console.Error.WriteLine("Unexpected Format");
                        System.Console.Error.WriteLine(ex.StackTrace);

                        var error_msg = ChatTransferModel.CreateModel(ChatTransferModelType.FORMAT_ERROR, user.UserHash);

                        MessageSubject.OnNext(error_msg);
                        break;
                    }

                    var echoModel = ChatTransferModel.FromRecieveModel(json_d);
                    MessageSubject.OnNext(echoModel);
                    #endregion

                    #region Bot Command
                    var commandModel = CommandHelper.isValidCommandFormat(json_d);
                    if (commandModel == null) break;

                    var transfer = await CommandRunner.GenerateResponse(commandModel);
                    MessageSubject.OnNext(transfer);
                    #endregion
                    break;

                    case WebSocketMessageType.Close:
                    if (WeakSocket.TryGetTarget(out socket)) {
                        await socket.CloseAsync(received.CloseStatus.Value, received.CloseStatusDescription, CancellationToken.None);
                    }
                    MyBroadcastStream.Dispose();
                    return;
                    case WebSocketMessageType.Binary:
                    // not supported
                    if (WeakSocket.TryGetTarget(out socket)) {
                        await socket.CloseAsync(received.CloseStatus.Value, received.CloseStatusDescription, CancellationToken.None);
                    }
                    MyBroadcastStream.Dispose();
                    return;
                }
            }
        }
    }
}