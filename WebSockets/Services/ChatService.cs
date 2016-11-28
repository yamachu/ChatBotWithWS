using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ChatBotWithWS.Models;
using Newtonsoft.Json;
using ChatBotWithWS.Models.ChatCommands;

namespace ChatBotWithWS.WebSockets.Services
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
        private ConcurrentBag<ChatUser> websockets;
        private const int BufferSize = 1024 * 4;

        public ChatService()
        {
            websockets = new ConcurrentBag<ChatUser>();
        }

        public void DebugLog(string message)
        {
            System.Console.WriteLine(message);
        }

        async public Task HandleConnection(HttpContext context)
        {
            var socket = await context.WebSockets.AcceptWebSocketAsync();
            if (socket.State != WebSocketState.Open) return;

            var user = new ChatUser(socket);
            // will support
            user.Name = System.Guid.NewGuid().ToString().Substring(0, 5);

            websockets.Add(user);

            while (user.Socket?.State == WebSocketState.Open)
            {
                var buffer = new ArraySegment<Byte>(new Byte[BufferSize]);
                var token = CancellationToken.None;
                WebSocketReceiveResult received;

                try {
                    received = await user.Socket?.ReceiveAsync(buffer, token);
                } catch {
                    System.Console.WriteLine("Unexpected Disconnect...");
                    System.Console.WriteLine("Codecheck test case is not sending kill signal");
                    
                    user.Socket?.Dispose();
                    return;
                }

                switch (received.MessageType)
                {
                    case WebSocketMessageType.Text:
                    // recieve
                    var request = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
                    Models.Entities.ChatRecieveModel json_d;
                    try {
                        json_d = JsonConvert.DeserializeObject<Models.Entities.ChatRecieveModel>(request);
                    } catch(Exception ex) {
                        System.Console.WriteLine("Unexpected Format");
                        System.Console.WriteLine(ex.StackTrace);
                        var error_msg = new Models.Entities.ChatTransferModel()
                        {
                            Text = "Unexpected format, require json => {text:\"FooBar\"}",
                            MessageType = Models.Entities.ChatMessageType.Bot,
                            Success = false
                        };
                        await TransferAsync(error_msg, user.Socket);
                        break;
                    }

                    await Echo(json_d);

                    // bot command
                    var commandModel = CommandHelper.isValidCommandFormat(json_d.Text);
                    if (commandModel == null) break;

                    var transfer = CommandRunner.GenerateResponse(commandModel);

                    // 自分だけに返したいコマンドができたら使う
                    if (!transfer.SecretMessage) {
                        await TransferAsync(transfer);
                    } else {
                        await TransferAsync(transfer, user.Socket);
                    }

                    break;

                    case WebSocketMessageType.Close:
                    case WebSocketMessageType.Binary:
                    await socket.CloseAsync(received.CloseStatus.Value, received.CloseStatusDescription, CancellationToken.None);
                    break;
                }                
            }
            user.Socket.Dispose();
        }

        async private Task SendBroadcast(ArraySegment<Byte> buffer, WebSocketMessageType messageType = WebSocketMessageType.Text)
        {
            var token = CancellationToken.None;
            // 一個コネクション死んでたら死ぬかも
            await Task.WhenAll(websockets.Where(x => x?.Socket?.State == WebSocketState.Open)
                        .Select(x => x.Socket.SendAsync(buffer, messageType, true, token)));
        }

        async private Task SendUser(ArraySegment<Byte> buffer, WebSocket socket, WebSocketMessageType messageType = WebSocketMessageType.Text)
        {
            var token = CancellationToken.None;
            await socket.SendAsync(buffer, messageType, true, token);
        }

        async private Task TransferAsync(Models.Entities.ChatTransferModel model, WebSocket user = null, WebSocketMessageType messageType = WebSocketMessageType.Text)
        {
            var response = JsonConvert.SerializeObject(model);
            var sendingData = Encoding.UTF8.GetBytes(response);
            var sendingBuffer = new ArraySegment<byte>(sendingData);

            if (user == null) {
                await SendBroadcast(sendingBuffer, messageType);
            } else {
                await SendUser(sendingBuffer, user, messageType);
            }
        }

        async private Task Echo(Models.Entities.ChatRecieveModel request)
        {
            var trans_m = new Models.Entities.ChatTransferModel
            {
                Text = request.Text,
                MessageType = Models.Entities.ChatMessageType.Message,
                Success = true
            };
            var response = JsonConvert.SerializeObject(trans_m);
            var sendingData = Encoding.UTF8.GetBytes(response);
            var sendingBuffer = new ArraySegment<byte>(sendingData);

            await SendBroadcast(sendingBuffer);
        }
    }
}