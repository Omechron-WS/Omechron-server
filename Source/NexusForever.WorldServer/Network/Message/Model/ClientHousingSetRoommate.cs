using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Contact.Static;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientHousingSetRoommate)]
    public class ClientHousingSetRoommate : IReadable
    {
        public TargetPlayerIdentity player = new TargetPlayerIdentity();
        public byte roommate = 0;

        public void Read(GamePacketReader reader)
        {
            player.Read(reader);
            reader.ReadByte(); // Always 0?
            roommate = reader.ReadByte(2);
            reader.ReadUInt(); // Always 0?
        }
    }
}
