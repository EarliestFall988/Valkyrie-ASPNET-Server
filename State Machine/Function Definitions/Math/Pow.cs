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
                { "a", new Parameter(StateMachineVariableType.Decimal.ToString().ToLower()) },
                { "b", new Parameter(StateMachineVariableType.Decimal.ToString().ToLower()) },
                { "out", new Parameter(StateMachineVariableType.Decimal.ToString().ToLower(), VariableIO.Out) }
            };
        }

        protected override void DefineFunction()
        {
            Name = nameof(PowNumber);
            Function = () =>
            {
                var a = Get<float>("a");
                var b = Get<float>("b");


                var result = (float)Math.Pow(a, b);

                Debug.WriteLine("a: " + a + " b: " + b + " result: " + result);

                Set<float>("out", result);

                return 1;

            };
        }
    }
}
