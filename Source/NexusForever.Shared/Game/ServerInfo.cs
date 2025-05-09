using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using NexusForever.Database.Auth.Model;
using NLog;

namespace NexusForever.Shared.Game
{
    public class ServerInfo
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public ServerModel Model { get; private set; }
        public uint Address { get; private set; }
        public uint? InternalAddress { get; private set; }
        public bool AssumeOnline { get; private set; }
        public bool IsOnline { get; private set; }

        public ServerInfo(ServerModel model)
        {
            Model = model;
            AssumeOnline = Model.AssumeOnline;

            Update();
        }

        /// <summary>
        /// Attempt to connect to the remote world server asynchronously.
        /// </summary>
        public async Task PingHostAsync()
        {
            if(AssumeOnline)
            {
                IsOnline = true;
                return;
            }

            string host = Model.InternalIP != null ? Model.InternalIP : Model.Host;

            using var client = new TcpClient();
            await client.ConnectAsync(host, Model.Port).ContinueWith(task =>
            {
                IsOnline = !task.IsFaulted;
                return task;
            });
        }

        public void Update()
        {
            try
            {
                if (!IPAddress.TryParse(Model.Host, out IPAddress ipAddress))
                {
                    // find first IPv4 address, client doesn't support IPv6 as address is sent as 4 bytes
                    ipAddress = Dns.GetHostEntry(Model.Host)
                        .AddressList
                        .First(a => a.AddressFamily == AddressFamily.InterNetwork);
                }

                Address = (uint)IPAddress.HostToNetworkOrder(BitConverter.ToInt32(ipAddress.GetAddressBytes()));
            }
            catch (Exception e)
            {
                if (Address == 0)
                {
                    log.Fatal(e, $"Failed to load server entry id: {Model.Id}, host: {Model.Host} from the database!");
                    throw;
                }
                // If we have a valid address, we can ignore the error
            }

            try
            {
                InternalAddress = null;
                if (Model.InternalIP != null)
                {
                    if (!IPAddress.TryParse(Model.InternalIP, out IPAddress internalAddress))
                    {
                        // find first IPv4 address, client doesn't support IPv6 as address is sent as 4 bytes
                        internalAddress = Dns.GetHostEntry(Model.InternalIP)
                            .AddressList
                            .First(a => a.AddressFamily == AddressFamily.InterNetwork);

                        InternalAddress = (uint)IPAddress.HostToNetworkOrder(BitConverter.ToInt32(internalAddress.GetAddressBytes()));
                    }
                }
            }
            catch (Exception e)
            {
                if (InternalAddress == 0)
                {
                    log.Fatal(e, $"Failed to load server entry id: {Model.Id}, host: {Model.Host} from the database!");
                    throw;
                }
                // If we have a valid address, we can ignore the error
            }
        }
    }
}
