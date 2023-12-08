using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ValkyrieFSMCore
{
    public static class StateMachineInfo
    {   
        /// <summary>
        /// the endpoint to connect to the web app and authenticate
        /// </summary>
        public static readonly string AuthEndpoint = "http://localhost:3000/api/auth";


        /// <summary>
        /// the id of the state machine
        /// </summary>
        public static readonly string Id = "93889477-f225-4827-b618-50151f4e49d1";

        /// <summary>
        /// the name of the state machine
        /// </summary>
        public static readonly string Name = "C# Academic Demo";

        /// <summary>
        /// the description what the state machine is used for
        /// </summary>
        public static readonly string Description = "A demo of the visual state machine for the fall 2023 capstone class.";

        /// <summary>
        /// the type of state machine amd/or context it is used in
        /// </summary>
        public static readonly string Type = "C# .Net Core 6 State Machine";
    }
}