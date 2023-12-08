
namespace ValkyrieFSMCore.WM
{
    public class SplitSector : FunctionDefinition
    {

        public SplitSector()
        {
            Setup();
            DefineFunction();
        }

        void Setup()
        {
            Name = "SplitSector";
            ExpectedParameters = new Dictionary<string, Parameter>()
            {
                { "sector", new Parameter("sector", VariableIO.In) },
                { "id", new Parameter("string", VariableIO.Out) },
                { "name", new Parameter("string", VariableIO.Out) },
            };
        }

        protected override void DefineFunction()
        {
            Function = () =>
            {
                var sector = Get<Sector>("sector");

                if (sector != null)
                {
                    Set("id", sector.Id);
                    Set("name", sector.Name);

                    return 1;
                }

                return -1;
            };
        }

    }
}
