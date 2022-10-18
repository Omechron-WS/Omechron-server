using NexusForever.Game.Entity;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.RBAC;
using NexusForever.WorldServer.Command.Context;

namespace NexusForever.WorldServer.Command.Handler
{
    [Command(Permission.Boost, "Character Boosts and Unlocks.", "boost")]
    [CommandTarget(typeof(Player))]
    public class BoostCommandCategory : CommandCategory
    {
        [Command(Permission.Boost, "Boosts your character to level 50, restart client for it to take effect.", "level")]
        public void HandleBoostLevel(ICommandContext context)
        {
            Player target = context.GetTargetOrInvoker<Player>();
            if (target.Level < 50)
            {
                target.XpManager.SetLevel(50);
            }
        }

        [Command(Permission.Boost, "Grants some character currencies.", "money")]
        public void HandleBoostMoney(ICommandContext context)
        {
            Player target = context.GetTargetOrInvoker<Player>();
            target.CurrencyManager.CurrencyAddAmount(CurrencyType.Credits, 500000000);
            target.CurrencyManager.CurrencyAddAmount(CurrencyType.Renown, 500000);
            target.CurrencyManager.CurrencyAddAmount(CurrencyType.ElderGems, 500000);
            target.CurrencyManager.CurrencyAddAmount(CurrencyType.CraftingVoucher, 500000);
            target.CurrencyManager.CurrencyAddAmount(CurrencyType.Prestige, 500000);
            target.CurrencyManager.CurrencyAddAmount(CurrencyType.Glory, 500000);
        }

        [Command(Permission.Boost, "Unlock lore.", "lore")]
        public void HandleBoostLore(ICommandContext context)
        {
            Player target = context.GetTargetOrInvoker<Player>();

            target.DatacubeManager.UnlockAllLore();
        }

        [Command(Permission.Boost, "Level boost, currencies, unlock all dyes and unlock all lore.", "all")]
        public void HandleBoostAll(ICommandContext context)
        {
            Player target = context.GetTargetOrInvoker<Player>();

            if (target.Level < 50)
            {
                target.XpManager.SetLevel(50);
            }

            target.Account.GenericUnlockManager.UnlockAll(GenericUnlockType.Dye);

            target.CurrencyManager.CurrencyAddAmount(CurrencyType.Credits, 500000000);
            target.CurrencyManager.CurrencyAddAmount(CurrencyType.Renown, 500000);
            target.CurrencyManager.CurrencyAddAmount(CurrencyType.ElderGems, 500000);
            target.CurrencyManager.CurrencyAddAmount(CurrencyType.CraftingVoucher, 500000);
            target.CurrencyManager.CurrencyAddAmount(CurrencyType.Prestige, 500000);
            target.CurrencyManager.CurrencyAddAmount(CurrencyType.Glory, 500000);

            target.DatacubeManager.UnlockAllLore();
        }
    }
}
