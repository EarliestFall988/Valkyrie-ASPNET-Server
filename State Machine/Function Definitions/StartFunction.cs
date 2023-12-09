using ValkyrieFSMCore;

namespace Valkyrie_Server.State_Machine.Function_Definitions
{
    public sealed class StartFunction : FunctionDefinition
    {
        public StartFunction()
        {
            DefineFunction();
        }

        protected override void DefineFunction()
        {

            //do any setting up needed here if desired

            Function = () =>
            {
                return 1; //boot the process and start normally.
            };
        }
    }
}
