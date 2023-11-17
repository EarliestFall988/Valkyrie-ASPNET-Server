using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Avalon
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
                Console.WriteLine(Name);
                return 0;
            };
        }
    }
}
