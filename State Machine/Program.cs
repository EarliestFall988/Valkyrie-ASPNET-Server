
using Avalon;

//boot the state machine

string baseDomain = Environment.CurrentDirectory;

string json = System.IO.File.ReadAllText($@"{baseDomain}\stateMachine.json"); //get the json file

StateMachineBuilder builder = new StateMachineBuilder(); // create a state machine builder
var stateMachine = builder.ParseInstructionsJSON(json); // parse the json file and build the state machine


stateMachine.Boot(); // boot the state machine