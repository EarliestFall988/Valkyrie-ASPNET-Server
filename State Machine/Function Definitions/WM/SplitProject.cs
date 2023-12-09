
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
                { "name", new Parameter("string", VariableIO.Out) },
                { "id", new Parameter("string", VariableIO.Out) },
                { "TotalManHours", new Parameter("decimal", VariableIO.Out) },
                { "laborCost", new Parameter("decimal", VariableIO.Out) },
            };
        }

        protected override void DefineFunction()
        {

            Func<int> func = () =>
            {

                var project = Get<Project>("project");

                Set("name", project.Name);
                Set("id", project.Id);
                Set("TotalManHours", project.TotalManHours);
                Set("laborCost", project.LaborCost);

                return 1;
            };

            Function = func;
        }
    }
}
