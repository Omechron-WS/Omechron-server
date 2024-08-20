using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NexusForever.WorldServer.Script.Creature.Transporters
{
    [Script(26007)]
    [Script(47103)] // Star-Comm Station and nothing else, links to a 26007
    public class WalkOnTeleporter26007 : WalkOnTeleporter
    {
        protected override (ulong, ulong)[] pairs
        {
            get
            {
                return new (ulong, ulong)[] {
                (19318, 19319),
                (19323, 19321),
                (19333, 19335),
                (19931, 19933),
                };
            }
        }
    }
}
