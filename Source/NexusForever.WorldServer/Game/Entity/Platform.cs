using NexusForever.Database.World.Model;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.CSI;
using NexusForever.WorldServer.Game.Entity.Network;
using NexusForever.WorldServer.Game.Entity.Network.Command;
using NexusForever.WorldServer.Game.Entity.Network.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Map;
using NexusForever.WorldServer.Game.Quest.Static;
using NexusForever.WorldServer.Game.Reputation.Static;
using NexusForever.WorldServer.Game.Spell;
using NexusForever.WorldServer.Network.Message.Model;
using NexusForever.WorldServer.Script;
using System.Linq;
using System.Numerics;

namespace NexusForever.WorldServer.Game.Entity
{
    [DatabaseEntity(EntityType.Platform)]
    public class Platform : WorldEntity
    {
        public Platform()
            : base(EntityType.Platform)
        {
        }

        public Platform(uint creatureId)
            : base(EntityType.Platform)
        {
            CreatureId = creatureId;

            // temp
            DisplayInfo = 24413;
            SetBaseProperty(Property.BaseHealth, 101f);

            SetStat(Stat.Health, 101u);
        }

        public Platform(Creature2Entry entry, long propId, ushort plugId)
            : base(EntityType.Platform)
        {
            CreatureId = entry.Id;
            ActivePropId = propId;
            WorldSocketId = plugId;
            Faction1 = (Faction)entry.FactionId;
            Faction2 = (Faction)entry.FactionId;

            Creature2DisplayGroupEntryEntry displayGroupEntry = GameTableManager.Instance.Creature2DisplayGroupEntry.Entries.FirstOrDefault(i => i.Creature2DisplayGroupId == entry.Creature2DisplayGroupId);
            if (displayGroupEntry != null)
                DisplayInfo = displayGroupEntry.Creature2DisplayInfoId;

            Creature2OutfitGroupEntryEntry outfitGroupEntry = GameTableManager.Instance.Creature2OutfitGroupEntry.Entries.FirstOrDefault(i => i.Creature2OutfitGroupId == entry.Creature2OutfitGroupId);
            if (outfitGroupEntry != null)
                OutfitInfo = (ushort)outfitGroupEntry.Creature2OutfitInfoId;

            SetBaseProperty(Property.BaseHealth, 101f);

            SetStat(Stat.Health, 101u);

            CreateFlags |= EntityCreateFlag.SpawnAnimation;
        }

        public override void Initialise(EntityModel model)
        {
            base.Initialise(model);
            QuestChecklistIdx = model.QuestChecklistIdx;
            MaxHealth = 101u;
            ModifyHealth(MaxHealth);

            ScriptManager.Instance.GetScript<CreatureScript>(CreatureId)?.OnCreate(this);
        }

        protected override IEntityModel BuildEntityModel()
        {
            return new PlatformEntityModel
            {
                CreatureId        = CreatureId,
                QuestChecklistIdx = QuestChecklistIdx
            };
        }

        public override ServerEntityCreate BuildCreatePacket()
        {
            ServerEntityCreate serverEntityCreate = base.BuildCreatePacket();

            if (ActivePropId > 0)
            {
                serverEntityCreate.WorldPlacementData = new ServerEntityCreate.WorldPlacement
                {
                    Type = 1,
                    SocketId = WorldSocketId
                };
            }

            return serverEntityCreate;
        }

        public override void OnAddToMap(BaseMap map, uint guid, Vector3 vector)
        {
            base.OnAddToMap(map, guid, vector);
            MovementManager.AddCommand(new SetStateDefaultCommand
            {
                Strafe = true
            }, true);
            //MovementManager.AddCommand(new SetModeCommand
            //{
            //    Mode = 3
            //}, true);
        }
    }
}
