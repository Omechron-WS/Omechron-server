using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Newtonsoft.Json.Linq;
using NexusForever.Shared;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Command.Convert;
using NexusForever.WorldServer.Command.Static;
using NexusForever.WorldServer.Game;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Housing;
using NexusForever.WorldServer.Game.Housing.Static;
using NexusForever.WorldServer.Game.Map;
using NexusForever.WorldServer.Game.RBAC.Static;
using NexusForever.WorldServer.Network.Message;
using NexusForever.WorldServer.Network.Message.Model;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Command.Handler
{
    [Command(Permission.House, "A collection of commands to modify housing residences.", "house")]
    [CommandTarget(typeof(Player))]
    public class HouseCommandCategory : CommandCategory
    {
        [Command(Permission.HouseDecor, "A collection of commands to modify decor in housing residences.", "decor")]
        public class HouseDecorCommandCategory : CommandCategory
        {
            [Command(Permission.HouseDecorAdd, "Add decor to housing residence crate optionally specifying quantity.", "add")]
            public void HandleHouseDecorAdd(ICommandContext context,
                [Parameter("Decor info id entry to add to the crate.")]
                uint decorInfoId,
                [Parameter("Quantity of decor to add to the crate.")]
                uint? quantity,
                [Parameter("Color of the decor to add to the crate.")]
                ushort? color)
            {
                DecorAdd_impl(context, decorInfoId, quantity, color);
            }

            public static void DecorAdd_impl(ICommandContext context, uint decorInfoId, uint? quantity, ushort? color = null)
            {
                quantity ??= 1u;

                HousingDecorInfoEntry entry = GameTableManager.Instance.HousingDecorInfo.GetEntry(decorInfoId);
                if (entry == null)
                {
                    context.SendMessage($"Invalid decor info id {decorInfoId}!");
                    return;
                }

                context.GetTargetOrInvoker<Player>().ResidenceManager.DecorCreate(entry, quantity.Value, color);
            }

            [Command(Permission.HouseDecorLookup, "Returns a list of decor ids that match the supplied name.", "lookup")]
            public void HandleHouseDecorLookup(ICommandContext context,
                [Parameter("Name or partial name of the housing decor item to search for.")]
                string name)
            {
                var sw = new StringBuilder();
                sw.AppendLine("Decor Lookup Results:");

                TextTable tt = GameTableManager.Instance.GetTextTable(context.Language);
                foreach (HousingDecorInfoEntry decorEntry in
                    SearchManager.Instance.Search<HousingDecorInfoEntry>(name, context.Language, e => e.LocalizedTextIdName, true))
                {
                    string text = tt.GetEntry(decorEntry.LocalizedTextIdName);
                    sw.AppendLine($"({decorEntry.Id}) {text}");
                }

                context.SendMessage(sw.ToString());
            }
        }

        [Command(Permission.HouseDecorAdd, "Add decor to housing residence crate optionally specifying quantity.", "decoradd")]
        public void HandleHouseDecorAddDummy(ICommandContext context,
            [Parameter("Decor info id entry to add to the crate.")]
            uint decorInfoId,
            [Parameter("Quantity of decor to add to the crate.")]
            uint? quantity,
            [Parameter("Color of the decor to add to the crate.")]
            ushort? color)
        {
            HouseDecorCommandCategory.DecorAdd_impl(context, decorInfoId, quantity, color);
        }

        [Command(Permission.HouseTeleport, "Teleport to a residence, optionally specifying a character.", "teleport")]
        public void HandleHouseTeleport(ICommandContext context,
            [Parameter("", ParameterFlags.Optional)]
            string name,
            [Parameter("", ParameterFlags.Optional)]
            string lastName)
        {
            Player target = context.GetTargetOrInvoker<Player>();
            if (!target.CanTeleport())
            {
                context.SendMessage("You have a pending teleport! Please wait to use this command.");
                return;
            }

            if (!string.IsNullOrWhiteSpace(lastName))
            {
                name = $"{name} {lastName}";
            }
            else if (string.IsNullOrWhiteSpace(name))
            {
                name = target.Name;
            }

            Residence residence = GlobalResidenceManager.Instance.GetResidenceByOwner(name);
            if (residence == null)
            {
                if (name == target.Name)
                {
                    residence = GlobalResidenceManager.Instance.CreateResidence(target);
                }
                else
                {
                    context.SendMessage("A residence for that character doesn't exist!");
                    return;
                }
            }

            if (residence.OwnerId != context.InvokingPlayer.CharacterId)
            {
                if (residence.Has18PlusLock())
                {
                    if (!context.InvokingPlayer.IsAdult)
                    {
                        context.InvokingPlayer.SendSystemMessage("This plot is currently unavailable.");
                        return;
                    }
                }

                switch (residence.PrivacyLevel)
                {
                    case ResidencePrivacyLevel.Private:
                        {
                            context.InvokingPlayer.SendSystemMessage("This plot is currently unavailable.");
                            return;
                        }
                    // TODO: check if player is either a neighbour or roommate
                    case ResidencePrivacyLevel.NeighborsOnly:
                        break;
                    case ResidencePrivacyLevel.RoommatesOnly:
                        break;
                }
            }

            ResidenceEntrance entrance = GlobalResidenceManager.Instance.GetResidenceEntrance(residence.PropertyInfoId);
            target.Rotation = entrance.Rotation.ToEulerRadians();
            target.TeleportTo(entrance.Entry, entrance.Position, residence.Parent?.Id ?? residence.Id);
        }

        [Command(Permission.AdultPlotLockOwner, "Toggle the 18+ lock on your plot.", "nsfwlock")]
        public void HandleHouseNSFWLock(ICommandContext context,
            [Parameter("On, off or status")]
            string setting)
        {
            Residence res = GlobalResidenceManager.GetCurrentVisitedResidence(context.InvokingPlayer);
            bool setLock;
            if (setting.Equals("on", StringComparison.InvariantCultureIgnoreCase))
            {
                setLock = true;
            }
            else if (setting.Equals("off", StringComparison.InvariantCultureIgnoreCase))
            {
                setLock = false;
            }
            else if (setting.Equals("status", StringComparison.InvariantCultureIgnoreCase))
            {
                context.SendMessage($"NSFWLock status is {(res.Has18PlusLock() ? "on" : "off")}");
                return;
            }
            else
            {
                context.SendError("Setting was not 'on' or 'off'.");
                return;
            }
            if (!res.CanModifyResidence(context.InvokingPlayer))
            {
                context.SendError("You can't modify the residence you're currently on.");
                return;
            }
            bool result = res.Set18PlusLock(setLock);
            if (!result)
            {
                context.SendError("Could not enable lock. Is there anyone on the plot that is not 18+?");
                return;
            }
        }

        [Command(Permission.AdultPlotLockOwner, "Set the time limit on the 18+ lock on your plot.", "nsfwtimelock")]
        public void HandleHouseNSFWTimeLock(ICommandContext context,
           [Parameter("How long?")]
            string time,
           [Parameter("Time unit (minute, hour, day, week, month)")]
            string timeUnit)
        {
            DateTime lockTime = DateTime.Now;
            string timeAmount = "";
            if (!string.IsNullOrWhiteSpace(timeUnit))
            {
                if (uint.TryParse(time, out uint timeNum))
                {
                    switch (timeUnit.ToLowerInvariant())
                    {
                        case "minute":
                        case "min":
                        case "minutes":
                        case "mins":
                            lockTime = lockTime.AddMinutes(timeNum);
                            timeAmount = $"{timeNum} {(timeNum != 1 ? "minutes" : "minute")}";
                            break;
                        case "hour":
                        case "hours":
                            lockTime = lockTime.AddHours(timeNum);
                            timeAmount = $"{timeNum} {(timeNum != 1 ? "hours" : "hour")}";
                            break;
                        case "day":
                        case "days":
                            lockTime = lockTime.AddDays(timeNum);
                            timeAmount = $"{timeNum} {(timeNum != 1 ? "days" : "day")}";
                            break;
                        case "week":
                        case "weeks":
                            lockTime = lockTime.AddDays(timeNum * 7);
                            timeAmount = $"{timeNum} {(timeNum != 1 ? "weeks" : "week")}";
                            break;
                        case "month":
                        case "months":
                            lockTime = lockTime.AddMonths((int)timeNum);
                            timeAmount = $"{timeNum} {(timeNum != 1 ? "months" : "month")}";
                            break;
                        default:
                            context.SendError("Time unit not recognized, should be minute/hour/day/week/month");
                            return;
                    }
                }
                else
                {
                    context.SendError("Could not parse the first parameter.");
                    return;
                }
            }
            else
            {
                context.SendError("Time unit not defined.");
                return;
            }

            Residence res = GlobalResidenceManager.GetCurrentVisitedResidence(context.InvokingPlayer);
            if (!res.CanModifyResidence(context.InvokingPlayer))
            {
                context.SendError("You can't modify the residence you're currently on.");
                return;
            }
            bool result = res.Set18PlusLock(true, lockTime, timeAmount);
            if (!result)
            {
                context.SendError("Could not enable lock. Is there anyone on the plot that is not 18+?");
                return;
            }
        }

        [Command(Permission.HouseRemodel, "Change ground/sky.", "remodel")]
        public void HandleRemodelCommand(ICommandContext context,
            [Parameter("Ground, sky, music, or house plug?")]
            string option,
            [Parameter("ID")]
            ushort id)
        {
            Player target = context.InvokingPlayer;
            //remodel
            ClientHousingRemodel clientRemod = new ClientHousingRemodel();
            ResidenceMapInstance residenceMap = target.Map as ResidenceMapInstance;
            if (residenceMap == null)
            {
                context.SendError("You need to be on a housing map to use this command!");
            }

            Residence residence = GlobalResidenceManager.Instance.GetResidenceByOwner(context.InvokingPlayer.Name);

            if (option.ToLower() == "ground")
            {
                residence.Ground = id;
            }
            else if (option.ToLower() == "sky")
            {
                residence.Sky = id;
            }
            else if (option.ToLower() == "music")
            {
                residence.Music = id;
            }
            else if (option.ToLower() == "house")
            {
                var plugItem = GameTableManager.Instance.HousingPlugItem.GetEntry(id);
                if (plugItem != null)
                {
                    ClientHousingPlugUpdate pu = new ClientHousingPlugUpdate
                    {
                        Operation = PlugUpdateOperation.Place,
                        PlotInfo = residence.GetPlotByIndex(0).PlotInfoEntry.Id,
                        PlugFacing = (uint)residence.GetPlotByIndex(0).PlugFacing,
                        PlugItem = id,
                        ResidenceId = residence.Id,
                        RealmId = WorldServer.RealmId
                    };
                    residence.Map.SetPlug(target, pu);
                    /*if (residence.SetHouse(plugItem))
                    {
                        residence.getMap().HandleHouseChange(target, residence.GetPlot(0));
                    }
                    else
                    {
                        context.SendError("Unknown error.");
                        return;
                    }*/
                }
                else
                {
                    context.SendError("Invalid housingPlugItem ID.");
                    return;
                }
                return; // not a normal remodel.
            }
            else
            {
                context.SendError("You can only change the ground, sky, music, or house plug with this command.");
            }
            residenceMap.Remodel(new Network.Message.Model.Shared.TargetResidence
            {
                ResidenceId = residence.Id,
                RealmId = WorldServer.RealmId
            }, target, clientRemod);
        }

        [Command(Permission.House, "A collection of commands to modify decor in housing residences.", "neighbor")]
        public class HouseNeighborCommandCategory : CommandCategory
        {
            [Command(Permission.GMFlag, "Add neighbor", "add")]
            public void HandleAddNeighbor(ICommandContext context,
                [Parameter("CharacterID")]
                ulong val)
            {
                Player player = context.InvokingPlayer;
                if (player == null) { return; }
                Residence res = GlobalResidenceManager.Instance.GetResidenceByOwner(player.CharacterId);
                if (res == null) { return; }
                res.AddNeighbor(val);
                res.SendNeighbors(player);
            }
        }

        [Command(Permission.House, "Set decor entity.", "setentity")]
        public void HandleSetEntity(ICommandContext context,
            [Parameter("Decor ID")]
            ulong decorId,
            [Parameter("Entity Type", ParameterFlags.None, typeof(EnumParameterConverter<EntityType>))]
            EntityType entityType,
            [Parameter("Creature2 ID")]
            ulong creature2Id)
        {
            Player player = context.InvokingPlayer;
            ResidenceMapInstance map = player?.Map as ResidenceMapInstance;
            Residence res = map?.GetResidenceByZone(player.Zone.Id);
            Decor dec = res?.GetDecor(decorId);
            WorldEntity en = dec?.Entity;
            if (dec != null)
            {
                Vector3 pos = context.InvokingPlayer.Position;
                if(en != null)
                {
                    pos = en.Position;

                    dec.RemoveEntity();
                }

                WorldEntity pl;
                ushort plotID = ResidenceMapInstance.GetPlotIdForDecorEntity(dec.Residence, dec.PlotIndex);
                switch (entityType) {
                    case EntityType.Platform:
                        pl = new Platform(GameTableManager.Instance.Creature2.GetEntry(creature2Id), (long)decorId, plotID);
                        break;
                    case EntityType.NonPlayer:
                        pl = new NonPlayer(GameTableManager.Instance.Creature2.GetEntry(creature2Id), (long)decorId, plotID);
                        break;
                    case EntityType.Simple:
                        pl = new Simple(GameTableManager.Instance.Creature2.GetEntry(creature2Id), (long)decorId, plotID);
                        break;
                    default:
                        return;
                }
                
                map.SetDecorEntityProperties(dec, pl, pos, dec.Rotation);
                pl.InitialiseTemporaryEntity();
                dec.SetEntity(pl);
                map.EnqueueAdd(pl, new MapPosition()
                {
                    Info = new MapInfo()
                    {
                        Entry = map.Entry,
                        InstanceId = map.InstanceId
                    },
                    Position = pos
                });
            }
        }

        [Command(Permission.House, "Set decor entity.", "setstate")]
        public void HandleSetState(ICommandContext context,
            [Parameter("Decor ID")]
            ulong decorId,
            [Parameter("StandState", ParameterFlags.None, typeof(EnumParameterConverter<StandState>))]
            StandState state)
        {
            Player player = context.InvokingPlayer;
            ResidenceMapInstance map = player?.Map as ResidenceMapInstance;
            Residence res = map?.GetResidenceByZone(player.Zone.Id);
            Decor dec = res?.GetDecor(decorId);
            WorldEntity en = dec?.Entity;
            if (en != null)
            {
                en.StandState = state;
            }
            else
            {
                context.SendError("No such decor or entity!");
            }
        }

        [Command(Permission.House, "Elevator test.", "elevatortestmove")]
        public void HandleElevatorTestMove(ICommandContext context,
            [Parameter("Decor ID")]
            ulong decorId)
        {
            Player player = context.InvokingPlayer;
            ResidenceMapInstance map = player?.Map as ResidenceMapInstance;
            Residence res = map?.GetResidenceByZone(player.Zone.Id);
            Decor dec = res.GetDecor(decorId);
            WorldEntity en = dec.Entity;
            if (dec != null && en != null)
            {
                List<Vector3> nodes = new();
                nodes.Add(en.Position);
                nodes.Add(en.Position + Vector3.UnitY * 20);

                dec.Entity.MovementManager.LaunchSpline(nodes, Game.Entity.Movement.Spline.Static.SplineType.Linear, Game.Entity.Movement.Spline.Static.SplineMode.BackAndForth, 1);
            }
        }


        /*[Command(Permission.House, "Elevator test.", "elevatortest")]
        public void HandleElevatorTest(ICommandContext context,
            [Parameter("Decor ID")]
            ulong decorId)
        {
            Player player = context.InvokingPlayer;
            ResidenceMapInstance map = player?.Map as ResidenceMapInstance;
            Residence res = map?.GetResidenceByZone(player.Zone.Id);
            Decor dec = res.GetDecor(decorId);
            if (dec != null)
            {
                NonPlayer pl = new NonPlayer(GameTableManager.Instance.Creature2.GetEntry(1581), (long)decorId, (byte)dec.PlotIndex);
                //Platform pl = new Platform(GameTableManager.Instance.Creature2.GetEntry(9501), (long)decorId, (byte)dec.PlotIndex);
                map.SetDecorEntityProperties(dec, pl, dec.Position, dec.Rotation);
                pl.InitialiseTemporaryEntity();
                map.AddVisibleEntity(pl);

                List<Vector3> nodes = new();
                nodes.Add(dec.Position);
                nodes.Add(dec.Position + Vector3.UnitY * 5);
                pl.MovementManager.LaunchSpline(nodes, Game.Entity.Movement.Spline.Static.SplineType.Linear, Game.Entity.Movement.Spline.Static.SplineMode.BackAndForth, 1);

                dec.RemoveEntity();
                dec.SetEntity(pl);
            }
        }*/
    }
}
