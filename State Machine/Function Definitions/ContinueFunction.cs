using System.Diagnostics;

namespace ValkyrieFSMCore
{
    public sealed class ContinueFunction : FunctionDefinition
    {
        public ContinueFunction()
        {
            DefineFunction();
        }

        protected override void DefineFunction()
        {
            Name = "Continue";
            Function = () =>
            {
                if (StateMachine != null)
                    StateMachine.Result = "yayayayaya!!";
                else
                    Debug.WriteLine("state machine is null");

                Debug.WriteLine("\n\tEvaluating default function!!!");
                return 0;
            };
        }
    }
}
