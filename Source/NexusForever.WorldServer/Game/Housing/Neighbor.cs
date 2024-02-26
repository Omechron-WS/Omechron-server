using Microsoft.EntityFrameworkCore;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.WorldServer.Game.Contact.Static;
using NexusForever.WorldServer.Game.Housing.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusForever.WorldServer.Game.Housing
{
    public class Neighbor : ISaveCharacter
    {
        public Neighbor(ulong characterId)
        {
            this.NeighborResidenceId = characterId;
        }
        public Neighbor(NeighborModel neighborModel)
        {
            this.NeighborResidenceId = neighborModel.NeighborResidenceId;
            this.AccessLevel = (NeighborPermission)neighborModel.AccessLevel;
        }

        public ulong Id { get; private set; }
        public ulong OwningResidenceId { get; private set; }
        public ulong NeighborResidenceId { get; private set; }
        public NeighborPermission AccessLevel = NeighborPermission.Neighbor;

        private NeighborSaveMask saveMask;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void Save(CharacterContext context)
        {
            if (saveMask == NeighborSaveMask.None)
                return;

            if ((saveMask & NeighborSaveMask.Create) != 0)
            {
                context.Add(new NeighborModel
                {
                    Id = Id,
                    OwningResidenceId = OwningResidenceId,
                    NeighborResidenceId = NeighborResidenceId,
                    AccessLevel = (byte)AccessLevel
                });
            }
            else if ((saveMask & NeighborSaveMask.Delete) != 0)
            {
                var model = new NeighborModel
                {
                    Id = Id,
                    OwningResidenceId = OwningResidenceId,
                    NeighborResidenceId = NeighborResidenceId,
                };

                context.Entry(model).State = EntityState.Deleted;
            }
            else
            {
                var model = new NeighborModel
                {
                    Id = Id,
                    OwningResidenceId = OwningResidenceId,
                    NeighborResidenceId = NeighborResidenceId
                };

                if ((saveMask & NeighborSaveMask.Modify) != 0)
                {
                    model.AccessLevel = (byte)AccessLevel;

                    context.Entry(model).State = EntityState.Modified;
                }
            }

            saveMask = NeighborSaveMask.None;
        }

        /// <summary>
        /// Enqueue <see cref="Contact"/> to be deleted from the database.
        /// </summary>
        public void EnqueueDelete()
        {
            saveMask = NeighborSaveMask.Delete;
        }
    }
}