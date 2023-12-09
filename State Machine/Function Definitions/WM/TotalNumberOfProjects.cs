namespace ValkyrieFSMCore.WM
{
    /// <summary>
    /// Get the total number of projects
    /// </summary>
    public class TotalNumberOfProjects : FunctionDefinition
    {

        public TotalNumberOfProjects()
        {
            Name = nameof(TotalNumberOfProjects);
            Setup();
            DefineFunction();
        }

        void Setup()
        {
            ExpectedParameters = new Dictionary<string, Parameter>()
            {
                { "projects", new Parameter("projects") },
                { "total", new Parameter("integer", VariableIO.Out) }
            };
        }

        protected override void DefineFunction()
        {
            Function = () =>
            {
                var projects = Get<List<Project>>("projects");
                Set("total", projects.Count);

                return 1;
            };
        }
    }
}
