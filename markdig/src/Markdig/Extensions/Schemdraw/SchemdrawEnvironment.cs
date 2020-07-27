// Copyright (c) Vincent DETROYAT. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Markdig.Extensions.Schemdraw
{
    /// <summary>
    /// Defines the python environment for SchemDraw.
    /// </summary>
    public class SchemdrawEnvironment : ExtensionEnvironment
    {
        public override string ExtensionName { get => "schemdraw"; }

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

        public string SchemdrawPath
        {
            get
            {
                return this["SchemdrawPath"];
            }

            private set
            {
                this["SchemdrawPath"] = value;
            }
        }

        public string HeaderPath
        {
            get
            {
                return this["HeaderPath"];
            }

            private set
            {
                this["HeaderPath"] = value;
            }
        }

        /// <summary>
        /// The default header of the Python temporary script.
        /// </summary>
        public string DefaultPythonHeader
        {
            get
            { StringBuilder sb = new StringBuilder();
                sb.AppendLine("import sys");
                sb.AppendLine("import zipimport");
                sb.AppendLine("importer = zipimport.zipimporter(\"${schemdraw_path_zip}\")");
                sb.AppendLine("mod = importer.load_module('schemdraw')");

                return sb.ToString();
            }

        }

        /// <summary>
        /// Load a Schemdraw environment.
        /// It defines the Default values of Python path, the path to the application and the Python header content
        /// </summary>
        public SchemdrawEnvironment()
        {
            try
            {
                this.PythonPath = RetrievePythonPathFromEnvironmentVariable();
                this.SchemdrawPath = RetrieveSchemdrawpath();
            }
            catch (NullReferenceException)
            {
                throw;
            }
        }

        /// <summary>
        /// Build the environment.
        /// NOte:Python environment will be retrieved from the environment variable.
        /// </summary>
        /// <param name="path">Path to Schemdraw application.</param>
        public SchemdrawEnvironment(string path, string pythonHeaderPath)
        {
            try
            {
                this.PythonPath = RetrievePythonPathFromEnvironmentVariable();
            }
            catch (NullReferenceException)
            {
                throw;
            }

            this.SchemdrawPath = path;
            this.HeaderPath = pythonHeaderPath;
        }

        /// <summary>
        /// Build the environment.
        /// </summary>
        /// <param name="schemdrawPath">Path to Schemdraw application.</param>
        /// <param name="pythonpath">Path to Python exe.</param>
        public SchemdrawEnvironment(string pythonpath, string schemdrawPath, string pythonHeaderPath)
        {
            this.SchemdrawPath = schemdrawPath;
            this.HeaderPath = pythonHeaderPath;
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
        /// Retrieve in the assembly.
        /// </summary>
        /// <returns></returns>
        private static string RetrieveSchemdrawpath()
        {
            string schemdrawPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"vendors\", "schemdraw.zip");

            if (File.Exists(schemdrawPath))
                return schemdrawPath;
            else
                throw new NullReferenceException("Unable to find Schemdraw path");
        }
    }
}