using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.InterClient.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerIcCommJoinResult)]
    public class ServerIcCommJoinResult : IWritable
    {
        public IcCommChannelType Type { get; set; }
        public IcCommJoinResult Result { get; set; }
        public string Channel { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Type, 3u);
            writer.Write(Result, 4u);
            writer.WriteStringWide(Channel);
        }
    }
}
