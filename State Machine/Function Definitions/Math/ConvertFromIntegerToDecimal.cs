
namespace ValkyrieFSMCore
{
    public class ConvertFromIntegerToDecimal : FunctionDefinition
    {
        public ConvertFromIntegerToDecimal()
        {
            Name = nameof(ConvertFromIntegerToDecimal);
            Setup();
            DefineFunction();
        }

        void Setup()
        {
            Description = "Convert an integer type variable to decimal type variable";
            ExpectedParameters = new Dictionary<string, Parameter>()
            {
                { "integer", new Parameter("integer", io:VariableIO.In) },
                { "decimal", new Parameter("decimal", io:VariableIO.Out) }
            };
        }

        protected override void DefineFunction()
        {
            Function = () =>
            {
                var integer = Get<int>("integer");
                Set("decimal", (float)integer);
                return 1;
            };
        }

    }
}
