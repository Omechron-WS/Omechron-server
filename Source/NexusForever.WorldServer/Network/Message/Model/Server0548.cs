using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Server0548)]
    public class Server0548 : IWritable
    {
        public byte Unknown0 { get; set; }
        public ulong Channel { get; set; }
        public string Unknown10 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unknown0, 3u);
            writer.Write(Channel);
            writer.WriteStringWide(Unknown10);
        }
    }
}
