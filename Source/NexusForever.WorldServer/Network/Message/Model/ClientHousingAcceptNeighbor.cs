using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Contact.Static;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientHousingAcceptNeighbor)]
    public class ClientHousingAcceptNeighbor : IReadable
    {
        public bool Accepted;

        public void Read(GamePacketReader reader)
        {
            Accepted = reader.ReadBit();
        }
    }
}
