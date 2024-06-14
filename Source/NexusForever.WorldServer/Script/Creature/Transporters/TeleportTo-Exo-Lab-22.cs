using NexusForever.Shared;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Map;
using NexusForever.WorldServer.Game.Reputation.Static;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace NexusForever.WorldServer.Script.Creature.Transporters
{
    [Script(26007)]
    public class TeleportTo_EXOLab_22 : CreatureScript
    {
        private WorldLocation2Entry[] Locations;

        public override void OnCreate(WorldEntity me)
        {
            me.RangeCheck = 5f;
            SetLocationsForCreatureId(me.CreatureId);
        }

        private void SetLocations(params ulong[] worldLocations)
        {
            Locations = new WorldLocation2Entry[worldLocations.Length];
            for (int i = 0; i < worldLocations.Length; i++)
            {
                Locations[i] = GameTableManager.Instance.WorldLocation2.GetEntry(worldLocations[i]);
            }
        }

        private void SetLocationsForCreatureId(uint creature2ID)
        {
            switch(creature2ID)
            {
                case 26007:
                    SetLocations(19323, 19321); // entrane is at -2682.32, -1027.07, -5875.07 1387
                    break;
            }
        }

        private int GetClosestLocationID(WorldEntity me)
        {
            float nearest = float.MaxValue;
            int nearestIdx = 0;
            for (int i = 0; i < Locations.Length; i++)
            {
                float distance = me.Position.GetDistance(Locations[i].GetPosition());
                if (distance < nearest)
                {
                    nearestIdx = i;
                    nearest = distance;
                }
            }
            return nearestIdx;
        }

        public override void OnEnterRange(WorldEntity me, WorldEntity activator)
        {
            if (activator is not Player player)
                return;

            if (!player.CanTeleport())
                return;

            player.TeleportTo(Locations[(GetClosestLocationID(me) + 1) % Locations.Length]); // go to the next location in a cycle.

        }
    }
}
