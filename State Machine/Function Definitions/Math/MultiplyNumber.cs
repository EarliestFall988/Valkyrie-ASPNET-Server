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


                var a = Get<float>("a");
                var b = Get<float>("b");

                var result = a * b;

                Debug.WriteLine("a: " + a + " b: " + b + " result: " + result);

                Set<float>("out", result);


                return 1;
            };
        }
    }
}
