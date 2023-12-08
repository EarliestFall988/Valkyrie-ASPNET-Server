using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ValkyrieFSMCore
{
    /// <summary>
    /// Set a random number
    /// </summary>
    public sealed class SetRandomNumberFunction : FunctionDefinition
    {

        public SetRandomNumberFunction()
        {
            Setup();
            DefineFunction();
        }

        void Setup()
        {
            Name = "SetRandomNumber";
            Description = "Set a random number within the range of min and max";

            ExpectedParameters = new Dictionary<string, Parameter>()
            {
                { "valueToChange", new Parameter(StateMachineVariableType.Integer.ToString()) },
                { "min", new Parameter(StateMachineVariableType.Integer.ToString()) },
                { "max", new Parameter(StateMachineVariableType.Integer.ToString()) }
            };
        }

        protected override void DefineFunction()
        {

            Func<int> func = () =>
            {

                Random random = new Random();
                int randomNumber = random.Next(Get<int>("min"), Get<int>("max"));
                bool result = TrySet("valueToChange", randomNumber);


                return result == true ? 1 : -1;
            };

            Function = func;
        }
    }

}