using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace MarkSpecs
{
    /// <summary>
    /// Manage the PlantUml processes.
    /// It is possible to launch several PlantUml processes. 
    /// </summary>
    public class PlantUmlProcessManager : IDisposable
    {
        //Record the list of the process to kill them on request.
        private List<Process> Processes = new List<Process>();

        private string javaPath;
        private int instanceNb;
        private string plantUmlPath;
        private string plantUmlParameters;
        private int initPort;
        private bool disposedValue;

        //The constructor also launches the process.
        public PlantUmlProcessManager()
        {
            RetrieveConfiguration();

            if (!File.Exists(javaPath))
                throw new ApplicationException("Java.exe not found: " + javaPath);

            if (!File.Exists(plantUmlPath))
                throw new ApplicationException("plantuml.jar not found in " + plantUmlPath);

            //Each PlantUml will have its own thread. 
            //WaitHandles is used to wait about all threads.
            WaitHandle[] waitHandles = new WaitHandle[instanceNb];

            //Build the required number of processes.
            //Then launch the threads.
            ///<see cref="https://stackoverflow.com/a/29753402/12731908"/> for the ConsoleOutputs.
            ///<see cref="https://stackoverflow.com/a/4190972/12731908"/> for the threads.
            for (int i = 0; i < instanceNb; i++)
            {
                var argument = "-jar " + plantUmlPath + " -ftp:" + (initPort + i) + " -tsvg " + plantUmlParameters;
                argument = argument.Trim();

                ProcessStartInfo pInfo = new ProcessStartInfo(javaPath, argument)
                {
                    //CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                };

                var id = i; //a way to have an id of the process, only for console print.
                var handle = new EventWaitHandle(false, EventResetMode.ManualReset);

                var thread = new Thread(() =>
                {
                    Process process = new Process
                    {
                        StartInfo = pInfo
                    };

                    Console.WriteLine("Process " + id + " " + argument);
                    process.Start();
                    handle.Set();
                    
                    //Redirect the outputs to the Console.
                    process.OutputDataReceived += (s, e) => Console.WriteLine("[" + id + "]" + e.Data);
                    process.ErrorDataReceived += (s, e) => Console.WriteLine("[" + id + "]" + e.Data);

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    Processes.Add(process);

                    process.WaitForExit();
                });

                waitHandles[i] = handle;
                thread.Start();
            }

            WaitHandle.WaitAll(waitHandles);
        }

        /// <summary>
        /// Retrieve the configuration from the App.config file.
        /// </summary>
        private void RetrieveConfiguration()
        {
            //Get the Java path
            javaPath = ConfigurationManager.AppSettings["JavaPath"];
            if (string.IsNullOrEmpty(javaPath.Trim()))
                javaPath = RetrieveJavaPathFromEnvironmentVariable();

            instanceNb = int.Parse(ConfigurationManager.AppSettings["PlantUml.InstancesCount"]);
            initPort = int.Parse(ConfigurationManager.AppSettings["PlantUml.Port"]);
            plantUmlPath = ConfigurationManager.AppSettings["PlantUml.Path"];
            plantUmlParameters = ConfigurationManager.AppSettings["PlantUml.Parameter"];
        }

        /// <summary>
        /// Retrieve Java from the environment variables.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Java environment not identified.</exception>
        private static string RetrieveJavaPathFromEnvironmentVariable()
        {
            //Search in user variables first
            var paths = Environment.GetEnvironmentVariable("path", EnvironmentVariableTarget.User).Split(";".ToCharArray());
#nullable enable
            var javaPath = SearchJavaInPaths(paths);

            if (javaPath is null)
            {//else search on the machine
                paths = Environment.GetEnvironmentVariable("path", EnvironmentVariableTarget.Machine).Split(";".ToCharArray());
                javaPath = SearchJavaInPaths(paths);
            }

            return javaPath ?? throw new ArgumentNullException("Unable to identify Java environment");
        }

        /// <summary>
        /// Search Java path in a string.
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        private static string? SearchJavaInPaths(string[] paths)
        {
            string? applicableJavaPath = null;
            Version? applicableJavaVersion = null;
            foreach (var pth in paths)
            {
                if (pth.Contains(@"\Java\"))
                {
                    if (File.Exists(Path.Combine(pth, "java.exe")))
                    {
                        var versionInfo = FileVersionInfo.GetVersionInfo(Path.Combine(pth, "java.exe"));
                        Version currentVersion = new Version(versionInfo.ProductVersion.Replace(",", "."));
                        if (applicableJavaVersion is null)
                        {
                            applicableJavaVersion = currentVersion;
                            applicableJavaPath = Path.Combine(pth, "java.exe");
                        }
                        else if (currentVersion > applicableJavaVersion)
                        {
                            applicableJavaVersion = currentVersion;
                            applicableJavaPath = Path.Combine(pth, "java.exe");
                        }
                    }
                }
            }

            return applicableJavaPath;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (var process in this.Processes)
                    {
                        process.Kill();
                    }
                }
                disposedValue = true;
            }
        }

        // // TODO: substituer le finaliseur uniquement si 'Dispose(bool disposing)' a du code pour libérer les ressources non managées
        // ~PlantUmlProcessManager()
        // {
        //     // Ne changez pas ce code. Placez le code de nettoyage dans la méthode 'Dispose(bool disposing)'
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Ne changez pas ce code. Placez le code de nettoyage dans la méthode 'Dispose(bool disposing)'
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}