using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TerminalChatLib
{
    public class ClientHandler
    {
        private ClientHandler() { Clients = new BlockingCollection<Client>(); Loop = true; }
        // private ClientHandler() { Clients = new List<Client>(); Loop = true; }
        private static readonly Lazy<ClientHandler> _clientHandler = new Lazy<ClientHandler>(() => new ClientHandler());
        public static ClientHandler Instance =>  _clientHandler.Value; 

        public BlockingCollection<Client> Clients { get; set; }
        // public List<Client> Clients { get; set; }
        public bool Loop { get; set; }

        public async void ProcessOutgoing()
        {
            while (Instance.Loop)
            {
                for (int i = 0; i < Instance.Clients.Count; i++)
                {
                    var client = Instance.Clients.ElementAt(i);
                    var outgoingToSend = client.OutgoingChatMessages
                                            .Take(client.OutgoingChatMessages.Count()).ToArray();

                    for (int j = 0; j < client.OutgoingChatMessages.Count(); j++)
                    {
                        // var message = outgoingToSend[j];
                        var message = client.OutgoingChatMessages[j];
                        client.SendMessage(message);

                        lock(client.OutgoingChatMessages)
                            client.OutgoingChatMessages.Remove(message);
                    }
                    // foreach (var outgoingMessage in client.OutgoingChatMessages.GetConsumingEnumerable())
                    // {
                    //     client.SendMessage(outgoingMessage);
                    // }
                }

                await Task.Delay(100);
            }
        }

        public async void ProcessIncoming()
        {
            while (Instance.Loop)
            {
                for (int i = 0; i < Instance.Clients.Count; i++)
                {
                    var client = Instance.Clients.ElementAt(i);
                    var incomingToProcess = client.IncomingChatMessages
                                                .Take(client.IncomingChatMessages.Count()).ToArray();

                    for (int j = 0; j < incomingToProcess.Count(); j++)
                    {
                        var message = incomingToProcess[j];
                        Console.WriteLine(message);

                        // lock(client.IncomingChatMessages)
                        //     client.IncomingChatMessages.Remove(message);
                    }
                }

                await Task.Delay(100);
            }
        }
    }
}
