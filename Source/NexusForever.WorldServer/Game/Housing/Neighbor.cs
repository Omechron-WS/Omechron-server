using NexusForever.WorldServer.Game.Housing.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusForever.WorldServer.Game.Housing
{
    public class Neighbor
    {
        public Neighbor(ulong characterId)
        {
            this.CharacterId = characterId;
        }
        public ulong CharacterId { get; private set; }
        public NeighborPermission Permission = NeighborPermission.Neighbor;
        public bool IsPending = true;
    }
}