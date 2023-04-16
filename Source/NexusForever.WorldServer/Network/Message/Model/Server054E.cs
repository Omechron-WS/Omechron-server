using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerIcCommSendMessage)]
    public class ServerIcCommSendMessage : IWritable
    {
        public ulong Channel { get; set; }
        public string Message { get; set; }
        public string SenderName { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Channel);
            writer.WriteString(Message);
            writer.WriteStringWide(SenderName);
        }
    }
}
