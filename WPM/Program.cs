using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.FileIO;
using System.Data;
using System.IO;
using System.Data.SqlClient;

using System.Collections;


namespace WPM
{
    class Program
    {

         static void Main(string[] args)
        {
            // =====  CHANGE this variable to TRUE when going LIVE to ensure that the WPM is processed =========================================================================
            // =============================================================================================================================
            String[] myFields = null;
            Boolean doManual = false;  // change this to false if creating the auto exe.
            Boolean doWCGOS = true;
            Boolean WpmP = true;
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            //Boolean m = utility.InsertFTPFile("GL07_Store_Payments_20210201.txt");

            //Boolean myX = utility.IsProcessed("GL07_Store_Payments_20210214.txt");
            if (!doManual)
            {
                if (doWCGOS)
                {
                    Console.WriteLine("Start WCGOS");
                    //==========   Process payments for WCG Short Courses (SC) ======================================================
                    WCGOS pp = new WCGOS();
                    Console.WriteLine("Get Batch Id's");
                    // Get a list of the BatchId which have not been processed yet.
                    DataView transData = utility.readDataView("csWCGCloud", "Select batchId from trans where type = 'gl07' and actioned is NULL  Group by batchId"); // Get the data from the cloud
                    DateTime pDate = DateTime.Now; // use the date now to get todays batch
                    String todaysBatch = pDate.ToString("yyyyMMdd") + "-STRIPE-ENROL"; // e.g. 20190312-STRIPE-ENROL
                    String todaysBatchNew = pDate.ToString("yyyyMMdd"); //+ "-STRIPE-ENROL"; // e.g. 20190312-STRIPE-ENROL
                    Console.WriteLine("Process Batch ID's");                                                                   // for each BatchId process the payment based upon the BatchId Select * from trans where BatchId = BatchId and actioned = null
                    foreach (DataRowView batchID in transData)
                    {
                        //pp.processPayment_SC("csWCGCloud", "Select * from trans where DATEDIFF(d,created,getdate()) = 1 and type = 'gl07' and actioned = null");
                        // Process the Short Courses (SC)
                        //Only process transactions prior to today
                        String tranBatch = batchID["batchID"].ToString().Trim(); // This is the batchID of the transaction.
                        String tranBatchNew = batchID["batchID"].ToString().Trim().Substring(0,8); // This is the batchID of the transaction.
                        if (tranBatchNew != todaysBatchNew.ToString().Trim()) //Only process transactions prior to today
                        {
                          
                            pp.processPayment_SC("csWCGCloud", "Select * from trans where batchId = '" + batchID["batchId"].ToString() + "' and actioned is null");// process all transactions for batchID
                        }
                    }
                }
            }
            // ============ END OF WCGOS processing START of WPM processing ==============================
            if (doManual)
            {
                Console.WriteLine("Press key to continue...");
                Console.ReadLine();
            }
            if (WpmP)// Do WPM processing
            {
                String batchID = "";
                String wpmPath = "";
                String wpmOutputPath = "";
                String wpmCPGPath = "";
                String wpmCPGOpenPathway = "";
                String wpmCPGOutputPath = "";
                String UBWdataImport = "";
                String wpmRCPPath = "";
                String wpmRCPPlansPath = ""; 
                String wpmRCPFailuresPath = "";

                // Don't forget to check the NAME of the EXECUTABLE before building.
                // change doManual to false if we are auto downloading and auto processing todays file.
                //get the latest downloads from WPM ===============================================================================
                //if (doManual) wpmPath = ".\\WPM_Files\\"; // change this path to the actual finance path or leave blank for the same path as the executable.
                //if (doManual) wpmOutputPath = ".\\OutputFiles\\"; // change this path to the actual finance path or leave blank for the same path as the executable.

                if (doManual) wpmPath = "\\\\mexico\\datafiles\\wpm\\WPM_Files\\"; // change this path to the actual finance path or leave blank for the same path as the executable.
                if (doManual) wpmOutputPath = "\\\\mexico\\datafiles\\wpm\\OutputFiles\\"; // change this path to the actual finance path or leave blank for the same path as the executable.
                                                                                            //use these PATHS to be used if creating Auto exe. Because the scheduler will create the folders in c:\windows\sysWow64
                                                                                            //if (!doManual) wpmPath = "C:\\Program Files (x86)\\Agresso 5.7.1\\Data Files\\agrLive\\wpm\\WPM_Files\\"; // change this path to the actual finance path or leave blank for the same path as the executable.
                                                                                            //if (!doManual) wpmOutputPath = "C:\\Program Files (x86) \\Agresso 5.7.1\\Data Files\\agrLive\\wpm\\OutputFiles\\"; // change this path to the actual finance path or leave blank for the same path as the executable.
                if (!doManual) wpmPath = "\\\\mexico\\datafiles\\wpm\\WPM_Files\\"; // change this path to the actual finance path or leave blank for the same path as the executable.
                if (!doManual) wpmOutputPath = "\\\\mexico\\datafiles\\wpm\\OutputFiles\\"; // change this path to the actual finance path or leave blank for the same path as the executable.
                if (!doManual) wpmCPGPath = "\\\\mexico\\datafiles\\wpm\\CPG\\WPM_Files\\"; // change this path to the actual finance path or leave blank for the same path as the executable.
                if (!doManual) wpmCPGOpenPathway = "\\\\mexico\\datafiles\\wpm\\CPG\\OpenPathway\\"; // change this path to the actual finance path or leave blank for the same path as the executable.
                if (!doManual) wpmCPGOutputPath = "\\\\mexico\\datafiles\\wpm\\CPG\\OutputFiles\\"; // change this path to the actual finance path or leave blank for the same path as the executable.
                if (!doManual) UBWdataImport = "\\\\mexico\\datafiles\\data Import\\"; // change this path to the actual finance path or leave blank for the same path as the executable.
                if (!doManual) wpmRCPPath = "\\\\mexico\\datafiles\\wpm\\CPG\\RCP\\"; // change this path to the actual finance path or leave blank for the same path as the executable.
                if (!doManual) wpmRCPPlansPath = "\\\\mexico\\datafiles\\wpm\\CPG\\RCP\\Plans\\ToDo\\"; // change this path to the actual finance path or leave blank for the same path as the executable.
                if (!doManual) wpmRCPFailuresPath = "\\\\mexico\\datafiles\\wpm\\CPG\\RCP\\Failures\\ToDo\\"; // change this path to the actual finance path or leave blank for the same path as the executable.
                                                                                      // "\\\\mexico\\datafiles\\data import\\"
                try
                {
                    if (!Directory.Exists(wpmPath))
                    {
                        Directory.CreateDirectory(wpmPath);//note CreateDirectory does nothing if the directory exists 
                    }                                      // so the Directory.Exists is redundant and we can remove this test if we want to. 
                    if (!Directory.Exists(wpmOutputPath))
                    {
                        Directory.CreateDirectory(wpmOutputPath);
                    }
                    if (!Directory.Exists(wpmCPGPath))
                    {
                        Directory.CreateDirectory(wpmCPGPath);//note CreateDirectory does nothing if the directory exists 
                    }                                      // so the Directory.Exists is redundant and we can remove this test if we want to. 
                    if (!Directory.Exists(wpmCPGOutputPath))
                    {
                        Directory.CreateDirectory(wpmCPGOutputPath);
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }

                // Process the WPM pathway download and upload files ================================================================
               
                WPM_Pathway wpm = new WPM_Pathway(wpmCPGPath, wpmCPGOpenPathway,UBWdataImport,wpmRCPPath, wpmRCPPlansPath, wpmRCPFailuresPath);

                wpm.ProcessCPGdownload();
                //wpm.ProcessCPGupload();
                wpm.ProcessRCPDownload();

                // This is thje new stuff for SFTP ===============================================
                wpm_sftp sftpClient = new wpm_sftp(@"ftp-dx.wpmeducation.com", "WAR0127-01_store_edu_wpmhost_net_download", "yvn?ncLXfK--eO#7");
                string[] simpleDirectoryListingx = sftpClient.simpledirectorylist("data_export");// get downloads from the data_export directory
                for (int i = 0; i < simpleDirectoryListingx.Count(); i++)
                {
                    //string xy = simpleDirectoryListingx[i];
                    //Boolean hasData = sftpClient.MyFTPFileHasData("data_export/" + simpleDirectoryListingx[i]);
                    //if (!File.Exists(wpmPath + simpleDirectoryListingx[i]) && simpleDirectoryListingx[i].Length > 0) sftpClient.download("data_export/" + simpleDirectoryListingx[i], wpmPath + simpleDirectoryListingx[i]);
                    if (sftpClient.MyFTPFileHasData("data_export/" + simpleDirectoryListingx[i])) utility.InsertFTPFile(simpleDirectoryListingx[i]);//If FTP file has data then add the filename to the table for processing.

                    if (!File.Exists(wpmPath + simpleDirectoryListingx[i]) && simpleDirectoryListingx[i].Length > 0)// if the file does not exist locally and the filename is greater than zero
                    {
                        // Write a record to toyota.utility.WPM_FTP_Files if the file has data

                        // then download file
                        sftpClient.download("data_export/" + simpleDirectoryListingx[i], wpmPath + simpleDirectoryListingx[i]);
                        if (doManual) Console.WriteLine("Downloaded file: " + simpleDirectoryListingx[i]);
                    }
                    else if (simpleDirectoryListingx[i].Length > 0) if (doManual) Console.WriteLine("This file exists locally, no need to download: " + simpleDirectoryListingx[i]);

                }
                sftpClient = null;

                //===============================================================================
                // Get FTP files from the online store
                //ftp ftpClient = new ftp(@" ftps://uk-ftp-1.wpmeducation.com:990", "WAR0127-01_store_edu_wpmhost_net_download", "yvn?ncLXfK--eO#7");
                //string[] simpleDirectoryListing = ftpClient.directoryListSimple("data_export");// get downloads from the data_export directory
                //for (int i = 0; i < simpleDirectoryListing.Count(); i++)
                //{
                //    if (ftpClient.MyFTPFileHasData("data_export/" + simpleDirectoryListing[i])) utility.InsertFTPFile(simpleDirectoryListing[i]);//If FTP file has data then add the filename to the table for processing.

                //    if (!File.Exists(wpmPath + simpleDirectoryListing[i]) && simpleDirectoryListing[i].Length > 0)// if the file does not exist locally and the filename is greater than zero
                //    {
                //        // Write a record to toyota.utility.WPM_FTP_Files if the file has data

                //        // then download file
                //        ftpClient.download("data_export/" + simpleDirectoryListing[i], wpmPath + simpleDirectoryListing[i]);
                //        if (doManual) Console.WriteLine("Downloaded file: " + simpleDirectoryListing[i]);
                //    }
                //    else if (simpleDirectoryListing[i].Length > 0) if (doManual) Console.WriteLine("This file exists locally, no need to download: " + simpleDirectoryListing[i]);
                //}
                //ftpClient = null;




                 // ==================================================================================================================

                String wpmOut = "";
                String wpmCopy = "";
                String myFile = "";
                Boolean gl07FileIsopen = false;
                // Get the current date as YYYYMMDD
                //DateTime myDate = DateTime.Now;
                //String currDate = myDate.ToString("yyyyMMdd");
                //int PayTotal;
                //String wpmPath = "D:\\qlrepdev\\wpm\\"; // change this path to the actual finance path
                if (doManual) Console.WriteLine("");
                if (doManual) Console.WriteLine("Please type in the date part of your File name or leave blank for todays file");// get the file to process, leave blank for todays file
                if (doManual) Console.WriteLine("e.g. if filename is GL07_Store_Payments_20180102.txt then type in 20180102");
                if (doManual) myFile = Console.ReadLine();
                String wpmF = "";
                //===================================================================================
                String wpmMonth = DateTime.Now.Month.ToString().PadLeft(2, '0');
                String wpmDay = DateTime.Now.Day.ToString().PadLeft(2, '0');
                String x = DateTime.Now.Year.ToString() + wpmMonth.Substring(wpmMonth.Length - 2) + wpmDay.Substring(wpmMonth.Length - 2);
                //===================================================================================
                String wpmDate = DateTime.Now.ToString("yyyyMMdd");
                String batchIDDate = DateTime.Now.AddDays(-1).ToString("yyyyMMdd") + "-WPM.TXT";//Data within the file is always for the previous day so take 1 day off today.

                wpmF = "GL07_Store_Payments_" + wpmDate + ".txt"; // todays file - not using this since I changed to the 'round tripping' with the toyota.utility.dbo.WPM_FTP_Files table
                //comemt this line out when going live
                //myFile = "20211213";
                if (myFile.Length > 1) // get the  filename to process, if this is blank then we use todays file, see above
                {
                    wpmF = "GL07_Store_Payments_" + myFile.ToString() + ".txt";
                    // batchIDDate = 
                    string[] format = { "yyyyMMdd" };
                    DateTime date;
                    date = DateTime.Now.AddDays(1);// set a default value which will not exist in case the value entered by the user is wrong.
                    if (DateTime.TryParseExact(myFile,
                                               format,
                                               System.Globalization.CultureInfo.InvariantCulture,
                                               System.Globalization.DateTimeStyles.None,
                                               out date))
                    {
                        //valid
                        batchIDDate = date.AddDays(-1).ToString("yyyyMMdd") + "-WPM.TXT";//IF manual then use the date of the requested file.  Data within the file is always for the previous day.
                    }
                }
                //if (args.Length != 0) // get the filename from the command line.  Decided NOT to use this.
                //{
                //    wpmF = args[0].Trim();
                //}
                StreamWriter writerX;
                if (doManual) Console.WriteLine("File to be processed is: " + wpmF);
                if (doManual) Console.WriteLine(File.Exists(wpmPath + wpmF) ? "File exists." : "File does not exist.");

                // Update the WPMFTPfiles table if the file has been processed e.g the batchID exists in the Batch Input Maintenance table (acrbatchinput)
                String sqlBim = "update wff set wff.BIMexist = 1 from utility.dbo.wpm_ftp_files wff inner join agrlive.dbo.acrbatchinput bi on wff.BatchID collate Latin1_General_CI_AS = bi.batch_id";
                DataView UpdateBIMexists = utility.readDataView("csUtility", sqlBim);

                // Now lets get the 
                String sqlFTP = "Select FTPfile, BatchID from utility.dbo.WPM_FTP_Files wff where wff.BIMexist = 0";
                DataView FTPfilesToProcess = utility.readDataView("csUtility",sqlFTP);

            // Here is where we can loop around the toyota.utility.dbo.WPM_FTP_files table for all those that have not been processed i.e. BIMexist = false

            foreach (DataRowView FTPfile in FTPfilesToProcess)
            {
                wpmF = FTPfile["FTPfile"].ToString().Trim();
                batchIDDate = FTPfile["BatchID"].ToString().Trim();
                //}
                // FTPfile has to exist and the output file in the data import folder must NOT exist i.e. do not process again as the file is awaiting import into UBW
                if (File.Exists(wpmPath + wpmF) && !File.Exists("\\\\mexico\\datafiles\\data import\\" + batchIDDate))
                {
                    using (StreamWriter writer = new StreamWriter(wpmOutputPath + "wpm_" + wpmF , false, Encoding.Unicode))// This is the OUTPUT file
                                                                                                                                      //using (StreamWriter writer = new StreamWriter("d:\\qlrepdev\\wpm\\wpm.txt", false, Encoding.Unicode))// This is the OUTPUT file
                    {
                        using (TextFieldParser tfp = new TextFieldParser(wpmPath + wpmF.ToString()))// + DateTime.Now.ToString("yyyyMMdd") + ".txt"))// This is the SOURCE file.
                                                                                                    //using (TextFieldParser tfp = new TextFieldParser(@"D:\qlrepdev\wpm\wpmF.ToString()))// + DateTime.Now.ToString("yyyyMMdd") + ".txt"))// This is the SOURCE file.
                        
                        //using (TextFieldParser tfp = new TextFieldParser(@"d:\qlrepdev\wpm\GL07_Store_Payments_" + DateTime.Now.ToString("yyyyMMdd") + ".txt"))// This is the SOURCE file.
                        {
                            // The TextFieldParser allows us to read a fixed width file and populate a String array based upon the
                            // SetFieldWidths definition.
                            tfp.TextFieldType = FieldType.FixedWidth;
                            tfp.TrimWhiteSpace = false; // Ensure none of the spaces in the fixed width data are truncated.
                                                        // Below is the fixed width definition as taken from the ABW _GL07_55 file.
                            tfp.SetFieldWidths(new int[] { 25, 25, 25, 2, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 2, 20, 20, 11,
                            20, 20, 20, 255, 8, 8, 15, 6, 1, 100, 255, 8, 8, 20, 25, 15, 27, 2, 1, 1,
                            25, 1, 15, 9, 25, 25, 25, 255, 160, 40, 40, 35, 2, 25, 15, 3, 25, 20, 20, 4,
                            2, 2, 13, 11, 15, 2});

                            int i = 0;
                            //PayTotal = 0;
                            //if (true)// for TESTING
                            if (Directory.Exists("\\\\mexico\\datafiles\\data import\\"))
                            {
                                while (!tfp.EndOfData)
                                {

                                    i = i + 1; // use this to append to the ext_inv_ref field in order to make that field unique
                                    string[] fields = tfp.ReadFields();  // remove any data here as we do not use this data.
                                    //wpmOut = "\\\\mexico\\datafiles\\data import\\" + fields[0].Trim() + ".txt";
                                    //wpmCopy = "\\\\mexico\\datafiles\\wpm\\WPM_Copy\\" + fields[0].Trim() + ".txt";
                                    wpmOut = "\\\\mexico\\datafiles\\data import\\" + batchIDDate;
                                    wpmCopy = "\\\\mexico\\datafiles\\wpm\\WPM_Copy\\" + batchIDDate;
                                    //wpmOut = "d:\\" + fields[0].Trim() + ".txt";
                                    try
                                    {
                                        //writerX = new StreamWriter("\\\\mexico\\datafiles\\data import\\" + fields[0].Trim() + ".txt", true, Encoding.Unicode);// This is the OUTPUT file
                                        using (writerX = new StreamWriter("\\\\mexico\\datafiles\\data import\\" + batchIDDate, true, Encoding.Unicode))
                                        {
                                            // This is the OUTPUT file
                                           //writerX = new StreamWriter("D:\\" + fields[0].Trim() + ".txt", true, Encoding.Unicode);// for TESTING

                                            // not using the GL07 class yet.  May do in the future
                                            GL07 fwx = new GL07(fields);
                                            //String bid = fw.batch_id;

                                            //fields[29] = ext_inv_ref - this needs to be unique
                                            //fields[5] = Account - this needs to be different for Trade customers.
                                            //fields[39] = apar_type = R if we are a sales ledger line
                                            //fields[40] = apar_id = customerID trade = C101380 which generally has a length of 7; the length > 5 and < 9 and begins with a C
                                            // studentID's generally are 11 characters long.
                                            // Change fields[5] to 21101 if match a trade ID
                                            // change apar_id to blank field 25 in length if apar_type <> R and there is something in the apar_id
                                            // for NON Sales Ledger lines this needs removing.
                                            //batchID = fields[0].Trim() + ".TXT";// the .txt appended to batchID means we can use [filename] in the IntellAgent for the batchID in the GL07
                                            batchID = batchIDDate;// this will allow for when the GL07 file has different batchID's
                                            fields[0] = batchID.PadRight(25);// rename the batchID field so that we can automate the import in ABW via IntellAgent
                                                                             //fields[1] = "BA".PadRight(25);// rename the batchID field so that we can automate the import in ABW via IntellAgent
                                            if (fields[5].Trim() == "24301")// This is the bank line.
                                            {
                                                fields[2] = "XX".PadRight(25);// set the voucher type to XX for the bank line only, this ensures that the transaction fails and it will go into the Batch Input maintenance screen
                                                                              // Gill can then see all of the transaction lines and amend the bank line to OP so that it will post.
                                            }
                                            if (fields[6].ToUpper().Trim() == "R")
                                            {
                                                //PayTotal = PayTotal + Convert.ToInt32(fields[18]);
                                                fields[6] = "".PadRight(25); // products  CostCentre data for field cat_1 () the remove any data here as we do not use this data.
                                            }
                                            fields[8] = "".PadRight(25); // products dim_3 data for field cat_3 () the remove any data here as we do not use this data.
                                            if (fields[13].ToUpper().Trim() == "SS")
                                            {
                                                fields[13] = "SG".PadRight(25); // Sales TAX from the online store needs setting to SG from SS
                                            }
                                            if (fields[39].ToUpper().Trim() != "R" && fields[40].Trim().Length > 0)
                                            {
                                                fields[40] = "".PadRight(25);// remove the apar_id data for non sales ledger data
                                            }
                                            if (fields[39].ToUpper().Trim() == "R" && fields[40].Trim().Length > 5 && fields[40].Trim().Length < 9 && fields[40].Substring(0, 1).ToUpper() == "C")
                                            {
                                                fields[5] = "21101".PadRight(25);// this is a trade account so change the Account to the trade account 21101
                                            }
                                            if (fields[39].ToUpper().Trim() == "R")
                                            {
                                                String newS = fields[29].Trim() + " ^" + i; // add an extra number onto the end of the order number for Sales Ledger lines.
                                                fields[29] = newS.PadRight(100);// make the ext_inv_ref field unique - this only applys to the ABW test system.
                                            }
                                            // Create a fixed width line from the String array of fixed width values.
                                            //test for a NON bankline and only write NON bank lines - to be done maybe
                                            String newLine = "";
                                            foreach (String field in fields)
                                            {
                                                newLine = newLine + field;
                                            }

                                            writer.WriteLine(newLine);// Write the updated fixed width data to file for local use.
                                            writerX.WriteLine(newLine);// Write the updated fixed width data to file for mexico.
                                        }
                                        //if (fields[5].ToString() == "24301") // then this is the bank line which is the last line in the file so we can close the streamwriter
                                        //{
                                        //    writerX.Close();
                                        //    writerX.Dispose();  //}//if fileOpen
                                        //}
                                    }
                                    catch (Exception ex) { Console.WriteLine("ERROR: " + ex.ToString()); }
                                }
                            }
                            else { if (doManual) Console.WriteLine("You do not have access to \\\\MEXICO\\DataFiles\\Data Import"); }


                            tfp.Close();
                            if (File.Exists(wpmOut)) if (doManual) Console.WriteLine(wpmOut + " Successfully created");

                            if (File.Exists(wpmOut))
                            {
                                if (!File.Exists(wpmCopy)) File.Copy(wpmOut,wpmCopy); // this is a copy of the data import file in the copy directory
                            }

                        }

                        //create a new GL bank line for each type
                        // PaymentTotal
                        // BankNonPayLineTotal = BankTotal - PaymentTotal
                        // Write bank line for Payment

                        //Write bank line for NonPayment

                        //-----------------------------------------------------------------------------------------------------------
                        // using the Streamwriter works effectively for our system as we are already dealing with a fixed width format
                        // from the TextFieldParser
                        // This may be useful to write A FIXED WIDTH FILE then look at this link
                        // http://www.blackwasp.co.uk/WriteFixedWidth.aspx



                        writer.Close();
                        writer.Dispose();
                        //StreamReader sr = new StreamReader("d:\\qlrepdev\\wpm\\wpm.txt");
                        //string s = sr.ReadToEnd();
                        //sr.Close();
                    }// end of using
                    if (doManual) Console.WriteLine("Press Enter key to continue");
                    if (doManual) Console.ReadLine();
                }// end of file test
                else // the file does NOT exist
                {
                    if (doManual) Console.WriteLine("Press Enter key to continue");
                    if (doManual) Console.ReadLine();

                }
            } // End of the foreach FTPfile loop


                //FTP stuff
                /* Create Object Instance */
                /* Create Object Instance */
                //ftp ftpClient = new ftp(@" ftp://uk-ftp-1.wpmeducation.com", "WAR0127-01_store_edu_wpmhost_net_download", "yvn?ncLXfK--eO#7");

                /* Upload a File */
                //ftpClient.upload("etc/test.txt", @"C:\Users\metastruct\Desktop\test.txt");

                /* Download a File */
                //ftpClient.download("data_export/GL07_Store_Payments_20180704.txt", @"D:\testMe.txt");

                /* Delete a File */
                //ftpClient.delete("etc/test.txt");

                /* Rename a File */
                //ftpClient.rename("etc/test.txt", "test2.txt");

                /* Create a New Directory */
                //ftpClient.createDirectory("etc/test");

                /* Get the Date/Time a File was Created */
                //string fileDateTime = ftpClient.getFileCreatedDateTime("etc/test.txt");
                //Console.WriteLine(fileDateTime);

                /* Get the Size of a File */
                //string fileSize = ftpClient.getFileSize("etc/test.txt");
                //Console.WriteLine(fileSize);

                /* Get Contents of a Directory (Names Only) */
                //string[] simpleDirectoryListing = ftpClient.directoryListSimple("data_export");
                //for (int i = 0; i < simpleDirectoryListing.Count(); i++)
                //{
                //    Console.WriteLine(simpleDirectoryListing[i]);
                //    if (!File.Exists(simpleDirectoryListing[i]) && simpleDirectoryListing[i].Length > 0)
                //    {
                //        // then download file
                //        ftpClient.download("data_export/" + simpleDirectoryListing[i] , simpleDirectoryListing[i]);

                //    }
                //}

                /* Get Contents of a Directory with Detailed File/Directory Info */
                //string[] detailDirectoryListing = ftpClient.directoryListDetailed("/data_export");
                //for (int i = 0; i < detailDirectoryListing.Count(); i++) { Console.WriteLine(detailDirectoryListing[i]); }
                ///* Release Resources */
                //ftpClient = null;
            }

        } // End of WPM processing
    }

}
