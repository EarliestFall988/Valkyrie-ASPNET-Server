using System.Diagnostics;

namespace ValkyrieFSMCore
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
            ExpectedParameters = new Dictionary<string, Parameter>()
            {
                { "a", new Parameter(StateMachineVariableType.Decimal.ToString()) },
                { "b", new Parameter(StateMachineVariableType.Decimal.ToString()) },
                { "out", new Parameter(StateMachineVariableType.Decimal.ToString(), VariableIO.Out) }
            };
        }

        protected override void DefineFunction()
        {
            Name = nameof(PowNumber);
            Function = () =>
            {
                var x = Parameters["a"];
                var y = Parameters["b"];

                var z = Parameters["out"];

                if (x is VariableDefinition<decimal> aV && y is VariableDefinition<decimal> bV && z is VariableDefinition<decimal> resultV)
                {

                    var b = bV.Value;
                    var a = aV.Value;

                    var powResult = Math.Pow((double)a, (double)b);

                    Debug.WriteLine($"{a} to the power of {b} = {powResult}");

                    resultV.Value = (decimal)powResult;
                    return 1;
                }

                return -1;

            };
        }
    }
}
