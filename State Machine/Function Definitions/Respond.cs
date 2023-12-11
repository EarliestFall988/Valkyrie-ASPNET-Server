using System.Diagnostics;
using System.Text.Json;

using ValkyrieFSMCore;

namespace Valkyrie_Server.State_Machine.Function_Definitions
{
    public class Respond : FunctionDefinition
    {
        public Respond()
        {
            Name = nameof(Respond);
            Setup();
            DefineFunction();
        }

        void Setup()
        {
            Description = "Responds with the data provided and terminates the process.";
            ExpectedParameters = new Dictionary<string, Parameter>()
            {
                { "data", new Parameter("any") }
            };
        }

        protected override void DefineFunction()
        {
            Function = () =>
            {

                if (StateMachine != null)
                {

                    var res = Parameters["data"];


                    Debug.WriteLine($"Responding with {res.ToJSON()}");

                    StateMachine.Result = res.ToJSON();

                    var state = StateMachine.States.Find(x => x.Function.GetHashCode() == this.Function.GetHashCode());
                    if (state != null)
                        state.FallbackState = true; //hijack the fallback state prop so the sm stops normally after this state
                    return 1;
                }

                return -1;
            };
        }
    }
}
