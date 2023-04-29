using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Housing.Static;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientHousingSetRoommate)]
    public class ClientHousingSetPermission : IReadable
    {
        public TargetPlayerIdentity player = new TargetPlayerIdentity();
        public NeighborPermission Permission = NeighborPermission.Neighbor;

        public void Read(GamePacketReader reader)
        {
            player.Read(reader);
            reader.ReadByte(); // Always 0?
            Permission = (NeighborPermission)reader.ReadByte(2);
            reader.ReadUInt(); // Always 0?
        }
    }
}
