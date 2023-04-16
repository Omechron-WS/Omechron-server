using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerIcCommJoinSuccess)]
    public class ServerIcCommJoinSuccess : IWritable
    {
        public byte ChannelType { get; set; }
        public ulong ChannelID { get; set; }
        public string ChannelName { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ChannelType, 3u);
            writer.Write(ChannelID);
            writer.WriteStringWide(ChannelName);
        }
    }
}
