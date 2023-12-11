
namespace ValkyrieFSMCore.WM
{
    public class SplitProject : FunctionDefinition
    {

        public SplitProject()
        {
            Setup();
            DefineFunction();
        }

        void Setup()
        {
            Name = "SplitProject";
            ExpectedParameters = new Dictionary<string, Parameter>()
            {
                { "project", new Parameter("project") },
                { "name", new Parameter("string", required: false, io:VariableIO.Out) },
                { "id", new Parameter("string", required: false, io:VariableIO.Out) },
                { "TotalManHours", new Parameter("decimal", required: false, io:VariableIO.Out) },
                { "laborCost", new Parameter("decimal", required: false, io:VariableIO.Out) },
            };
        }

        protected override void DefineFunction()
        {

            Func<int> func = () =>
            {

                var project = Get<Project>("project");

                TrySet("name", project.Name);
                TrySet("id", project.Id);
                TrySet("TotalManHours", project.TotalManHours);
                TrySet("laborCost", project.LaborCost);

                return 1;
            };

            Function = func;
        }
    }
}
