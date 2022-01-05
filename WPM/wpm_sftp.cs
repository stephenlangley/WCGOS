using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using Renci.SshNet;
using Renci.SshNet.Common;
using Renci.SshNet.Sftp;


namespace WPM
{
    class wpm_sftp
    {
        private string host = null;
        private string user = null;
        private string pass = null;
        private int Port = 22;

        public wpm_sftp(string hostIP, string userName, string password)
        {
            host = hostIP; user = userName; pass = password;
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        public void download(string remoteFile, string localFile)
        {
            //String Host = "ftp-dx.wpmeducation.com";
            //int Port = 22;
            //String RemoteFileName = "data_export/20211216-PG.txt";
            //String LocalDestinationFilename = "20211216-PG.txt";
            //String Username = "WAR0127-01_cpg_edu_wpmhost_net_download";
            //String Password = "NckEhnK!ifyyxeV5";

            using (var sftp = new SftpClient(host, Port, user, pass))
            {
                sftp.Connect();

                using (var file = File.OpenWrite(localFile))
                {
                    sftp.DownloadFile(remoteFile, file);
                }

                sftp.Disconnect();
            }
        } // end of wpmConnect

        public string[] simpledirectorylist(string remoteFolder)
        {
            try
            {

                using (var sftp = new SftpClient(host, Port, user, pass))
                {
                    String[] dList = new string[] { };
                    sftp.Connect();

                    var files = sftp.ListDirectory(remoteFolder);
                    int i = 0;
                    foreach (var file in files)
                    {
                        if (!file.Name.StartsWith("."))
                        {
                            i++;
                            Array.Resize(ref dList, i);
                            dList[i - 1] = file.Name;
                            string v = file.Length.ToString();
                        }
                    }
                    sftp.Disconnect();
                    return dList;
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            return new string[] { "" };
        }

        public Boolean MyFTPFileHasData(string remoteFile)
        {

            using (var sftp = new SftpClient(host, Port, user, pass))
            {
                Boolean isData = false;
                String[] dList = new string[] { };
                sftp.Connect();
                if (sftp.Get(remoteFile).Length > 500) isData = true;            
                sftp.Disconnect();
                return isData;
            }
        }
    }//class
}//namspace
