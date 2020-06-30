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
