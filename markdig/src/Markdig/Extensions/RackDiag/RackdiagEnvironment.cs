// Copyright (c) Vincent DETROYAT. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System;
using System.Diagnostics;
using System.IO;

namespace Markdig.Extensions.Rackdiag
{
    /// <summary>
    /// Defines the python environment for Mocodo.
    /// </summary>
    public class RackdiagEnvironment : ExtensionEnvironment
    {
        public override string ExtensionName { get => "rackdiag"; }

        public string RackdiagPath
        {
            get
            {
                return this["RackdiagPath"];
            }

            private set
            {
                this["RackdiagPath"] = value;
            }
        }

        public RackdiagEnvironment()
        {
            try
            {
                this.RackdiagPath = RetrievePacketdiagPath();
            }
            catch (NullReferenceException)
            {
                throw;
            }
        }

        public RackdiagEnvironment(string path)
        {
            this.RackdiagPath = path;
        }


        /// <summary>
        /// Retrieve Moco in the assembly.
        /// </summary>
        /// <returns></returns>
        private static string RetrievePacketdiagPath()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"vendors\nwdiag\bin", "rackdiag.exe");
            if (File.Exists(path))
                return path;
            else
                throw new NullReferenceException("Unable to find Rackdiag path");
        }
    }
}