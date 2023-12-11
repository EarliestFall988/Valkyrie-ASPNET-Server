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
                { "a", new Parameter(StateMachineVariableType.Decimal.ToString().ToLower()) },
                { "b", new Parameter(StateMachineVariableType.Decimal.ToString().ToLower()) },
                { "out", new Parameter(StateMachineVariableType.Decimal.ToString().ToLower(), io:VariableIO.Out) }
            };
        }

        protected override void DefineFunction()
        {
            Name = nameof(AddNumber);
            Function = () =>
            {
                var a = Get<float>("a");
                var b = Get<float>("b");


                Debug.WriteLine("\t adding " + a + " + " + b + " " + (a + b).ToString());

                Set("out", a + b);

                return 1;


            };
        }

    }
}
