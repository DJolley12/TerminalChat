using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;

namespace TerminalChatLib
{
    public class Client
    {
        public string Username { get; set; }
        public BlockingCollection<MessagePacket> IncomingChatMessages { get; set; }
        // public BlockingCollection<MessagePacket> OutgoingChatMessages { get; set; }
        // public List<MessagePacket> IncomingChatMessages { get; set; }
        public List<MessagePacket> OutgoingChatMessages { get; set; }
        public bool IsActive { get; set; }
        private TcpClient _client { get; set; }
        private string chatKey { get; set; }
        private IPAddress ipAddress { get; set; }
        private int port { get; set; }

        public Client(TcpClient client, string username)
        {
            _client = client;
            _client.Client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveTime, 2000);
            Username = username;
            IncomingChatMessages = new BlockingCollection<MessagePacket>();
            // OutgoingChatMessages = new BlockingCollection<MessagePacket>();
            // IncomingChatMessages = new List<MessagePacket>();
            OutgoingChatMessages = new List<MessagePacket>();
        }

        public async void Connect(IPAddress ipAddress, int port)
        {
            if (_client is null)
            {
                _client = new TcpClient();
            }

            this.ipAddress = ipAddress;
            this.port = port;

            await _client.ConnectAsync(ipAddress, port);
        }

        // public async void QueueOutgoingMessage(string message)
        // {
        //     try
        //     {
        //         if (_client is null)
        //         {
        //             _client = new TcpClient();
        //         }
        //         else if (_client.Client is null)
        //         {
        //             _client = new TcpClient();
        //         }

        //         if (!_client.Connected)
        //         {
        //             // _client = new TcpClient();
        //             await _client.ConnectAsync(ipAddress, port);
        //         }

        //         var stream = _client.GetStream();
        //         var writer = new BinaryWriter(stream);
        //         writer.Write(message);
        //     }
        //     catch (System.Exception)
        //     {
        //         //TODO: handle disconnect
        //         IsActive = false;
        //     }
        // }

        public async void SendMessage(MessagePacket messagePacket)
        {
            try
            {
                if (_client is null)
                {
                    _client = new TcpClient();
                }
                else if (_client.Client is null)
                {
                    _client = new TcpClient();
                }

                if (!_client.Connected)
                {
                    // _client = new TcpClient();
                    await _client.ConnectAsync(ipAddress, port);
                }

                messagePacket.WriteToStream(_client.GetStream());
            }
            catch (System.Exception)
            {
                //TODO: handle disconnect
                IsActive = false;
            }
        }

        public async void QueueIncomingMessage()
        {
            try
            {
                if (!_client.Connected)
                {
                    await _client.ConnectAsync(ipAddress, port);
                }

                if (_client.Available > 0 )
                {
                    // var stream = _client.GetStream();
                    // var messagePacket = new MessagePacket(stream);
                    var stream = _client.GetStream();
                    // var messagePacket = stream.ReadPacket();
                    var messagePacket = new MessagePacket(stream);
                    // var reader = new BinaryReader(stream);
                    // var incomingMessage = reader.ReadString();
                    // IncomingChatMessages.Add(incomingMessage);
                    IncomingChatMessages.Add(messagePacket);
                    stream.Flush();
                }
            }
            catch (System.Exception)
            {
                //TODO handle disconnect
                IsActive = false;
            }
        }

        public void QueueOutgoingMessage(string message)
        {
            var messagePacket = new MessagePacket(Username, MessageType.ChatMessage, message);

            OutgoingChatMessages.Add(messagePacket);
        }
    }
}
