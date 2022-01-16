using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TerminalChatLib
{
    public class Listener
    {
        private readonly IEncryptionService _encryptionService;

        private ClientHandler clientHandler { get; set; }
        private TcpListener tcpListener { get; set; }
        private int port { get; set; }
        private string publicKey { get; set; }
        private string connectionPassphrase { get; set; }
        private bool listen { get; set; }
        private ListenMethodType listenMethodType { get; set; }
        private List<IPAddress> blackListedIpAddressess { get; set; }
        private List<IPAddress> whiteListedIpAddressses { get; set; }

        public Listener(int port, ListenMethodType listenMethodType, List<IPAddress> blackListedIpAddressess, List<IPAddress> whiteListedIpAddressses, string connectionKey, string connectionPassphrase, IPAddress ipAddress, EncryptionService encryptionService)
        {
            this.port = port;
            this.publicKey = connectionKey;
            this.connectionPassphrase = connectionPassphrase;
            this.listenMethodType = listenMethodType;
            this.blackListedIpAddressess = blackListedIpAddressess;
            this.whiteListedIpAddressses = whiteListedIpAddressses;
            var ipAddr = ipAddress == IPAddress.Parse("127.0.0.1") ? IPAddress.Parse("127.0.0.1") : IPAddress.Any;
            // tcpListener = new TcpListener(ipAddr, port);
            tcpListener = new TcpListener(ipAddress, port);
            _encryptionService = encryptionService;
            listen = true;
        }

        public async void Listen()
        {
            tcpListener.Start();
            while (listen)
            {
                if (tcpListener.Pending())
                {
                    await HandleConnectionRequest(await tcpListener.AcceptTcpClientAsync().ConfigureAwait(false));
                }
                else
                {
                    await Task.Delay(100);
                }
            }
        }

        public void Stop()
        {
            listen = false;
            tcpListener.Stop();
        }

        private async Task HandleConnectionRequest(TcpClient tcpClient)
        {
            using NetworkStream netStream = tcpClient.GetStream();
            using StreamReader reader = new StreamReader(netStream);

            var ipEndPoint = (IPEndPoint)tcpClient.Client.RemoteEndPoint;
            var ipAddress = ipEndPoint.Address;

            if (listenMethodType == ListenMethodType.Whitelist && !whiteListedIpAddressses.Contains(ipAddress))
            {
                tcpClient.Close();
                return;
            }
            else if (listenMethodType == ListenMethodType.Blacklist && blackListedIpAddressess.Contains(ipAddress))
            {
                tcpClient.Close();
                return;
            }

            var publicKeyBytes = Encoding.ASCII.GetBytes(publicKey);
            tcpClient.Client.Send(publicKeyBytes);

            var incomingPassphrase = _encryptionService.Decrypt(await reader.ReadToEndAsync());

            if (incomingPassphrase == connectionPassphrase || connectionPassphrase == "")
            {
                //TODO: handle valid connection
                //share encryption key secret
                var client = new Client(tcpClient, ipAddress.ToString());
                clientHandler.Clients.Add(client);
            }
            else
            {
                tcpClient.Close();
            }
        }
    }
}
