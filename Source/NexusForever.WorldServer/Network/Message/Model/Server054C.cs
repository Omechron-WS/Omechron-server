using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Server054C)]
    public class Server054C : IWritable
    {
        public ulong Channel { get; set; }
        public uint Unknown8 { get; set; }
        public byte UnknownC { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Channel);
            writer.Write(Unknown8);
            writer.Write(UnknownC, 4u);
        }
    }
}
