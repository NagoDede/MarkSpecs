// Copyright (c) Vincent DETROYAT. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System;
using System.Diagnostics;
using System.IO;

namespace Markdig.Extensions.Mocodo
{
    /// <summary>
    /// Defines the python environment for Mocodo.
    /// </summary>
    public class MocodoEnvironment : ExtensionEnvironment
    {
        public override string ExtensionName { get => "mocodo"; }

        public string PythonPath
        {
            get
            {
                return this["PythonPath"];
            }

            private set
            {
                this["PythonPath"] = value;
            }
        }

        public string MocodoPath
        {
            get
            {
                return this["MocodoPath"];
            }

            private set
            {
                this["MocodoPath"] = value;
            }
        }

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
        private static string RetrievePythonPathFromEnvironmentVariable()
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

        private static string? SearchInPaths(string[] paths)
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

        /// <summary>
        /// Retrieve Moco in the assembly.
        /// </summary>
        /// <returns></returns>
        private static string RetriveMocodopath()
        {
            string mocodoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"vendors\mocodo", "mocodo.py");
            if (File.Exists(mocodoPath))
                return mocodoPath;
            else
                throw new NullReferenceException("Unable to find mocodo path");
        }
    }
}