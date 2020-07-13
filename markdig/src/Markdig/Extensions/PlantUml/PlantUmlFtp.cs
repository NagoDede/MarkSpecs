// Copyright (c) Vincent DETROYAT. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System;
using System.IO;
using System.Net;
using System.Text;
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
        private readonly string port = null;
        private readonly string user = null;
        private readonly string pass = null;

        private static int contentsCount;

        public PlantUmlFtp(PlantUmlEnvironment environment)
        {
            host = environment.Host;
            user = environment.FtpUserId;
            pass = environment.FtpUserPwd;
            port = environment.Port.ToString();
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

            var sendFileResult = await FtpWriteAsync(@"ftp://" + host + ":" + port + "/" + fileNameIn, content);

            if (sendFileResult.Item1)
            {
                return await FtpReadAsycn(@"ftp://" + host + ":" + port + "/" + fileNameOut);
            }
            else
            {
                return "Unable to generate SVG file: " + sendFileResult.Item2;
            }
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
            return (true,"");
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
    }
}