
namespace Avalon
{
    /// <summary>
    /// The State Machine is the main class that handles the state machine.
    /// </summary>
    public class StateMachine
    {
        /// <summary>
        /// CSV data that the state machine is operating on.
        /// </summary>
        /// <value></value>
        public string Store { get; set; } = "";

        /// <summary>
        /// The list of states that this machine can be in.
        /// </summary>
        /// <typeparam name="State"></typeparam>
        /// <returns></returns>
        public List<State> States = new List<State>();

        public Dictionary<string, KeyTypeDefinition> Variables = new Dictionary<string, KeyTypeDefinition>();

        /// <summary>
        /// The current state of the machine.
        /// </summary>
        /// <value></value>
        public State? CurrentState { get; set; }

        /// <summary>
        /// The state that the machine will fall back to if it cannot transition to the next state, or can be used as a 'throw error' state
        /// </summary>
        /// <value></value>
        public State? FallbackState { get; set; }

        /// <summary>
        /// The Initial State
        /// </summary>
        /// <value></value>
        public State? InitialState { get; set; }

        /// <summary>
        /// is the state machine running?
        /// </summary>
        public bool IsRunning { get; set; } = false;

        /// <summary>
        /// Creates a new state machine with the given initial state and fallback state.
        /// </summary>
        /// <param name="initialState"></param>
        /// <param name="fallbackState"></param>
        public StateMachine()
        {

        }

        /// <summary>
        /// Adds a state to the state machine.
        /// </summary>
        /// <param name="state"></param>
        public void AddState(State state)
        {
            States.Add(state);
        }

        public string Result { get; set; } = "";

        private void RunStateMachine()
        {

            // if (Store == string.Empty)
            // {
            //     Console.ForegroundColor = ConsoleColor.Red;
            //     Console.WriteLine("The Store is empty");
            //     Console.ResetColor();
            //     return;
            // }

            // if (FallbackState == null || InitialState == null)
            // {
            //     Console.ForegroundColor = ConsoleColor.Red;
            //     Console.WriteLine("the fallback state or initial state is null");
            //     Console.ResetColor();
            //     return;
            // }

            CurrentState = InitialState; // load the first state

            if (CurrentState != null && !CurrentState.active)
                CurrentState.active = true;

            Console.WriteLine("\t>Start\n\n");

            IsRunning = true;

            while (CurrentState != FallbackState)
            {
                Evaluate();
            }

            Console.WriteLine("\n\n\t>End");
        }

        /// <summary>
        /// Evaluates the transitions of the current state.
        /// </summary>
        public void Evaluate()
        {

            if (!IsRunning)
            {
                Console.WriteLine("the state machine is not running - falling back.");
                CurrentState = FallbackState;
                return;
            }

            if (CurrentState == null)
            {
                // throw new NullReferenceException("the current state is null");
                Console.WriteLine("the current state is null, booting to start state.");
                CurrentState = InitialState;
            }

            if (CurrentState == null)
                throw new NullReferenceException("cannot boot to start state");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"------------------[{CurrentState.Name}]------------------");
            Console.ResetColor();
            Console.WriteLine();

            Console.WriteLine("current state: " + CurrentState.Name);

            if (CurrentState == null)
                throw new NullReferenceException("the current state is null");

            try
            {
                int result = CurrentState.Function();

                if (CurrentState == FallbackState)
                {
                    Console.WriteLine("the current state is the fallback state, exiting.");
                    return;
                }

                // Console.WriteLine("Result: " + result);
                State? nextState = CurrentState.EvaluateTransitions(result);


                // Console.WriteLine("Next State: " + nextState?.Name);

                // Console.WriteLine(Store);

                if (nextState != null && nextState != CurrentState)
                {
                    // Console.WriteLine("transitioning to: " + nextState.Name);

                    CurrentState.active = false;
                    CurrentState = nextState;
                    CurrentState.active = true;
                }


                if (nextState == null)
                {
                    Console.WriteLine($"cannot transition to next state from {CurrentState.Name} -> result: {result}; exiting via fallback state.");
                    CurrentState = FallbackState;
                }
            }
            catch (Exception ex)
            {
                if (CurrentState == null)
                    throw new NullReferenceException("the current state is null");

                Console.WriteLine($"Error in state {CurrentState.Name}: {ex.Message}");
                CurrentState = FallbackState;
            }
        }

        /// <summary>
        /// Write a value to the store
        /// </summary>
        /// <param name="col">the column</param>
        /// <param name="row">the row</param>
        /// <param name="data">the data</param>
        /// <returns>returns true if the value is written, false if there was an error</returns>
        public bool WriteValue(int col, int row, string data)
        {

            if (data.Contains(","))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: Cannot write a value to the store with a comma");
                Console.ResetColor();
                return false;
            }

            string[] lines = Store.Split("\n");
            if (lines != null)
            {
                string line = lines[row];
                string[] cells = line.Split(",");

                if (cells != null)
                {
                    cells[col] = data;
                    lines[row] = string.Join(',', cells);
                    string result = string.Join('\n', lines);
                    Store = result;
                    // Console.WriteLine(data);
                    return true;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: Cells is null for this line.");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: This line does not exist.");
                Console.ResetColor();
            }

            return false;
        }

        /// <summary>
        /// Evaluates the transitions of the current state. (Note that commas cannot be used in the store)
        /// </summary>
        /// <param name="col">the column</param>
        /// <param name="row">the row</param>
        /// <returns>returns the value from the csv</returns>
        public bool ReadKey(int col, int row, out string result)
        {
            var lines = Store.Split("\n");
            if (lines.Length > row)
            {
                var line = lines[row];
                var cells = line.Split(",");

                if (cells.Length > col)
                {
                    var cell = cells[col];
                    result = cell;
                    return true;
                }
            }

            result = "";
            return false;
        }

        /// <summary>
        /// Run the state machine
        /// </summary>
        /// <param name="machine">the state machine</param>

        public void Boot()
        {
            Console.WriteLine("\n\tRun Program?\n");
            string? result = Console.ReadLine();

            if (result != null && (result.Trim().ToLower() == "yes" || result.Trim().ToLower() == "y"))
            {
                RunStateMachine();
            }
            else
            {
                Console.WriteLine("\n\t>Canceled.");
            }

            Console.WriteLine("\n\n\t>Exiting...");

        }
    }
}