using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Server054A)]
    public class Server054A : IWritable
    {
        public ulong Channel { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Channel);
        }
    }
}
