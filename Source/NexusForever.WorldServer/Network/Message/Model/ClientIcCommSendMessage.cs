using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientIcCommSendMessage)]
    public class ClientIcCommSendMessage : IReadable
    {
        public ulong ChannelID { get; private set; }
        public uint MessageId { get; private set; }
        public string Message { get; private set; }
        public string Recipient { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ChannelID = reader.ReadULong();
            MessageId = reader.ReadUInt();
            Message   = reader.ReadString();
            Recipient = reader.ReadWideString();
        }
    }
}
