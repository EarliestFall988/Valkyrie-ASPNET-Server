using System.Diagnostics;

namespace Avalon
{
    /// <summary>
    /// Divide Number Function Definition
    /// </summary>
    public class DivideNumber : FunctionDefinition
    {


        public DivideNumber()
        {
            Setup();
            DefineFunction();
        }

        void Setup()
        {
            ExpectedParameters = new Dictionary<string, Parameter>()
            {
                { "a", new Parameter(StateMachineVariableType.Decimal) },
                { "b", new Parameter(StateMachineVariableType.Decimal) },
                { "out", new Parameter(StateMachineVariableType.Decimal, VariableIO.Out) }
            };
        }

        protected override void DefineFunction()
        {
            Name = nameof(DivideNumber);
            Function = () =>
            {
                var x = Parameters["a"];
                var y = Parameters["b"];

                var z = Parameters["out"];

                if (x is VariableDefinition<decimal> aV && y is VariableDefinition<decimal> bV && z is VariableDefinition<decimal> resultV)
                {
                    var b = bV.Value;
                    var a = aV.Value;

                    if (b == 0)
                    {
                        Debug.WriteLine("Cannot divide by zero");
                        return -1;
                    }

                    Debug.WriteLine($"{a} / {b} = {a / b}");

                    resultV.Value = a / b;

                    return 1;
                }

                return -1;
            };
        }

    }
}
