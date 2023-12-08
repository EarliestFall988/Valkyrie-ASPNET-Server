namespace ValkyrieFSMCore
{
    public class EachProject : FunctionDefinition
    {

        int iterator = 0;

        public EachProject()
        {
            Setup();
            DefineFunction();
        }

        void Setup()
        {
            Name = "EachProject";
            ExpectedParameters = new Dictionary<string, Parameter>()
            {
                { "projects", new Parameter("projects") },
                { "project", new Parameter("project", VariableIO.Out) }
            };
        }

        protected override void DefineFunction()
        {
            Function = () =>
            {
                var proj = Get<List<Project>>("project");

                if (proj is List<Project> projects)
                {
                    if (iterator < projects.Count)
                    {
                        Set("project", projects[iterator]);
                        iterator++;
                        return 0;
                    }
                    else
                    {
                        iterator = 0;
                        return 1;
                    }
                }

                return -1;
            };
        }
    }
}
