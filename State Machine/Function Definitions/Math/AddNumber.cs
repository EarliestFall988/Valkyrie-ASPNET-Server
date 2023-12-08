using System.Diagnostics;

namespace ValkyrieFSMCore
{
    public class AddNumber : FunctionDefinition
    {

        public AddNumber()
        {
            Setup();
            DefineFunction();
        }

        void Setup()
        {
            ExpectedParameters = new Dictionary<string, Parameter>()
            {
                { "a", new Parameter(StateMachineVariableType.Decimal.ToString()) },
                { "b", new Parameter(StateMachineVariableType.Decimal.ToString()) },
                { "out", new Parameter(StateMachineVariableType.Decimal.ToString(), VariableIO.Out) }
            };
        }

        protected override void DefineFunction()
        {
            Name = nameof(AddNumber);
            Function = () =>
            {
                var x = Parameters["a"];
                var y = Parameters["b"];

                var z = Parameters["out"];

                if (x is VariableDefinition<decimal> a && y is VariableDefinition<decimal> b && z is VariableDefinition<decimal> result)
                {

                    Debug.WriteLine($"{a} + {b} = {a.Value + b.Value}");

                    result.Value = a.Value + b.Value;

                    return 1;
                }

                return -1;
            };
        }

    }
}
