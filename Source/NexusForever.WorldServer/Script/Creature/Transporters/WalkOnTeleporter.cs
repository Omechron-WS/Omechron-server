using NexusForever.Database.World.Model;
using NexusForever.Shared;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Map;
using NexusForever.WorldServer.Game.Reputation.Static;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using static NexusForever.WorldServer.Network.Message.Model.ServerCharacterList.Character;

namespace NexusForever.WorldServer.Script.Creature.Transporters
{
    public class WalkOnTeleporter : CreatureScript
    {
        protected virtual (ulong, ulong)[] pairs { get; }

        public override void OnCreate(WorldEntity me)
        {
            me.RangeCheck = 5f;
        }

        protected WorldLocation2Entry GetTeleportLocation(Vector3 pos, params (ulong, ulong)[] locations)
        {
            float nearestDist = float.MaxValue;
            WorldLocation2Entry target = null;
            for (int i = 0; i < locations.Length; i++)
            {
                var loc1 = GetLocationFromID(locations[i].Item1);
                var loc2 = GetLocationFromID(locations[i].Item2);

                float distance = pos.GetDistance(loc1.GetPosition());
                if (distance < nearestDist)
                {
                    target = loc2; // new best match
                    nearestDist = distance;
                }
                distance = pos.GetDistance(loc2.GetPosition());
                if (distance < nearestDist)
                {
                    target = loc1; // new best match
                    nearestDist = distance;
                }
            }
            return target;
        }

        protected WorldLocation2Entry GetLocationFromID(ulong worldLocation2Id)
        {
            return GameTableManager.Instance.WorldLocation2.GetEntry(worldLocation2Id);
        }

        public override void OnEnterRange(WorldEntity me, WorldEntity activator)
        {
            if (activator is not Player player)
                return;

            if (!player.CanTeleport())
                return;

            player.TeleportTo(GetTeleportLocation(player.Position, pairs));
            player.SetHousingTeleportCooldown();

        }
    }
}
