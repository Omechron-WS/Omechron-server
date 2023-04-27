using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Contact.Static;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientHousingAddNeighbor)]
    public class ClientHousingAddNeighbor : IReadable
    {
        ulong Unknown1;
        ushort Unknown2;
        string CharacterName;
        ushort Unknown3;
        
        public void Read(GamePacketReader reader)
        {
            Unknown1 = reader.ReadULong();
            Unknown2 = reader.ReadUShort(14);
            CharacterName = reader.ReadWideString();
            Unknown3 = reader.ReadUShort(10);
        }
    }
}
