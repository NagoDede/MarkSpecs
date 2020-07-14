// Copyright (c) Vincent DETROYAT. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Connect to a PlantUlm FTp server.
/// Send the PlantUml description to the server and return the SVG content.
/// </summary>
namespace Markdig.Extensions.PlantUml
{
    internal class PlantUmlFtp
    {
        private readonly string host = null;
        private readonly int port = 0;
        private readonly string user = null;
        private readonly string pass = null;
        private readonly int instanceNb = 0;

        private static int contentsCount;

        private readonly static Queue<int> _connectionPool = new Queue<int>();
        private static bool poolInit = true;

        private readonly static ManualResetEvent _availableEvent = new ManualResetEvent(false);

        public PlantUmlFtp(PlantUmlEnvironment environment)
        {
            host = environment.Host;
            user = environment.FtpUserId;
            pass = environment.FtpUserPwd;
            port = environment.Port;
            instanceNb = environment.InstanceNb;

            InitPool(environment.Port, instanceNb);
        }

        private static void InitPool(int portInit, int instanceNb)
        {
            //init the port allocation only one time
            if (poolInit)
            {
                poolInit = false;
                lock (_connectionPool)
                {
                    for (int i = 0; i < instanceNb; i++)
                    {
                        _connectionPool.Enqueue(portInit + i);
                    }
                }
            }
        }

        private static int GetPort(TimeSpan timeout)
        {
            if (_connectionPool.Count == 0)
            {
                _availableEvent.Reset();
                return CountIsZero(timeout);
            }
            else
            {
                lock (_connectionPool)
                {
                    if (_connectionPool.Count == 0)
                        return CountIsZero(timeout);
                    else
                        return _connectionPool.Dequeue();
                }
            }
        }

        private static int CountIsZero(TimeSpan timeout)
        {

            if (_availableEvent.WaitOne(timeout))
            {
                return _connectionPool.Dequeue();
            }
            else
            {
                return 0;
            }
        }

        public static void Put(int port)
        {
            lock (_connectionPool)
                _connectionPool.Enqueue(port);

            _availableEvent.Set();
        }

        /// <summary>
        /// Get the PlantUml SVG diagram from the content.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<string> GetSvg(string content)
        {
            var fileNameIn = "Markedig_" + contentsCount + ".txt";
            var fileNameOut = "Markedig_" + contentsCount + ".svg";
            contentsCount++;

            var watch = System.Diagnostics.Stopwatch.StartNew();

            int specificPort;
            if (this.instanceNb == 1)
                specificPort = this.port;
            else
                 specificPort   = GetPort(TimeSpan.FromSeconds(15));

            Console.WriteLine($"Generate PlantUml SVG on {specificPort}");

            if (specificPort == 0)
                return "Unable to generate SVG file. No server available.";

            string svgContent;

            var sendFileResult = FtpWrite(@"ftp://" + host + ":" + specificPort + "/" + fileNameIn, content);

            if (sendFileResult.Item1)
            {
                svgContent = FtpRead(@"ftp://" + host + ":" + specificPort + "/" + fileNameOut);
                Put(specificPort);

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine($"SVG Generated on {specificPort}: {elapsedMs} ms");
                //return svgContent;
            }
            else
            {
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine($"Unable to generate SVG on {specificPort}: {elapsedMs} ms");
                svgContent = $"Unable to generate SVG on {specificPort}: " + sendFileResult.Item2;
            }
            return svgContent;
        }

        private async Task<(bool, string)> FtpWriteAsync(string distantFile, string content)
        {
            /* Create an FTP Request */
            FtpWebRequest ftpRequest = (FtpWebRequest)FtpWebRequest.Create(distantFile);
            /* Log in to the FTP Server with the User Name and Password Provided */
            ftpRequest.Credentials = new NetworkCredential(user, pass);
            /* When in doubt, use these options */
            ftpRequest.UseBinary = false;
            ftpRequest.UsePassive = false;
            ftpRequest.KeepAlive = true;

            ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;

            try
            {
                var ftpStream = ftpRequest.GetRequestStream();
                byte[] data = Encoding.UTF8.GetBytes(content);
                await ftpStream.WriteAsync(data, 0, data.Length);
                ftpStream.Flush();
                ftpStream.Close();
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
            return (true, "");
        }

        private async Task<string> FtpReadAsycn(string distanceFile)
        {
            /* Create an FTP Request */
            FtpWebRequest ftpRequest = (FtpWebRequest)FtpWebRequest.Create(distanceFile);
            /* Log in to the FTP Server with the User Name and Password Provided */
            ftpRequest.Credentials = new NetworkCredential(user, pass);
            /* When in doubt, use these options */
            ftpRequest.UseBinary = false;
            ftpRequest.UsePassive = false;
            ftpRequest.KeepAlive = true;

            ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;

            var ftpResponse = ftpRequest.GetResponse();
            var ftpWebResponse = (FtpWebResponse)ftpResponse;
            using var ftpStreamDwn = ftpWebResponse.GetResponseStream();
            {
                try
                {
                    StreamReader reader = new StreamReader(ftpStreamDwn);
                    return await reader.ReadToEndAsync();
                }
                catch (Exception ex)
                {
                    String s = "Unable to retrieve a valid SVG file";
                    s += ex.Message;
                    return s;
                }
            }
        }

        private (bool, string) FtpWrite(string distantFile, string content)
        {
            /* Create an FTP Request */
            FtpWebRequest ftpRequest = (FtpWebRequest)FtpWebRequest.Create(distantFile);
            /* Log in to the FTP Server with the User Name and Password Provided */
            ftpRequest.Credentials = new NetworkCredential(user, pass);
            /* When in doubt, use these options */
            ftpRequest.UseBinary = false;
            ftpRequest.UsePassive = false;
            ftpRequest.KeepAlive = true;

            ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;

            try
            {
                var ftpStream = ftpRequest.GetRequestStream();
                byte[] data = Encoding.UTF8.GetBytes(content);
                ftpStream.Write(data, 0, data.Length);
                ftpStream.Flush();
                ftpStream.Close();
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
            return (true, "");
        }

        private string FtpRead(string distanceFile)
        {
            /* Create an FTP Request */
            FtpWebRequest ftpRequest = (FtpWebRequest)FtpWebRequest.Create(distanceFile);
            /* Log in to the FTP Server with the User Name and Password Provided */
            ftpRequest.Credentials = new NetworkCredential(user, pass);
            /* When in doubt, use these options */
            ftpRequest.UseBinary = false;
            ftpRequest.UsePassive = false;
            ftpRequest.KeepAlive = true;

            ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;

            var ftpResponse = ftpRequest.GetResponse();
            var ftpWebResponse = (FtpWebResponse)ftpResponse;
            using var ftpStreamDwn = ftpWebResponse.GetResponseStream();
            {
                try
                {
                    StreamReader reader = new StreamReader(ftpStreamDwn);
                    return reader.ReadToEnd();
                }
                catch (Exception ex)
                {
                    String s = "Unable to retrieve a valid SVG file";
                    s += ex.Message;
                    return s;
                }
            }
        }
    }
}