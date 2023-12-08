
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;


namespace ValkyrieFSMCore
{
    public sealed class GreetUserFunction : FunctionDefinition
    {

        public GreetUserFunction()
        {
            DefineFunction();
        }

        protected override void DefineFunction()
        {

            Name = "GreetUser";

            ExpectedParameters = new Dictionary<string, Parameter>()
            {
                { "name", new Parameter(StateMachineVariableType.Text.ToString()) }
            };

            Func<int> GreetUser = () =>
            {

                string? result = "";
                Debug.WriteLine("Hello!");

                while (result == null || result == string.Empty)
                {
                    Debug.WriteLine("What is your Name?");
                    result = "John Doe"; // Console.ReadLine();

                    var nameV = Parameters["name"];

                    if (nameV is VariableDefinition<string> name)
                    {

                        if (result == null || result == string.Empty)
                        {
                            Debug.WriteLine("That's not your name!");
                        }

                        if (result?.ToLower().Trim() == "exit")
                        {
                            return -1;
                        }

                        name.Value = result?.Replace(',', '\'') ?? "";

                    }
                    else
                    {
                        return -1;
                    }
                }

                return 1;
            };

            Function = GreetUser;
        }
    }

}