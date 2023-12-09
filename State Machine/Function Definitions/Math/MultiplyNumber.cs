using System.Diagnostics;

namespace ValkyrieFSMCore
{
    /// <summary>
    /// Multiply Number Function Definition
    /// </summary>
    public class MultiplyNumber : FunctionDefinition
    {

        public MultiplyNumber()
        {
            Setup();
            DefineFunction();
        }

        void Setup()
        {
            ExpectedParameters = new Dictionary<string, Parameter>()
            {
                { "a", new Parameter(StateMachineVariableType.Decimal.ToString().ToLower()) },
                { "b", new Parameter(StateMachineVariableType.Decimal.ToString().ToLower()) },
                { "out", new Parameter(StateMachineVariableType.Decimal.ToString().ToLower(), VariableIO.Out) }
            };
        }

        protected override void DefineFunction()
        {
            Name = nameof(MultiplyNumber);
            Function = () =>
            {
                var x = Parameters["a"];
                var y = Parameters["b"];

                var z = Parameters["out"];

                if (x is VariableDefinition<decimal> aV && y is VariableDefinition<decimal> bV && z is VariableDefinition<decimal> resultV)
                {
                    var b = bV.Value;
                    var a = aV.Value;

                    Debug.WriteLine($"{a} * {b} = {a * b}");

                    resultV.Value = a * b;

                    return 1;
                }

                return -1;
            };
        }
    }
}
