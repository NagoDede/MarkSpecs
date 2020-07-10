using System;
using System.IO;
using System.Diagnostics;

namespace Markdig.Extensions.Mocodo
{
    /// <summary>
    /// Defines the python environment for Mocodo.
    /// </summary>
    public class MocodoEnvironment
    {
        public string PythonPath { get; set; }
        public string MocodoPath { get; set; }


        public MocodoEnvironment()
        {
            try
            {
                this.PythonPath = RetrievePythonPathFromEnvironmentVariable();
                this.MocodoPath = RetriveMocodopath();
            }
            catch (NullReferenceException)
            {
                throw;
            }
        }

        /// <summary>
        /// Python environment will be retrieved from the environment variable.
        /// </summary>
        /// <param name="mocodoPath"></param>
        public MocodoEnvironment(string mocodoPath)
        { 
            try
            {
                this.PythonPath = RetrievePythonPathFromEnvironmentVariable();
            }
            catch (NullReferenceException)
            {
                throw;
            }

            this.MocodoPath = mocodoPath;
        }


        public MocodoEnvironment(string pythonpath, string mocodoPath)
        {
            this.MocodoPath = mocodoPath;
            if (File.Exists(pythonpath))
                this.PythonPath = pythonpath;
            else
                throw new ArgumentNullException($"Unable to identify Python executable in {pythonpath}");
        }

        /// <summary>
        /// Retrieve the last Python version from the environment variables.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">Python environment not identified.</exception>
        private string RetrievePythonPathFromEnvironmentVariable()
        {
            //Search in user variables first
            var paths = Environment.GetEnvironmentVariable("path", EnvironmentVariableTarget.User).Split(";".ToCharArray());
#nullable enable
            var pythonPath = SearchInPaths(paths);

            if (pythonPath is null)
            {//else search on the machine
                paths = Environment.GetEnvironmentVariable("path", EnvironmentVariableTarget.Machine).Split(";".ToCharArray());
                pythonPath = SearchInPaths(paths);
            }

            return pythonPath ?? throw new ArgumentNullException("Unable to identify Python environment");
        }

        private string? SearchInPaths(string[] paths)
        {
            string? applicablePythonPath = null;
            Version? applicablePythonVersion = null;
            foreach (var pth in paths)
            {
                if (pth.Contains(@"\Python\") && !pth.Contains(@"\Scripts"))
                {
                    if (File.Exists(Path.Combine(pth, "python.exe")))
                    {
                        var versionInfo = FileVersionInfo.GetVersionInfo(Path.Combine(pth, "python.exe"));
                        Version currentVersion = new Version(versionInfo.ProductVersion.Replace(",", "."));
                        if (applicablePythonVersion is null)
                        {
                            applicablePythonVersion = currentVersion;
                            applicablePythonPath = Path.Combine(pth, "python.exe");
                        }
                        else if (currentVersion > applicablePythonVersion)
                        {
                            applicablePythonVersion = currentVersion;
                            applicablePythonPath = Path.Combine(pth, "python.exe");
                        }
                    }
                }
            }

            return applicablePythonPath;
        }



        private string RetriveMocodopath()
        {
            string mocodoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mocodo","mocodo.py");
            if (File.Exists(mocodoPath))
                return mocodoPath;
            else
                throw new NullReferenceException("Unable to find mocodo path");
        }

    }
}
