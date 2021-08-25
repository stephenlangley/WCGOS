using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;

namespace WPM
{
    class utility
    {


        public static Boolean IsProcessed(String FTPfile)
        {
            String aCSName = "csUtility";
            String aSqlQuery = "Select * from utility.dbo.WPM_FTP_Files wff where wff.BIMexist = 1 and wff.FTPfile = '" + FTPfile + "' ";

            Boolean processed = false;
            DataView transData = utility.readDataView(aCSName, aSqlQuery); // Get the data from the cloud

            // Do a foreach here to get the total for the bankline for amount, net and fee
            foreach (DataRowView drv in transData)// for each transaction
            {
                String bim = drv["BIMexist"].ToString();// Payment
                processed = true;

            }
            return processed;
        }

        public static Boolean InsertFTPFile(String FTPfile)
        {
            String aCSName = "csUtility";
            String aSqlQuery = "";

            Boolean processed = false;
            // convert the FTPfile date to a BatchID date which is the previous days date
            string fileDate = FTPfile.Substring(20, 8);
            DateTime dt = DateTime.ParseExact(fileDate, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture).AddDays(-1);
            string batchIDdate = dt.ToString("yyyyMMdd") + "-WPM.TXT";

            aSqlQuery = "if not exists (select * from utility.dbo.WPM_FTP_FILES where FTPFile = '" + FTPfile + "') ";
            aSqlQuery = aSqlQuery +  "Insert into utility.dbo.WPM_FTP_FILES (FTPfile, BatchID) values ('" + FTPfile + "' , '" + batchIDdate + "')";
            aSqlQuery = aSqlQuery + " select * from utility.dbo.WPM_FTP_FILES where FTPFile = '" + FTPfile + "'";
            DataView transData = utility.readDataView(aCSName, aSqlQuery); // Get the data from the cloud

            // Do a foreach here to get the total for the bankline for amount, net and fee
            foreach (DataRowView drv in transData)// for each transaction
            {
                String bim = drv["BIMexist"].ToString();// Payment
                processed = true;

            }
            return processed;
        }
        public static DataView readDataView(String aCSName, String aSqlQuery)
        {
            // ConnectionStrings are in the machine.config of the .net4 x86 fram
            //if (putLog(aCSName, aSqlQuery))
            //{
            ConnectionStringSettingsCollection connections =
                ConfigurationManager.ConnectionStrings;
            DataTable dt = new DataTable();
            //    SqlConnection connUtility = new SqlConnection(strConn);
            SqlConnection connUtility = new SqlConnection(connections[aCSName].ConnectionString);

            try
            {
                SqlDataAdapter adp = new SqlDataAdapter();
                SqlCommand cmd = new SqlCommand(aSqlQuery, connUtility);
                cmd.CommandTimeout = 180;// added this line because we were getting numerous in app timeouts @23/11/2017.  SP was fine on the server.
                adp.SelectCommand = cmd;
                try { adp.Fill(dt); }
                //catch (Exception ue) { putLog(aCSName + ":dataView Error", ue.Message); };
                catch (Exception ue)
                {
                    string myUE = ue.Message;
                }
            }
            finally { connUtility.Close(); }
            return dt.DefaultView;
            //}
            //else
            //    return null;
        }

    }
}
