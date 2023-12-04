
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;


namespace Avalon
{
    public sealed class GuessFunction : FunctionDefinition
    {

        public GuessFunction()
        {
            DefineFunction();
        }

        protected override void DefineFunction()
        {

            // ExpectedParameters = new List<StateMachineVariableType>()
            // {
            //     StateMachineVariableType.Text, // name
            //     StateMachineVariableType.Integer // guess
            // };

            ExpectedParameters = new Dictionary<string, Parameter>()
            {
                { "name", new Parameter(StateMachineVariableType.Text) },
                { "guess", new Parameter(StateMachineVariableType.Integer) }
            };

            Name = "Guess";

            Function = () =>
            {

                VariableDefinition<string>? nameVarCheck = Parameters["name"] as VariableDefinition<string>;
                VariableDefinition<int>? guessNumberVarCheck = Parameters["guess"] as VariableDefinition<int>;

                if (nameVarCheck == null || guessNumberVarCheck == null)
                    return -1;

                var name = nameVarCheck.Value;
                var guessNumber = guessNumberVarCheck.Value;

                Debug.WriteLine($"Okay {name}, Guess a number");
                string? number = guessNumber.ToString(); // Console.ReadLine();

                if (number == null || number.Trim() == "")
                    return 0;


                if (number.ToLower().Trim() == "exit") // exit the program
                {
                    return -1;
                }

                bool result = Int32.TryParse(number, out int numberInt);

                if (!result)
                {
                    Debug.WriteLine("That's not a number! Try again!");
                    return 0;
                }


                if (guessNumber == numberInt)
                {
                    Debug.WriteLine("Correct.");
                    return 1;
                }
                else
                {
                    Debug.WriteLine("Incorrect. Try Again");
                    return 0;
                }

            };
        }
    }
}