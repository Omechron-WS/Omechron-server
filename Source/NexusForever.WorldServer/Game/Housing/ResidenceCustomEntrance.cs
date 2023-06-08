using NexusForever.Shared.GameTable.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NexusForever.WorldServer.Game.Housing
{
    public class ResidenceCustomEntrance : ResidenceEntrance
    {
        public ulong Id { get; }
        public string Name { get; }
        public ulong ResidenceId { get; set; }

        public ResidenceCustomEntrance(ulong id, string name, ulong residenceId, WorldEntry entry, Vector3 position, Quaternion rotation) : base(entry, position, rotation)
        {
            Id = id;
            Name = name;
            ResidenceId = residenceId;
        }
    }
}
