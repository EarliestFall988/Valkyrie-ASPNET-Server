using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ValkyrieFSMCore
{
    /// <summary>
    /// The Transition is the class that handles the transition between states.
    /// </summary>
    public struct Transition
    {
        /// <summary>
        /// The state that the transition is coming from
        /// </summary>
        /// <value></value>
        public State From { get; set; }

        /// <summary>
        /// The state that the transition is going to
        /// </summary>
        /// <value></value>
        public State To { get; set; }

        /// <summary>
        /// The name of the transition
        /// </summary>
        /// <value></value>
        public string Name { get; set; }


        /// <summary>
        /// the outcome to check
        /// </summary>
        /// <value></value>
        public int Outcome { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="from">
        ///   The state that the transition is coming from
        /// </param>
        /// <param name="to">
        ///  The state that the transition is going to
        /// </param>
        public Transition(State from, State to, string name, int outcome)
        {
            From = from;
            To = to;
            Name = name;
            Outcome = outcome;
        }


        public bool EvaluateCondition(int condition)
        {
            if (condition == Outcome)
            {
                // Console.WriteLine($"[{From.Name}] -> [{To.Name}]");
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}