# AdvCmdParser
AdvCmdParser is an Advanced Command Parser for .Net console projects.  
The parser enables you to create efficient and complex command line software though a single class without any dependency.

## Capabilities
AdvCmdParser provides:
- the full print of the description of the commands and the associated options.
- mandatory and optional parameters.
- complex and combined command line orders.

Thanks AdvCmdParser, it is easy to create beautiful in a simple way.

## Getting Started

Initiate the command parser with a description of the application.
```cs
CommandParser cp = new CommandParser();
string appDefinition =
"This is an exzmple on how to use the Command Parser. \r\n" +
"The parser allows the management of commands with arguments/options, or a simple use of arguments. \r\n" +
"Main idea is to manage a command line quite similar to git command line. Typically, a command line: \r\n" +
"\t soft.exe new -branch \"new branch\" \r\n" +
"is managed by the command parser. \r\n" +
"Also, the command parser generates a help display thanks the information provided during the configuration. \r\n" +
"What you see here, is the content of the ApplicationDescription";

cp.ApplicationDescription = appDefinition;
```
Create the first command (the 'new' command), without parameter nor arguments.
```cs
//First command
string c1Description = "Create a new command in the command parser. A command is the first word just after the application name. A command shall never start by the arguments prefix (usually defined by -), and shall not be a space. \r\n" +
  " A command can have mulitple parameters. Parameters are mandatory inputs, while arguments are optional items. \r\n" +
  "If you use one or several commands, you should avoid the definition of arguments at application level. \r\n" +
  "you should avoid a such configuration: \r\n" +
  "\t app.exe command1 -arg1 -arg2 \r\n" +
  "\t app.exe --help \r\n" +
"The command help may be confused with an argument" +
  "Instead, use commands under this form:" +
  "\t app.exe command1 -arg1 -arg2 \r\n" +
  "\t app.exe help \r\n" +
  "Commands and arguments are associated to action item. Keep only the action of an argument associated to command to define a configuration. Do not execute specific command or launch a process from them. Else, your command will not be executed as expected.";
            
Command c1 = cp.Command("new", c1Description, (c, p, a) => {
  Console.WriteLine("This line is displayed following the action defined for the command new.");
  Console.WriteLine("No parameter nor arguments have been defined for this command.");
});
```

Create a command 'addParam', with on parameter (mandatory item) and one argument:
```cs
string c2Description = "This command shos how to add parameters to a command \r\n" +
  "Here, two parameters are defined and have to be retrieved on the command line to allow the action associated to the command";

Command c2 = cp.Command("addParam", c2Description, 2, (c, p, a) => {
  Console.WriteLine("This line is displayed follwing the action defined for the command new.");
  Console.WriteLine("No parameter nor arguments have been defined for this command.");
  Console.WriteLine("You can retrieve the value of parameter thanks its field Value");
  Console.WriteLine("p[0].Value will retrive the value of the first parameter \t\n");
 });
 
 c2.Parameters[0].Description = "This is the description of the first parameter. The name of the parameter is used for display. " + 
 "The description and the allocation of a name to a parameter is not mandatory. They are automatically defined thanks a constructor";
 c2.Parameters[1].Name = "Parameter2Name";
 c2.Parameters[1].Description = "A parameter name shall not contain a space.";

 c2.Parameters.Parameter("Param3", "Here we add a 3rd parameter by using a different method");
 '''

