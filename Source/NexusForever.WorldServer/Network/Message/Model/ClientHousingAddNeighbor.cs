using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Contact.Static;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientHousingAddNeighbor)]
    public class ClientHousingAddNeighbor : IReadable
    {
        public ushort Unknown1;
        public ulong Unknown2;
        public string CharacterName;
        public ushort Unknown3;
        
        public void Read(GamePacketReader reader)
        {
            Unknown1 = reader.ReadUShort(14);
            Unknown2 = reader.ReadULong(); // possible TargetPlayer? But is 0.
            CharacterName = reader.ReadWideString();
            Unknown3 = reader.ReadUShort(10);
        }
    }
}
