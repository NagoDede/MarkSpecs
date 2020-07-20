// Copyright (c) Vincent DETROYAT. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System;
using System.IO;

namespace Markdig.Extensions.Packetdiag
{
    /// <summary>
    /// Defines the environment for PacketDiag
    /// </summary>
    public class PacketdiagEnvironment : ExtensionEnvironment
    {
        public override string ExtensionName { get => "packetdiag"; }

        public string NwdiagPath
        {
            get
            {
                return this["PacketdiagPath"];
            }

            private set
            {
                this["PacketdiagPath"] = value;
            }
        }

        public PacketdiagEnvironment()
        {
            try
            {
                this.NwdiagPath = RetrievePacketdiagPath();
            }
            catch (NullReferenceException)
            {
                throw;
            }
        }

        public PacketdiagEnvironment(string nwdiagPath)
        {
            this.NwdiagPath = nwdiagPath;
        }

        /// <summary>
        /// Retrieve PackDiag.exe from the context.
        /// </summary>
        /// <returns></returns>
        private static string RetrievePacketdiagPath()
        {
            string mocodoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"vendors\nwdiag\bin", "packetdiag.exe");
            if (File.Exists(mocodoPath))
                return mocodoPath;
            else
                throw new NullReferenceException("Unable to find packetdiag path");
        }
    }
}