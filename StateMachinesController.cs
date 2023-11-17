using Avalon;

using System.Diagnostics;

namespace Valkyrie_Server
{
    /// <summary>
    /// Executes the state machines.
    /// </summary>
    public sealed class StateMachinesController
    {
        /// <summary>
        /// The list of machines that are being handled by this task handler.
        /// </summary>
        private Dictionary<string, StateMachine> Machines { get; set; } = new Dictionary<string, StateMachine>();

        /// <summary>
        /// get the active machines
        /// </summary>
        public StateMachine[] GetMachines => Machines.Values.ToArray();

        /// <summary>
        /// Is the state machine task handler running?
        /// </summary>
        public bool Running => TickThread != null && TickThread.IsAlive;

        /// <summary>
        /// The tick thread.
        /// </summary>
        private Thread? TickThread { get; set; }

        /// <summary>
        /// The state machine builder.
        /// </summary>
        StateMachineBuilder builder = new StateMachineBuilder();

        /// <summary>
        /// Is the state machine ticking?
        /// </summary>
        public bool IsTicking => TickThread != null && TickThread.IsAlive;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public StateMachinesController()
        {
            //StartThread();
        }

        /// <summary>
        /// Start the state machine.
        /// </summary>
        /// <param name="id">the id of the machine</param>
        /// <param name="machine">the state machine</param>
        public void AddMachine(string id, string instructions)
        {

            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException("id");

            if (string.IsNullOrEmpty(instructions))
                throw new ArgumentNullException("instructions");


            var machine = builder.ParseInstructionsJSON(instructions);
            Machines.Add(id, machine);

            //Debug.WriteLine("added machine: " + id);
        }

        /// <summary>
        /// Evaluate each state machine once.
        /// </summary>
        private void Tick()
        {
            Debug.WriteLine("tick " + Machines.Count);

            foreach (var x in Machines)
            {
                if (x.Value.CurrentState != x.Value.FallbackState)
                {
                    try
                    {
                        Debug.WriteLine("Evaluating");
                        x.Value.Evaluate();
                    }
                    catch (Exception ex)
                    {
                        x.Value.IsRunning = false;
                        x.Value.CurrentState = x.Value.FallbackState;



                        try
                        {
                            x.Value.Evaluate();
                        }
                        catch (Exception ex2)
                        {
                            x.Value.Result = "bru - Error running: " + ex2.Message;
                        }


                        x.Value.Result = "Error running: " + ex.Message;
                    }
                }
            }

            try
            {
                Thread.Sleep(100); //force the thread to sleep so other threads can execute (kinda like a video game serverside tick).
                Tick();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error ticking: " + ex.Message);
            }
        }

        /// <summary>
        /// Check if the machine is complete.
        /// </summary>
        /// <param name="id">the id to check</param>
        /// <returns>returns a tuple with the first value being a bool indicating if the machine is complete and the second value being the result of the machine</returns>
        public (bool complete, string result) HandleStatus(string id)
        {
            if (!Machines.ContainsKey(id))
            {
                return (false, "does not contain the key");
            }

            if (Machines[id].CurrentState == Machines[id].FallbackState)
            {
                Machines[id].IsRunning = false;
                string res = Machines[id].Result;
                Machines.Remove(id);


                if (Machines.Count == 0)
                {
                    Kill();
                }

                return (true, res);
            }

            return (false, "not complete");
        }

        /// <summary>
        /// Kill the state machine if it took too long..
        /// </summary>
        /// <param name="id">the id of the state machine..</param>
        /// <returns></returns>
        public State? KillStateMachineProcess(string id)
        {

            Debug.WriteLine("removing state machine " + id);

            if (Machines.ContainsKey(id))
            {
                Machines[id].IsRunning = false;
                var machine = Machines[id];

                //could be dangerous...
                machine.CurrentState = machine.FallbackState;
                machine.Evaluate();
                //

                Machines.Remove(id);

                Debug.WriteLine("state machine removed");

                Kill();

                if (Machines.Count > 0)
                    Boot();

                return machine.CurrentState;
            }

            return null;
        }

        /// <summary>
        /// Kill the ticking thread.
        /// </summary>
        public void Kill()
        {

            if (TickThread != null && TickThread.IsAlive)
            {
                //KILL the thread...
                try
                {
                    TickThread.Interrupt();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error killing thread: " + ex.Message);
                }
                TickThread.Join();
                TickThread = null;

                Debug.WriteLine("killed thread");
            }
        }

        /// <summary>
        /// Start the tick thread.
        /// </summary>
        public void Boot()
        {

            Debug.WriteLine("starting thread");

            //boot
            TickThread = new Thread(Tick);
            TickThread.IsBackground = true;
            TickThread.Start();
        }
    }
}
