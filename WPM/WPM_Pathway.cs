using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.FileIO;
using System.Data;
using System.IO;
//using System.Data.SqlClient;

//using System.Collections;
namespace WPM
{
    class WPM_Pathway
    {
        private String wpmCPGPath;
        private String wpmCPGOpenPathway;
        private String UBWdataImport;
        private String wpmRCPPath = "";
        private String wpmRCPPlansPath = "";
        private String wpmRCPFailuresPath = "";
        public WPM_Pathway(String wpmPath, String wpmOP, String dataImport, String RCP, String RCPPlans, String RCPFailures)
        {
            wpmCPGPath = wpmPath;
            wpmCPGOpenPathway = wpmOP;
            UBWdataImport = dataImport;
            wpmRCPPath = RCP;
            wpmRCPPlansPath = RCPPlans;
            wpmRCPFailuresPath = RCPFailures;

        }

        public void ProcessCPGupload()
        {
            // upload files and then delete source files ==============================================================================
            string[] filePaths = Directory.GetFiles(wpmCPGOpenPathway, "students.txt");// get files to upload
            ftp ftpClient = new ftp(@" ftp://uk-ftp-1.wpmeducation.com", "WAR0127-01_cpg_edu_wpmhost_net_upload", "cArAzoJJJG9reW22");

            foreach (string filePath in filePaths) // for aech file found upload to WPM and then delete the source
            {
                ftpClient.upload("" + "students.txt", filePath);
                //File.Delete("@" + wpmCPGPath + filePath);
            }


            ftpClient = null;
        }
        // ===============================================================================================

