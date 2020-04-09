using Microsoft.VisualStudio.TestTools.UnitTesting;
using NagoyDede.AdvCmdParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NagoyDede.AdvCmdParser.Tests
{
    [TestClass()]
    public class CommandParserTests
    {
        [TestMethod()]
        public void SimpleCommand()
        {
            CommandParser cp = new CommandParser();
            bool commandDone = false;
            Command c1 = cp.Command("new", "create new", (e, l, a) => {
                Assert.IsNull(l);
                Assert.IsNull(a);
                Console.WriteLine("New");
                commandDone = true;
            });

            string[] testValues = { "Command", "new" };
            cp.Parse(testValues);
            Assert.IsTrue(commandDone);

        }

        [TestMethod()]
        public void SimpleCommandWithOneArgument()
        {
            CommandParser cp = new CommandParser();
            bool commandDone = false;
            bool argDone = false;
            Command c1 = cp.Command("new", "create new", (e, l, a) =>
            {
                Console.WriteLine("New");
                commandDone = true;
            });
            c1.Arguments.Argument("f", "force", "forece the update", CommandArgumentFlags.TakesParameter,
                (c, s) =>
                {
                    Console.WriteLine("Arguments " + s);
                    argDone = true;
                });
            string[] testValues = { "Command", "new", "--force", "test" };
            cp.Parse(testValues);

            Assert.IsTrue(commandDone);
            Assert.IsTrue(argDone);
        }

        [TestMethod()]
        public void SimpleCommandWithTwoArgument()
        {
            CommandParser cp = new CommandParser();
            bool commandDone = false;
            bool argDone = false;
            bool arg2Done = false;
            Command c1 = cp.Command("new", "create new", (e, l, a) =>
            {
                Console.WriteLine("New");
                commandDone = true;
            });
            c1.Arguments.Argument("f", "force", "forece the update", CommandArgumentFlags.TakesParameter,
                (c, s) =>
                {
                    Console.WriteLine("Arguments " + s);
                    argDone = true;
                });
            c1.Arguments.Argument("d", "del" + "delete", "delete",
               (c, s) =>
               {
                   Console.WriteLine("Delete " + s);
                   arg2Done = true;
               });

            string[] testValues = { "Command", "new", "--force", "test", "-d" };
            cp.Parse(testValues);

            Console.WriteLine(cp.ToString());

            Assert.IsTrue(commandDone);
            Assert.IsTrue(argDone);
            Assert.IsTrue(arg2Done);
        }

        [TestMethod()]
        public void SCommand_2P_2A()
        {
            CommandParser cp = new CommandParser();
            bool commandDone = false;
            bool argDone = false;
            bool arg2Done = false;
            Command c1 = cp.Command("new", "create new", 2, (e, l, a) =>
            {
                Assert.AreEqual(2, l.Count);
                Assert.AreEqual("parameter1", l[0].Value);
                Assert.AreEqual(2, a.Count);
                Console.WriteLine("New " + l[0] + " " + l[1]);
                Console.WriteLine(l.ToString());
                commandDone = true;
            });

            c1.Arguments.Argument("f", "force", "forece the update", CommandArgumentFlags.TakesParameter,
                (c, s) =>
                {
                    Console.WriteLine("Arguments " + s);
                    argDone = true;
                });
            c1.Arguments.Argument("d", "del" + "delete", "delete",
               (c, s) =>
               {
                   Console.WriteLine("Delete " + s);
                   arg2Done = true;
               });

            string[] testValues = { "Command", "new", "parameter1", "parameter2", "--force", "test", "-d" };
            cp.Parse(testValues);

            Console.WriteLine(cp.ToString());
            Assert.IsTrue(commandDone, "Command done");
            Assert.IsTrue(argDone, "Arg1 done");
            Assert.IsTrue(arg2Done, "Arg2 done");
        }

        [TestMethod()]
        public void SimpleCommandWithOneParameter()
        {
            CommandParser cp = new CommandParser();
            bool commandDone = false;
            Command c1 = cp.Command("new", "create new", 1, (e, l, a) =>
            {
                Assert.AreEqual("parameter", l[0].Value);
                Assert.IsNull(a);
                Console.WriteLine("New " + l[0].Value);
                commandDone = true;
            });

            string[] testValues = { "Command", "new", "parameter" };
            cp.Parse(testValues);

            Assert.IsTrue(commandDone);

        }

        [TestMethod()]
        public void SimpleCommandWithTwoMandatoryParameters()
        {
            CommandParser cp = new CommandParser();
            bool commandDone = false;
            Command c1 = cp.Command("new", "create new", 2, (e, l, a) =>
            {
                Assert.AreEqual(2, l.Count);
                Assert.AreEqual("parameter1", l[0].Value);
                Assert.IsNull(a);
                Console.WriteLine("New " + l[0] + " " + l[1]);
                commandDone = true;
            });

            string[] testValues = { "Command", "new", "parameter1", "parameter2" };
            cp.Parse(testValues);

            Assert.IsTrue(commandDone);

        }

        [TestMethod()]
        public void SimpleCommandWithTwoMandatoryParameters_3Provided()
        {
            CommandParser cp = new CommandParser();
            bool commandDone = false;
            Command c1 = cp.Command("new", "create new", 2, (e, l, a) =>
            {
                Assert.AreEqual(2, l.Count);
                Assert.AreEqual("parameter1", l[0].Value);
                Assert.IsNull(a);
                Console.WriteLine("New " + l[0] + " " + l[1]);
                commandDone = true;
            });

            string[] testValues = { "Command", "new", "parameter1", "parameter2", "parameter3" };
            cp.Parse(testValues);

            Assert.IsTrue(commandDone);
        }

        [TestMethod()]
        public void FullExemple()
        {
            CommandParser cp = new CommandParser();
            string appDefinition =
                "This is an exemple on how to use the Command Parser. \r\n" +
                "The parser allows the management of commands with arguments/options, or a simple use of arguments. \r\n" +
                "Main idea is to manage a command line quite similar to git command line. Typically, a command line: \r\n" +
                "\t soft.exe new -branch \"new branch\" \r\n" +
                "is managed by the command parser. \r\n" +
                "Also, the command parser generates a help display thanks the information provided during the configuration. \r\n" +
                "What you see here, is the content of the ApplicationDescription";
            cp.ApplicationDescription = appDefinition;

            //First command
            string c1Description = "Create a new command in the command parser. A command is the first word just after the application name. A command shall never start by the arguments prefix (usually defined by -), and shall not be a space. \r\n" +
                            " A command can have mulitple parameters. Parameters are mandatory inputs, while arguments are optional items. \r\n" +
                            "If you use one or several commands, you should avoid the definition of arguments at application level. \r\n" +
                            "you should avoid a such configuration: \r\n" +
                            "\t app.exe command1 -arg1 -arg2 \r\n" +
                            "\t app.exe --help \r\n" +
                            "Instead, use commands under this form:" +
                             "\t app.exe command1 -arg1 -arg2 \r\n" +
                            "\t app.exe help \r\n" +
                            "Commands and arguments are associated to action item. Keep only the action of an argument associated to command to define a configuration. Do not execute specific command or launch a process from them. Else, your command will not be executed as expected.";
            Command c1 = cp.Command("new", c1Description, (c, p, a) => {
                Console.WriteLine("This line is displayed follwing the action defined for the command new.");
                Console.WriteLine("No parameter nor arguments have been defined for this command.");
            });

            string c2Description = "This command shos how to add parameters to a command \r\n" +
                "Here, two parameters are defined and have to be retrieved on the command line to allow the action associated to the command";
            Command c2 = cp.Command("addParam", c2Description, 2, (c, p, a) => {
                Console.WriteLine("This line is displayed follwing the action defined for the command new.");
                Console.WriteLine("No parameter nor arguments have been defined for this command.");
                Console.WriteLine("You can retrieve the value of parameter thanks its field Value");
                Console.WriteLine("p[0].Value will retrive the value of the first parameter \t\n");
            });
            c2.Parameters[0].Description = "This is the description of the first parameter. The name of the parameter is used for display. The description and the allocation of a name to a parameter is not mandatory. They are automatically defined thanks a constructor";
            c2.Parameters[1].Name = "Parameter2Name";
            c2.Parameters[1].Description = "A parameter name shall not contain a space.";

            c2.Parameters.Parameter("Param3", "Here we add a 3rd parameter by using a different method");
            Console.WriteLine(cp.ToString());

            Assert.IsFalse(cp is null);
        }

    }
}