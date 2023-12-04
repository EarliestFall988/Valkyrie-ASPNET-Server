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
            ExpectedParameters = new Dictionary<string, ReferenceTuple>()
            {
                { "a", new ReferenceTuple(StateMachineVariableType.Decimal, false) },
                { "b", new ReferenceTuple(StateMachineVariableType.Decimal, false) },
                { "out", new ReferenceTuple(StateMachineVariableType.Decimal, false, VariableIO.Out) }
            };
        }

        protected override void DefineFunction()
        {
            Name = nameof(DivideNumber);
            Function = () =>
            {
                var a = Parameters["a"].GetDecimal();
                var b = Parameters["b"].GetDecimal();

                if (b == 0)
                {
                    Debug.WriteLine("Cannot divide by zero");
                    return -1;
                }

                Debug.WriteLine($"{a} / {b} = {a / b}");

                Parameters["out"].SetValue(a / b);

                return 1;
            };
        }

    }
}