        public void ProcessCPGdownload()
        {
            // Get FTP files from the CPG download area
            //ftp ftpClient = new ftp(@"ftp://ftp-dx.wpmeducation.com", "WAR0127-01_cpg_edu_wpmhost_net_download", "NckEhnK!ifyyxeV5");
            //String[] simpleDirectoryListing = ftpClient.directoryListSimple("data_export");// get downloads from the data_export directory
            wpm_sftp ftpClient = new wpm_sftp(@"ftp-dx.wpmeducation.com", "WAR0127-01_cpg_edu_wpmhost_net_download", "NckEhnK!ifyyxeV5");
            string[] simpleDirectoryListing = ftpClient.simpledirectorylist("data_export");// get downloads from the data_export directory


            String iDate = "2021-07-01";
            DateTime mDate;
            String UBW_Filename = "";
            for (int i = 0; i < simpleDirectoryListing.Count(); i++)
            {
                //if (ftpClient.MyFTPFileHasData("data_export/" + simpleDirectoryListing[i])) utility.InsertFTPFile(simpleDirectoryListing[i]);//If FTP file has data then add the filename to the table for processing.
                String wpmFname = simpleDirectoryListing[i].ToString();

                if (!File.Exists(wpmCPGPath + simpleDirectoryListing[i]) && simpleDirectoryListing[i].Length > 0)// if the file does not exist locally and the filename is greater than zero
                {
                    // Write a record to toyota.utility.WPM_FTP_Files if the file has data
                    // then download file to the wpm area
                    ftpClient.download("data_export/" + simpleDirectoryListing[i], wpmCPGPath + simpleDirectoryListing[i]);
                    //only download GL07 files to the data import folder for UBW import'
                    FileInfo fi = new FileInfo(wpmCPGPath + wpmFname);
                    if (wpmFname.ToLower().Contains("-pg.txt"))// change this to the actual GL07 file name used
                    {
                       
                        // rename the Data Import file here - needs to match the batch_id - in theory they should match
                        //UBW_Filename = wpmFname.ToString();
                        //UBW_Filename = UBW_Filename.Replace("GL07_PG_Payments_", "");
                        //UBW_Filename = UBW_Filename.Replace(".txt", "");
                        //Change filename to match batchID which is the previous days date
                        iDate = wpmFname.Substring(0, 4).ToString() + "-" + wpmFname.Substring(4, 2).ToString() + "-" + wpmFname.Substring(6, 2).ToString();
                        mDate = Convert.ToDateTime(iDate).AddDays(-1);
                        UBW_Filename = mDate.ToString("yyyyMMdd") + "-PG";

                        if (fi.Length > 100) ftpClient.download("data_export/" + simpleDirectoryListing[i], UBWdataImport + UBW_Filename.ToString());

                        //ftpClient.download("data_export/" + simpleDirectoryListing[i], UBWdataImport + simpleDirectoryListing[i]);
                    }
                    if (false) Console.WriteLine("Downloaded file: " + simpleDirectoryListing[i]);
                }
                else if (simpleDirectoryListing[i].Length > 0) if (false) Console.WriteLine("This file exists locally, no need to download: " + simpleDirectoryListing[i]);
            }
            ftpClient = null;
        }
        // ==================================================================================================================
        public void ProcessRCPDownload()
        {
            // Get FTP files from the RCP download area
            //ftp ftpClient = new ftp(@" ftp://uk-ftp-1.wpmeducation.com", "WAR0127-01_rcp_edu_wpmhost_net_download", "NckEhnK!ifyyxeV5");
            String iDate = "2021-07-01";
            DateTime mDate;
            String fName = "";
           
           // ftp ftpClient = new ftp(@" ftp://uk-ftp-1.wpmeducation.com", "WAR0127-01_rcp_download", "3gfw6SRklU9OGg%8");
            //String[] simpleDirectoryListing = ftpClient.directoryListSimple("data_export");// get downloads from the data_export directory

            wpm_sftp ftpClient = new wpm_sftp(@"ftp-dx.wpmeducation.com", "WAR0127-01_rcp_download", "3gfw6SRklU9OGg%8");
            string[] simpleDirectoryListing = ftpClient.simpledirectorylist("data_export");// get downloads from the data_export directory

            for (int i = 0; i < simpleDirectoryListing.Count(); i++)
            {
                //if (ftpClient.MyFTPFileHasData("data_export/" + simpleDirectoryListing[i])) utility.InsertFTPFile(simpleDirectoryListing[i]);//If FTP file has data then add the filename to the table for processing.
                String wpmFname = simpleDirectoryListing[i].ToString();

                if (!File.Exists(wpmCPGPath + simpleDirectoryListing[i]) && simpleDirectoryListing[i].Length > 0)// if the file does not exist locally and the filename is greater than zero
                {
                    // Write a record to toyota.utility.WPM_FTP_Files if the file has data

                    // then download file to the wpm area
                    ftpClient.download("data_export/" + simpleDirectoryListing[i], wpmCPGPath + simpleDirectoryListing[i]);

                    // get file details so we can ensure there is data in the file before downloading it to Data Import foolder
                    FileInfo fi = new FileInfo(wpmCPGPath + wpmFname);

                    //only download GL07 files to the data import folder for UBW import'
                    if (wpmFname.Contains("RCP."))// change this to the actual RCP file(s) name used
                    {
                        //Change filename to match batchID which is the previous days date
                        iDate = wpmFname.Substring(0, 4).ToString() + "-" + wpmFname.Substring(4, 2).ToString() + "-" + wpmFname.Substring(6, 2).ToString();
                        mDate = Convert.ToDateTime(iDate).AddDays(-1);
                        fName = mDate.ToString("yyyyMMdd") + "-RCP";

                        if (fi.Length > 100) ftpClient.download("data_export/" + simpleDirectoryListing[i], UBWdataImport + fName.ToString());
                    }
                    // RCP GL07 files

                    if (wpmFname.Contains("RCP."))// change this to the actual RCP file(s) name used
                    {
                        ftpClient.download("data_export/" + simpleDirectoryListing[i], wpmRCPPath + simpleDirectoryListing[i]);
                    }

                    //Download Plans
                    if (wpmFname.Contains("Plans"))// change this to the actual RCP file(s) name used
                    {
                        ftpClient.download("data_export/" + simpleDirectoryListing[i], wpmRCPPlansPath + simpleDirectoryListing[i]);
                    }

                    //Download Failures

                    if (wpmFname.Contains("Failures"))// change this to the actual RCP file(s) name used
                    {
                        ftpClient.download("data_export/" + simpleDirectoryListing[i], wpmRCPFailuresPath + simpleDirectoryListing[i]);
                    }




                    if (false) Console.WriteLine("Downloaded file: " + simpleDirectoryListing[i]);
                }
                else if (simpleDirectoryListing[i].Length > 0) if (false) Console.WriteLine("This file exists locally, no need to download: " + simpleDirectoryListing[i]);
            }
            ftpClient = null;
        }
                // ==================================================================================================================



    }
}
