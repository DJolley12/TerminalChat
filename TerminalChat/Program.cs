using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using LINQHelpers;
using Microsoft.Extensions.Configuration;
using Spectre.Console;
using TerminalChatLib;

namespace TerminalChat
{
    class Program
    {
        private static IConfiguration _config;
        private static EncryptionService encryptionService;
        private static List<string> incomingMessages;
        private static bool loop;
        private static ConfigureView configureView;
        private static Listener listener;
        private static ClientHandler clientHandler;

        static void Main(string[] args)
        {
            Console.Clear();
            var welcomePanel = new Panel("[underline bold green]Welcome to Terminal Chat![/]");
            welcomePanel.Border = BoxBorder.Double;
            welcomePanel.Padding(5, 2, 5, 2);
            welcomePanel.HeaderAlignment(Justify.Center);

            var welcomeFiglet = new FigletText("Terminal Chat")
                    .Color(Color.Cyan1)
                    .Centered();
            /* AnsiConsole.Live(welcomePanel); */

            AnsiConsole.Write(welcomeFiglet);


            Configure();

            var mainView = new MainView(_config);

            var tcpClient = new TcpClient();
            var client = new TerminalChatLib.Client(tcpClient, "Me");
            // Task.Run(() => listener.Listen());
            // Task.Run(() => clientHandler.ProcessOutgoing());
            // Task.Run(() => clientHandler.ProcessIncoming());
            listener.Listen();
            clientHandler.ProcessOutgoing();
            clientHandler.ProcessIncoming();
            var notConnected = true;
            var firstUsed = false;
            while (notConnected)
            {
                try 
                {
                    if (!firstUsed)
                    {
                        client.Connect(IPAddress.Parse("127.0.0.1"), 5001);
                        firstUsed = true;
                        notConnected = false;
                        ClientHandler.Instance.Clients.Add(client);
                    }
                    /* else */ 
                    /* { */
                    /*     client.Connect(IPAddress.Parse("127.0.0.1"), 5001); */
                    /* } */
                }
                catch (Exception ex)
                {
                    var excep = ex.Message;
                }
            }

            while (loop)
            {
                Console.Write("Message: ");
                var outgoingMessage = Console.ReadLine();
                clientHandler.Clients.First().QueueOutgoingMessage(outgoingMessage);
                // Task.Run(() => clientHandler.ProcessOutgoing());
                Console.WriteLine($"Message {outgoingMessage} added");
            }

            listener.Stop();
        }

        private static void Configure()
        {
            _config = new ConfigurationBuilder()
                .AddJsonFile($"{AppContext.BaseDirectory}/appsettings.json")
            /*     .SetBasePath(AppContext.BaseDirectory) */
                .Build();

            bool isConfigured = Boolean.Parse(_config["IsConfigured"]);

            if (!isConfigured)
            {
                configureView = new ConfigureView(_config);
                configureView.ConfigurationSetup();
            }

            loop = true;

            var port = Int32.TryParse(_config["Port"], out int result) == true ? result : 0;
            var listenMethodType = Helpers.ParseListenMethodType(_config["ListenMethodType"]);
            //TODO: need to parse vals from key val pair
            var ipWhitelistValues = _config.GetSection("IPWhiteList").AsEnumerable().TakeValues();
            Console.WriteLine($"white list {ipWhitelistValues.Length}");
            var ipWhitelist = ipWhitelistValues.Length > 0 ? ipWhitelistValues.ParseIPs(): new List<IPAddress>();
            var ipBlacklistValues = _config.GetSection("IPBlackList").AsEnumerable().TakeValues();
            var ipBlacklist = ipBlacklistValues.Length > 0 ? ipBlacklistValues.ParseIPs() : new List<IPAddress>();
            var connectionPassphrase = _config["ConnectionPassphrase"];
            var publicKey = _config["PublicKey"];

            encryptionService = new EncryptionService();

            clientHandler = ClientHandler.Instance;

            listener = new Listener(
                            port,
                            listenMethodType,
                            ipBlacklist,
                            ipWhitelist,
                            connectionPassphrase,
                            publicKey,
                            IPAddress.Parse("127.0.0.1"),
                            /* IPAddress.Any, */
                            encryptionService
                        );
        }
    }
}
