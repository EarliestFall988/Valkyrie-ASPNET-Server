using System.Diagnostics;

namespace Avalon
{
    /// <summary>
    /// Power Number Function Definition
    /// </summary>
    public class PowNumber : FunctionDefinition
    {

        public PowNumber()
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
            Name = nameof(PowNumber);
            Function = () =>
            {
                var a = Parameters["a"].GetDecimal();
                var b = Parameters["b"].GetDecimal();

                var pow = Math.Pow((double)a, (double)b);

                Debug.WriteLine($"{a} to the power of {b} = {pow}");

                Parameters["out"].SetValue((decimal)pow);

                return 1;
            };
        }
    }
}
