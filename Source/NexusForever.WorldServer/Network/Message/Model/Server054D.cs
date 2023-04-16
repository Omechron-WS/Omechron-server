using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Server054D)]
    public class Server054D : IWritable
    {
        public ulong Channel { get; set; }
        public uint Unknown8 { get; set; }
        public string UnknownC { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Channel);
            writer.Write(Unknown8);
            writer.WriteStringWide(UnknownC);
        }
    }
}
