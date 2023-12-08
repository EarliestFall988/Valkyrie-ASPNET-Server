
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ValkyrieFSMCore
{
    /// <summary>
    /// This is so I can fix the bug where I cannot pass pointers to the correct state function. Just google: value vs reference
    /// </summary>
    public struct Parameter : IVariableSignature
    {
        /// <summary>
        /// The type of variable
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Has the parameter been injected successfully?
        /// </summary>
        public bool InjectedSuccessfully { get; private set; }

        public VariableIO IO { get; set; }

        /// <summary>
        /// Key not used for this class.
        /// </summary>
        public string Key { get; set; } = "";

        /// <summary>
        /// the description of the parameter
        /// </summary>
        public string Description { get; set; } = "";

        public Parameter(string type, VariableIO io = VariableIO.In, bool parameterInjectedSuccessfully = false, string description = "")
        {
            Type = type;
            this.InjectedSuccessfully = parameterInjectedSuccessfully;
            this.IO = io;
            this.Description = description;
        }
    }
}