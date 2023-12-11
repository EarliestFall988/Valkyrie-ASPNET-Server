using ValkyrieFSMCore;

namespace Valkyrie_Server.State_Machine.Function_Definitions.Math
{
    public class ConvertFromDecimalToInteger : FunctionDefinition
    {
        public ConvertFromDecimalToInteger()
        {
            Name = nameof(ConvertFromDecimalToInteger);
            Setup();
            DefineFunction();
        }

        void Setup()
        {
            Description = "Convert a decimal type variable to integer type variable. Rounds up if the value is 0.5 or greater.";
            ExpectedParameters = new Dictionary<string, Parameter>()
            {
                { "decimal", new Parameter("decimal", io:VariableIO.In) },
                { "integer", new Parameter("integer", io:VariableIO.Out) },
            };
        }

        protected override void DefineFunction()
        {
            Function = () =>
            {
                var dec = Get<float>("decimal");
                int rounded = (int)MathF.Round(dec, 0, MidpointRounding.AwayFromZero);

                Set("integer", rounded);
                return 1;
            };
        }
    }
}
