
namespace ValkyrieFSMCore
{
    public class ConvertFromIntegerToDecimal : FunctionDefinition
    {
        public ConvertFromIntegerToDecimal()
        {
            Name = nameof(ConvertFromIntegerToDecimal);
            Description = "Convert an integer type variable to decimal type variable";
            DefineFunction();
        }

        void Setup()
        {
            ExpectedParameters = new Dictionary<string, Parameter>()
            {
                { "integer", new Parameter("integer", VariableIO.In) },
                { "decimal", new Parameter("decimal", VariableIO.Out) }
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
