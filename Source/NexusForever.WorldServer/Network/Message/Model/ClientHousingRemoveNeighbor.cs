using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Contact.Static;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientHousingRemoveNeighbor)]
    public class ClientHousingRemoveNeighbor : IReadable
    {
        public TargetPlayerIdentity player = new TargetPlayerIdentity();
        public ushort Unknown1;
        
        public void Read(GamePacketReader reader)
        {
            player.Read(reader);
            Unknown1 = reader.ReadUShort(10);
        }
    }
}
