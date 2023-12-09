using ValkyrieFSMCore;

namespace Valkyrie_Server.State_Machine.Function_Definitions
{
    public class Respond : FunctionDefinition
    {
        public Respond()
        {
            Name = nameof(Respond);
            DefineFunction();
        }

        void Setup()
        {
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
                    StateMachine.Result = Get<object>("data").ToString() ?? "nothing to respond";
                    StateMachine.FallbackState = StateMachine.States.Find(x => x.Function.GetHashCode() == this.Function.GetHashCode()); //hijack the fallback state so the sm stops normally after this state
                    return 1;
                }

                return -1;
            };
        }
    }
}
