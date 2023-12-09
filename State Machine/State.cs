
namespace ValkyrieFSMCore
{
    /// <summary>
    /// The State Machine is the main class that handles the state machine.
    /// </summary>
    public class State
    {
        /// <summary>
        /// The list of transitions that this state can make.
        /// </summary>
        /// <typeparam name="Transition">The Transition</typeparam>
        /// <returns></returns>
        public List<Transition> Transitions = new List<Transition>();


        /// <summary>
        /// The name of the state
        /// </summary>
        /// <value></value>
        public string Name { get; set; }

        /// <summary>
        /// The job performed by the state
        /// </summary>
        /// <value></value>
        public Func<int> Function { get; set; }

        /// <summary>
        /// Is the state active?
        /// </summary>
        /// <value></value>
        public bool active { get; set; }

        /// <summary>
        /// Store the data here for the state and function to act on.
        /// </summary>
        /// <value></value>
        public string Store { get; set; } = "";

        /// <summary>
        /// Is this the initial state?
        /// </summary>
        /// <value></value>
        public bool InitialState { get; set; }


        /// <summary>
        /// Is this the fallback state?
        /// </summary>
        /// <value></value>
        public bool FallbackState { get; set; }

        /// <summary>
        /// Creates a new state with the given name.
        /// </summary>
        /// <param name="name"></param>
        public State(string name, Func<int> stateJob, string store = "")
        {
            Name = name;
            active = false;
            Function = stateJob;
            Store = store;
        }

        public State? EvaluateTransitions(int outcome)
        {

            // Console.WriteLine($"Evaluating Transitions for {Name} with outcome {outcome}");
            Console.WriteLine(Transitions.Count);

            foreach (var x in Transitions)
            {
                Console.WriteLine(x.Name);
                var result = x.EvaluateCondition(outcome);
                if (result)
                {
                    return x.To;
                }
            }

            return null;
        }

        public override string ToString()
        {
            string isActive = active == true ? "Active" : "Inactive";

            return $"{Name} {isActive} Transitions: {Transitions.Count}";
        }
    }
}