using ValkyrieFSMCore;

namespace Valkyrie_Server.State_Machine.Function_Definitions
{
    public class ExitFunction : FunctionDefinition
    {

        public ExitFunction()
        {
            DefineFunction();
        }

        protected override void DefineFunction()
        {

            ///close or resolve anything that might be neccesary for the process to complete here...

            Function = () =>
            {
                return 1; //exit the process
            };
        }
    }
}
