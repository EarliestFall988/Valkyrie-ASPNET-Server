
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Avalon
{
    /// <summary>
    /// This is so I can fix the bug where I cannot pass pointers to the correct state function. Just google: value vs reference
    /// </summary>
    public class ReferenceTuple
    {
        public StateMachineVariableType type { get; set; }
        public bool applied { get; set; }

        public ReferenceTuple(StateMachineVariableType type, bool applied, string description = "")
        {
            this.type = type;
            this.applied = applied;
        }
    }
}