using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NexusForever.WorldServer.Script.Creature.Transporters
{


    [Script(28454)]
    public class WalkOnTeleporter28454 : WalkOnTeleporter
    {
        protected override (ulong, ulong)[] pairs
        {
            get
            {
                return [
                (21760, 21761),
            ];
            }
        }
    }
}
