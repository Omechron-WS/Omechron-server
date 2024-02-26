using System.Collections.Generic;

namespace NexusForever.Database.Character.Model
{
    public class NeighborModel
    {
        public ulong Id { get; set; }
        public ulong OwningResidenceId { get; set; }
        public ulong NeighborResidenceId { get; set; }
        public byte AccessLevel { get; set; }

        public ResidenceModel OwningResidence { get; set; }
        public ResidenceModel NeighborResidence { get; set; }
    }
}
