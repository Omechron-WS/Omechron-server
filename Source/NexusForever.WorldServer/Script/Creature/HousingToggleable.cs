using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network.Message.Model;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Script.Creature
{
    // housingDecorTypeId 30
    [Script(59496)] // Arch (White, Wooden)
    [Script(59497)] // Fence Gate (Picket)
    [Script(65845)] // Chain-Link Gate
    [Script(65852)] // Housing - Decor - Doors - Gothic Gate
    [Script(65853)] // Elegant Gate (Aurin)
    [Script(65867)] // Energy Barrier (Ikthian)
    [Script(65868)] // Laser Fence (Ikthian)
    [Script(65869)] // Laser Fence
    [Script(73486)] // Gate - Decor - Housing Active Prop - CLD

    // housingDecorTypeId 59
    [Script(65852)] // Housing - Decor - Doors - Gothic Gate
    [Script(70052)] // Housing - Decor - Activated - Door
    [Script(72338)] // Housing Door
    [Script(73038)] // Caution Gate (Exile) - Housing Active Prop
    [Script(73077)] // Chain-Link Gate (Protostar) - Decor - Housing Active Prop - CLD
    [Script(73087)] // Stone Door (Torine) - Decor - Housing Active Prop - CLD
    [Script(73103)] // Osun Door - Decor - Housing Active Prop - CLD
    [Script(75398)] // Redmoon Door (Circular) - Decor - Housing Active Prop
    [Script(75400)] // Redmoon Door (Square) - Decor - Housing Active Prop

    // lights
    [Script(26348)] // Candle (Medium)
    [Script(59484)] // Candle (Tall)
    [Script(59485)] // Candle (Short)
    [Script(59487)] // Hanging Lamp (Aurin)
    [Script(59488)] // Yellow Sconce (Aurin)
    [Script(59489)] // Cage Light
    [Script(59491)] // Skull Candle Holder
    [Script(59492)] // Hanging Spotlight
    [Script(59493)] // Robot Lamp (Marauder)
    [Script(59494)] // Glass Sconce (Orange)
    [Script(59495)] // Ritual Candle Circle
    [Script(59491)] // Skull Candle Holder (Draken)
    [Script(72359)] // Icefrost Candles
    [Script(72358)] // Frostglow Lantern
    [Script(72360)] // Frostforged Candles
    [Script(72357)] // Osun Battle-Lantern
    [Script(72786)] // Frostforged Brazier
    [Script(72845)] // Frostforged Watchfire
    [Script(75399)] // Wall Sconce (Redmoon)

    // other
    [Script(65856)] // Control Panel (Oscillating Display)
    [Script(65858)] // Hookah (Lopp)
    [Script(65859)] // Lever Switch
    [Script(65860)] // Display (Marauder)
    [Script(65861)] // Sewer Entrance
    [Script(65862)] // Multi-Screen Display (Marauder)
    [Script(65863)] // Holographic Bust (Exile)
    [Script(65864)] // Speaker
    [Script(65870)] // Generator (Exile)
    [Script(65871)] // Generator (Dominion)
    [Script(65872)] // Generator (Exile)
    [Script(65874)] // Cryo Pod (Arkship)
    [Script(65875)] // Laboratory Display (Eldan)
    [Script(65876)] // Wall-Mounted Viewscreen (Eldan)
    [Script(65881)] // Medical Station (Exile)
    [Script(65877)] // Slave Cage (Marauder)
    [Script(65878)] // Functioning Cryo Pod (Dominion)
    [Script(65857)] // Skeech Goodie Bag
    [Script(73039)] // Weapons Crate (Exile)
    [Script(73164)] // Laser Scanner (Dominion)
    [Script(75900)] // Starfall Cupcake Year 3

    public class HousingToggleable : CreatureScript
    {
        private static Dictionary<ulong, List<StandState>> stateProfiles = buildStateProfiles();
        private List<StandState> states;

        public override void OnCreate(WorldEntity me)
        {
            base.OnCreate(me);

            states = GetStandStates(me);
            me.StandState = states[0];
        }

        public List<StandState> GetStandStates(WorldEntity me)
        {
            if (stateProfiles.TryGetValue(me.CreatureId, out var stateList))
            {
                return stateList;
            }

            return new List<StandState>() { StandState.State0, StandState.State1 }; // default
        }

        private static Dictionary<ulong, List<StandState>> buildStateProfiles()
        {
            Dictionary<ulong, List<StandState>> profiles = new();
            List<StandState> sl;

            sl = new() { StandState.State1, StandState.State0 }; // lights
            profiles.Add(26348, sl);
            profiles.Add(59484, sl);
            profiles.Add(59485, sl);
            profiles.Add(59487, sl);
            profiles.Add(59488, sl);
            profiles.Add(59489, sl);
            profiles.Add(59491, sl);
            profiles.Add(59493, sl);
            profiles.Add(59494, sl);
            profiles.Add(65881, sl); // Medical Station (Exile)

            sl = new() { StandState.State1, StandState.State2 };
            profiles.Add(65857, sl); // Skeech Goodie Bag
            profiles.Add(73039, sl); // Weapons Crate (Exile)

            sl = new() { StandState.Stand, StandState.State0, StandState.State1, StandState.State2 };
            profiles.Add(59492, sl); // Hanging Spotlight

            sl = new() { StandState.State0, StandState.State1, StandState.State2, StandState.State3 };
            profiles.Add(65858, sl); // Hookah (Lopp)
            profiles.Add(65863, sl); // Holographic Bust (Exile)

            sl = new() { StandState.State1, StandState.State2, StandState.State0 }; // yellow-blue-off
            profiles.Add(72786, sl); // Frostforged Brazier
            profiles.Add(72845, sl); // Frostforged Watchfire

            sl = new() { StandState.State0, StandState.State1, StandState.State2 };
            profiles.Add(65875, sl); // Laboratory Display (Eldan)
            profiles.Add(65877, sl); // Slave Cage (Marauder)

            sl = new() { StandState.State2, StandState.State0, StandState.State1 };
            profiles.Add(65876, sl); // Wall-Mounted Viewscreen (Eldan)

            sl = new() { StandState.Stand, StandState.State0 };
            profiles.Add(75399, sl); // Wall Sconce (Redmoon)

            return profiles;
        }

        public override void OnActivateSuccess(WorldEntity me, WorldEntity activator)
        {
            base.OnActivateSuccess(me, activator);

            // TODO: Add cooldown

            int index = (states.FindIndex(e => e == me.StandState) + 1) % states.Count; // if not found, go back to first state in the list.
            me.StandState = states[index];

            // Emit from Player due to way Decor Entities are tracked on the map being... different.
            activator.EnqueueToVisible(new ServerEmote
            {
                Guid = me.Guid,
                StandState = me.StandState
            }, true);
        }
    }
}
