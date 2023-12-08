using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Contact.Static;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientHousingAddNeighbor)]
    public class ClientHousingAddNeighbor : IReadable
    {
        TargetPlayerIdentity Player = new TargetPlayerIdentity(); // Empty if added with menu
        public string CharacterName; // Probably empty if added with LUA by ID
        public ushort Unknown3;
        
        public void Read(GamePacketReader reader)
        {
            Player.Read(reader);
            CharacterName = reader.ReadWideString();
            Unknown3 = reader.ReadUShort(10);
        }
    }
}
