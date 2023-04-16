using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.InterClient.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerIcCommSendMessageResult)]
    public class ServerIcCommSendMessageResult : IWritable
    {
        public ulong Channel { get; set; }
        public uint MessageID { get; set; }
        public IcCommMessageResult Result { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Channel);
            writer.Write(MessageID);
            writer.Write(Result, 4u);
        }
    }
}
