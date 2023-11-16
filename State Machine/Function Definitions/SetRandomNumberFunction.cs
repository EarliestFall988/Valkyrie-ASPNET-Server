using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Avalon
{
    public sealed class SetRandomNumberFunction : FunctionDefinition
    {

        public SetRandomNumberFunction()
        {
            DefineFunction();
        }

        protected override void DefineFunction()
        {

            Name = "SetRandomNumber";

            ExpectedParameters = new Dictionary<string, ReferenceTuple>()
            {
                { "valueToChange", new ReferenceTuple(StateMachineVariableType.Integer, false) },
                { "min", new ReferenceTuple(StateMachineVariableType.Integer, false) },
                { "max", new ReferenceTuple(StateMachineVariableType.Integer, false) }
            };

            Func<int> func = () =>
            {
                int valueToChange = Parameters["valueToChange"].GetInt();

                int min = Parameters["min"].GetInt();

                int max = Parameters["max"].GetInt();

                Random random = new Random();

                int randomNumber = random.Next(min, max);

                // Console.WriteLine($"The random number is {randomNumber}");

                Parameters["valueToChange"].SetValue(randomNumber.ToString());

                return 1;
            };

            Function = func;
        }
    }

}