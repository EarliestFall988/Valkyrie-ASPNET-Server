using System.Diagnostics;

namespace ValkyrieFSMCore
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
                { "a", new Parameter(StateMachineVariableType.Decimal.ToString().ToLower()) },
                { "b", new Parameter(StateMachineVariableType.Decimal.ToString().ToLower()) },
                { "out", new Parameter(StateMachineVariableType.Decimal.ToString().ToLower(), VariableIO.Out) }
            };
        }

        protected override void DefineFunction()
        {
            Name = nameof(DivideNumber);
            Function = () =>
            {
                var a = Get<float>("a");
                var b = Get<float>("b");


                Debug.WriteLine("\t dividing " + a + " / " + b + " " + (a / b).ToString());

                Set("out", a / b);

                return 1;
            };
        }

    }
}
