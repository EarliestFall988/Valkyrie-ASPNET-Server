
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text.Json;

namespace ValkyrieFSMCore.WM
{
    /// <summary>
    /// Get the list of projects function
    /// </summary>
    public class GetProjects : FunctionDefinition
    {


        private static readonly string url = "http://localhost:3001/api/rest/v1/projects";
        private static readonly string devBranch = "https://warmanager-git-rest-api-earliestfall988.vercel.app/api/rest/v1/projects";

        public GetProjects()
        {
            Setup();
            DefineFunction();
        }

        void Setup()
        {
            Name = "GetProjects";
            ExpectedParameters = new Dictionary<string, Parameter>()
            {
                { "result", new Parameter("projects", VariableIO.Out) }
            };
        }


        protected override void DefineFunction()
        {
            Function = () =>
            {

                try
                {

                    using HttpClient client = new();

                    var body = new GetProjectsBody("fa3d5a36-f894-4af4-ae76-1b4ff70d9a19");

                    var reqJSON = JsonSerializer.Serialize(body);

                    Debug.WriteLine(reqJSON.ToString());

                    HttpContent content = new StringContent(reqJSON);

                    //client.DefaultRequestHeaders.Add("Accept", "application/json");
                    //client.DefaultRequestHeaders.Add("Content-Type", "application/json");
                    //client.DefaultRequestHeaders.Add("api_key", "fa3d5a36-f894-4af4-ae76-1b4ff70d9a19");



                    Debug.WriteLine(devBranch);

                    var response = client.PostAsync(devBranch, content).Result;


                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        Debug.WriteLine("err: " + response.StatusCode);
                        return -1;
                    }

                    using StreamReader reader = new StreamReader(response.Content.ReadAsStream());

                    var json = reader.ReadToEndAsync().Result;

                    Debug.WriteLine(json);

                    var projects = JsonSerializer.Deserialize<ProjectsListResult>(json);

                    if (projects == null)
                        projects = new ProjectsListResult();

                    Set("result", projects.projects);
                    return 1;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("error: " + ex.Message);
                    return -1;
                }
            };
        }
    }

    public class ProjectsListResult
    {
        public List<Project> projects { get; set; } = new List<Project>();
    }

    internal record class GetProjectsBody(string api_key);
}
