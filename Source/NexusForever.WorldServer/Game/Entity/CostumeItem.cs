using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Game.Entity
{
    public class CostumeItem : ISaveCharacter
    {
        public const byte MaxCostumeItemDyes = 3;

        /// <summary>
        /// Return dye ramp mask generated from supplied dyes.
        /// </summary>
        private void GenerateDyeMask(uint[] dyes)
        {
            int[] ramps = new int[MaxCostumeItemDyes];
            for (var i = 0; i < dyes.Length; i++)
            {
                if (dyes[i] == 0)
                    continue;

                DyeColorRampEntry entry = GameTableManager.Instance.DyeColorRamp.GetEntry(dyes[i]);
                ramps[i] = (int)entry.RampIndex;
            }

            DyeData = (int)((ramps[2] & 0x3FF | 0xFFFFF800) << 20) | ((ramps[1] & 0x3FF) << 10) | (ramps[0] & 0x3FF);
        }

        private void UpdateVisibleDyeData()
        {
            ItemColorSetEntry colorSet = null;
            if (Entry != null)
            {
                ItemDisplayEntry itemDisplay = GameTableManager.Instance.ItemDisplay.GetEntry(Entry.ItemDisplayId);
                uint colorSetId = itemDisplay?.ItemColorSetId ?? 0u;
                if (Entry.ItemColorSetId != 0)
                    colorSetId = Entry.ItemColorSetId;

                if (colorSetId != 0)
                {
                    colorSet = GameTableManager.Instance.ItemColorSet.GetEntry(colorSetId);
                }
            }
            VisibleDyeData = DyeData;
            if(colorSet == null)
            {
                return;
            }

            for(int i = 0; i < MaxCostumeItemDyes; ++i)
            {
                if ((VisibleDyeData & (0x3FF << (10 * i))) == 0)
                {
                    // Default dye
                    uint dye = 0;

                    switch (i)
                    {
                        case 0: dye = colorSet.DyeColorRampId00; break;
                        case 1: dye = colorSet.DyeColorRampId01; break;
                        case 2: dye = colorSet.DyeColorRampId02; break;
                    }
                    DyeColorRampEntry entry = GameTableManager.Instance.DyeColorRamp.GetEntry(dye);

                    VisibleDyeData |= (((int)entry.RampIndex) & 0x3FF) << (10 * i);
                }
            }
        }

        public CostumeItemSlot Slot { get; }
        public Item2Entry Entry { get; private set; }

        public uint ItemId
        {
            get => itemId;
            set
            {
                if (itemId == value)
                    return;

                Entry  = GameTableManager.Instance.Item.GetEntry(value);
                itemId = value;

                saveMask |= CostumeItemSaveMask.ItemId;
            }
        }

        private uint itemId;

        public int DyeData
        {
            get => dyeData;
            private set
            {
                if (dyeData == value)
                    return;

                dyeData = value;
                saveMask |= CostumeItemSaveMask.DyeData;
            }
        }

        public int VisibleDyeData { get; private set; }

        private int dyeData;

        private readonly Costume costume;

        private CostumeItemSaveMask saveMask;

        /// <summary>
        /// Create a new <see cref="CostumeItem"/> from an existing <see cref="CharacterCostumeItemModel"/> database model.
        /// </summary>
        public CostumeItem(Costume costume, CharacterCostumeItemModel model)
        {
            this.costume = costume;
            Slot         = (CostumeItemSlot)model.Slot;
            Entry        = GameTableManager.Instance.Item.GetEntry(model.ItemId);
            itemId       = model.ItemId;
            dyeData      = model.DyeData;
            UpdateVisibleDyeData();
        }

        /// <summary>
        /// Create a new <see cref="CostumeItem"/> from packet <see cref="ClientCostumeSave"/>.
        /// </summary>
        public CostumeItem(Costume costume, ClientCostumeSave.CostumeItem item, CostumeItemSlot slot)
        {
            this.costume = costume;
            Slot         = slot;
            Entry        = GameTableManager.Instance.Item.GetEntry(item.ItemId);
            itemId       = item.ItemId;
            saveMask     = CostumeItemSaveMask.Create;
            SetDyes(item.Dyes);
        }

        public void Save(CharacterContext context)
        {
            if (saveMask == CostumeItemSaveMask.None)
                return;

            if ((saveMask & CostumeItemSaveMask.Create) != 0)
            {
                // costume item doesn't exist in database, all infomation must be saved
                context.Add(new CharacterCostumeItemModel
                {
                    Id      = costume.Owner,
                    Index   = costume.Index,
                    Slot    = (byte)Slot,
                    ItemId  = itemId,
                    DyeData = dyeData
                });
            }
            else
            {
                // costume item already exists in database, save only data that has been modified
                var model = new CharacterCostumeItemModel
                {
                    Id    = costume.Owner,
                    Index = costume.Index,
                    Slot  = (byte)Slot
                };

                EntityEntry<CharacterCostumeItemModel> entity = context.Attach(model);
                if ((saveMask & CostumeItemSaveMask.ItemId) != 0)
                {
                    model.ItemId = itemId;
                    entity.Property(p => p.ItemId).IsModified = true;
                }
                if ((saveMask & CostumeItemSaveMask.DyeData) != 0)
                {
                    model.DyeData = dyeData;
                    entity.Property(p => p.DyeData).IsModified = true;
                }
            }

            saveMask = CostumeItemSaveMask.None;
        }

        public void SetDyes(uint[] dyes)
        {
            GenerateDyeMask(dyes);
            UpdateVisibleDyeData();
        }
    }
}
