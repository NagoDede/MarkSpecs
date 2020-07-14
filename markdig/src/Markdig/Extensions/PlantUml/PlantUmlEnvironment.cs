// Copyright (c) Vincent DETROYAT. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System;

namespace Markdig.Extensions.PlantUml
{
    /// <summary>
    /// Defines the environment to use PlantUml over FTP.
    /// </summary>
    public class PlantUmlEnvironment : ExtensionEnvironment
    {
        public override string ExtensionName { get => "plantuml"; }

        public string Host
        {
            get
            {
                return this["Host"];
            }

            private set
            {
                this["Host"] = value;
            }
        }

        public Int32 Port
        {
            get
            {
                return this["Port"];
            }

            private set
            {
                this["Port"] = value;
            }
        }

        //public string JavaPath { get; private set; }

        public string FtpUserId
        {
            get
            {
                return this["FtpUserId"];
            }

            private set
            {
                this["FtpUserId"] = value;
            }
        }

        public string FtpUserPwd
        {
            get
            {
                return this["FtpUserPwd"];
            }

            private set
            {
                this["FtpUserPwd"] = value;
            }
        }

        public int InstanceNb
        {
            get
            {
                return this["InstanceNb"];
            }

            private set
            {
                this["InstanceNb"] = value;
            }
        }

        /// <summary>
        /// Initiate a PlantUml environment to a local host on default port.
        /// Hot: 127.0.0.1 and port 4242.
        /// Also, user has to take care about Firewall parameters.
        /// </summary>
        public PlantUmlEnvironment()
        {
            this.Host = "127.0.0.1";
            this.Port = 4242;
            this.FtpUserId = "";
            this.FtpUserPwd = "";
            this.InstanceNb = 1;
        }

        /// <summary>
        /// Initiate a PlantUml environment.
        /// </summary>
        /// <param name="plantUmlHost"></param>
        /// <param name="plantUmlStartPort"></param>
        public PlantUmlEnvironment(string plantUmlHost,
    Int32 plantUmlStartPort, int instance = 1, string ftpUser = "", string ftpPass = "")
        //string javaPath)
        {
            this.Host = plantUmlHost;
            this.Port = plantUmlStartPort;
            this.FtpUserId = ftpUser;
            this.FtpUserPwd = ftpPass;
            this.InstanceNb = instance;
        }
    }
}