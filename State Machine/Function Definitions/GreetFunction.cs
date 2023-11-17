
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;


namespace Avalon
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

            ExpectedParameters = new Dictionary<string, ReferenceTuple>()
            {
                { "name", new ReferenceTuple(StateMachineVariableType.Text, false) }
            };

            Func<int> GreetUser = () =>
            {

                string? result = "";
                Debug.WriteLine("Hello!");

                while (result == null || result == string.Empty)
                {
                    Debug.WriteLine("What is your Name?");
                    result = "John Doe"; // Console.ReadLine();

                    if (result == null || result == string.Empty)
                    {
                        Debug.WriteLine("That's not your name!");
                    }

                    if (result?.ToLower().Trim() == "exit")
                    {
                        return -1;
                    }

                    // bool success = stateMachine.WriteValue(0, 0, result?.Replace(',', '\'') ?? "");

                    Parameters["name"].SetValue(result?.Replace(',', '\'') ?? "");

                    // if (!success)
                    // {
                    //     Debug.WriteLine("could not write to store");
                    //     return -1;
                    // }
                }

                return 1;
            };

            Function = GreetUser;
        }
    }

}