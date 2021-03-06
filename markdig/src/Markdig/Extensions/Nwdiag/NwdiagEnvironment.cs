// Copyright (c) Vincent DETROYAT. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System;
using System.IO;

namespace Markdig.Extensions.Nwdiag
{
    /// <summary>
    /// Defines the python environment for Mocodo.
    /// </summary>
    public class NwdiagEnvironment : ExtensionEnvironment
    {
        public override string ExtensionName { get => "nwdiag"; }

        public string NwdiagPath
        {
            get
            {
                return this["NwdiagPath"];
            }

            private set
            {
                this["NwdiagPath"] = value;
            }
        }

        public NwdiagEnvironment()
        {
            try
            {
                this.NwdiagPath = RetrieveNwdiagPath();
            }
            catch (NullReferenceException)
            {
                throw;
            }
        }


        public NwdiagEnvironment(string nwdiagPath)
        {
            this.NwdiagPath = nwdiagPath;
        }

        /// <summary>
        /// Retrieve NwDiag from the environment.
        /// </summary>
        /// <returns></returns>
        private static string RetrieveNwdiagPath()
        {
            string mocodoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"vendors\nwdiag\bin", "nwdiag.exe");
            if (File.Exists(mocodoPath))
                return mocodoPath;
            else
                throw new NullReferenceException("Unable to find Nwdiag path");
        }
    }
}