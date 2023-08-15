using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Microsoft.VisualBasic.FileIO;
using System.Data;
using System.IO;
using System.Data.SqlClient;
using System.Collections;
using System.Configuration;

namespace WPM
{
    class WCGOS
    {
        // Warwickshire College Online Shop (WCGOS)
        public WCGOS()
        {
            String x = "we are here";
        }


 

        public void processPayment_SC(String aCSName, String aSqlQuery)
        {
            // Process a payment for the Short Courses.
            GL07 fw = new GL07(); // Create a GL07 class
            string jsonData = "";
            string batchID = "";
            double totAmount = 0;
            double amount = 0.0;
            double totNet = 0;
            double net = 0.0;
            double totFee = 0;
            double fee = 0.0;

            StreamWriter writerWCGOS;
            String stcsessdSQL = "";
            String courseName = "";

            // Enryption handler NOT being used as at 12/03/2019.
            // Initially tested the handler and it worked fine - MIKE BRADLEY  will need to encrypt the STRIPE transaction before using this again..
            EncryptionHelper eH = new EncryptionHelper(); //Create an Encryption handler

            Console.WriteLine("Process payment for " + aSqlQuery + " in " + aCSName);
            DataView transData = utility.readDataView(aCSName, aSqlQuery); // Get the data from the cloud

            // Do a foreach here to get the total for the bankline for amount, net and fee
            foreach (DataRowView drv in transData)// for each transaction
            {
                double.TryParse(drv["amount"].ToString(),out amount);// Payment
                totAmount = totAmount +  amount;
                double.TryParse(drv["net"].ToString(), out net); // Payment  minus the Fee
                totNet = totNet + net;
                double.TryParse(drv["fee"].ToString(), out fee); // Fee
                totFee = totFee + fee;
            }
            // --------------------------------------------------------------------------------------------------------------------------------------------------------
            // Create the GL07 file here so that we can write the GL07 transaction lines.

            // Iterate through each transaction line
            foreach (DataRowView drv in transData)// for each transaction
                {

                    // get the QLSDAT.STCSESSD description Course Session name.
                    stcsessdSQL = "select  rtrim(ltrim(st1.full_desc)) title From stcsessd st1 where rtrim(ltrim(aos_code)) + '/' + rtrim(ltrim(acad_period)) + '/' + rtrim(ltrim(aos_period)) = '" + drv["courseCode"].ToString().Trim() + "'";
                    DataView stcsessd = utility.readDataView("csQLSDAT", stcsessdSQL);
                    //if (stcsessd.Count > 0) courseName = stcsessd[0]["title"].ToString();
                    foreach (DataRowView dataR in stcsessd)
                    {
                        courseName = dataR["title"].ToString();
                    }

                //jsonData = eH.DecryptString(drv["data"].ToString());// Decrypt the DATA

                jsonData = drv["data"].ToString(); // this is the json data from the STRIPE transaction
                //List<Dictionary<string, string>> obj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, string>>>("[" + jsonData + "]");

                //Return the data into the GL07 Class properties with the DeserializeObject function
                fw = Newtonsoft.Json.JsonConvert.DeserializeObject<GL07>(jsonData);

                batchID = fw.BatchId.Trim().ToString();
                fw.BatchId = batchID.ToString() + ".TXT";
                fw.Description = fw.Description.ToString().Trim() + ", " + courseName; // Append the session course name to the description

                writerWCGOS = new StreamWriter("\\\\mexico\\datafiles\\data import\\" + batchID.ToString() + ".TXT", true, Encoding.Unicode);// This is the OUTPUT file
                //writerWCGOS = new StreamWriter("D:\\" + batchID.ToString() + ".TXT", true, Encoding.Unicode);// This is the OUTPUT file

                String GL07Line = fw.beautifyGL07(); // Create the fixed width GL07 line
                writerWCGOS.WriteLine(GL07Line);
                writerWCGOS.Close();
                writerWCGOS.Dispose();

            }
            // Create a fee line here =========================================================================================================================================================
            // We only need to update the relevant fields and clear any data from the fields that have been used previously
            //if (fw.Amount > 0)
            //{
                fw.TransType = "GL".PadRight(2); // The FEE line goes to the General Ledger
                fw.Account = "88201".PadRight(25); // The STRIPE FEE account is 88201
                fw.Cat1 = "".PadRight(25); // Clear the cat field
                fw.Cat2 = "P020C".PadRight(25); // This is the PROJECT field - set to P019C for STRIPE FEES
                fw.Cat3 = "".PadRight(25); // Clear the Cat fields
                fw.Cat4 = "".PadRight(25); // Clear the Cat fields
                fw.Cat5 = "".PadRight(25); // Clear the Cat fields
                fw.Cat6 = "".PadRight(25); // Clear the Cat fields
                fw.TaxCode = "0".PadRight(25); // Clear the tax code
                fw.CurAmount = totFee.ToString().PadLeft(20); // Total amount for the bank - this is calculated above.
                fw.Amount = totFee.ToString().PadLeft(20); // Total amount for the bank
                fw.Description = "STRIPE Acc:12345678 FEES for ShuttleID".PadRight(255); // Description of the bank transaction i.e. Cardnet 54063298 Worlpay - We are using STRIPE
                fw.ExtInvRef = "Banking Date " + fw.TransDate.Trim();
                fw.ExtInvRef.PadRight(100);
                fw.ExtRef = fw.Description.ToString().PadRight(255);
                fw.AparType = "".PadRight(1);
                fw.AparId = "".PadRight(25);
                fw.PayMethod = "".PadRight(2);

                String GL07FeeLine = fw.beautifyGL07(); // Create the fixed width GL07 line for the FEE details

                //// write the bank line total to the file
                writerWCGOS = new StreamWriter("\\\\mexico\\datafiles\\data import\\" + batchID.ToString() + ".TXT", true, Encoding.Unicode);// This is the OUTPUT file
                //writerWCGOS = new StreamWriter("D:\\" + batchID.ToString() + ".TXT", true, Encoding.Unicode);// This is the OUTPUT file
                writerWCGOS.WriteLine(GL07FeeLine);
            //}
            // Create a bank line here with the bankline total - Should be able to just overwrite the last payment line values which need changing. ===========================================
            fw.VoucherType = "XX".PadRight(2); // The bank line goes to the General Ledger - NOTE XX is here so that the transaction fails in ABW and the data is forced into the 'Maintenance Of Batch Input'
            fw.Account = "24301".PadRight(25); // The bank account is 24301
            fw.Cat1 = "".PadRight(25); // Clear the Cat fields
            fw.Cat2 = "".PadRight(25); // Clear the Cat fields
            fw.Cat3 = "".PadRight(25); // Clear the Cat fields
            fw.Cat4 = "".PadRight(25); // Clear the Cat fields
            fw.Cat5 = "".PadRight(25); // Clear the Cat fields
            fw.Cat6 = "".PadRight(25); // Clear the Cat fields
            fw.TaxCode = "0".PadRight(25); // Clear the tax code
            fw.CurAmount = totNet.ToString().PadLeft(20); // Total NET amount for the bank - this is calculated above.
            fw.Amount = totNet.ToString().PadLeft(20); // Total NET amount for the bank - this is basically the payment minus any fees that stripe take out.
            fw.Description = "STRIPE Acc:12345678 BANK for ShuttleID".PadRight(255); // Description of the bank transaction i.e. Cardnet 54063298 Worlpay - We are using STRIPE
            fw.ExtInvRef = "Banking Date " + fw.TransDate.Trim();
            fw.ExtInvRef.PadRight(100);
            fw.ExtRef = fw.Description.ToString().PadRight(255);
            fw.AparType = "".PadRight(1);
            fw.AparId = "".PadRight(25);
            fw.PayMethod = "".PadRight(2);

            String GL07BankLine = fw.beautifyGL07(); // Create the fixed width GL07 line for the BANK details

            //// write the bank line total to the file
            //writerWCGOS = new StreamWriter("\\\\mexico\\datafiles\\data import\\" + batchID, true, Encoding.Unicode);// This is the OUTPUT file
            //writerWCGOS = new StreamWriter("D:\\" + batchID.ToString() + ".TXT", true, Encoding.Unicode);// This is the OUTPUT file
            writerWCGOS.WriteLine(GL07BankLine);


            // Close the file here
            writerWCGOS.Close();
            writerWCGOS.Dispose();

            //if file exists update the batch rows in the database setting actioned = todays date.  This will prevent it from running again.
            //if (File.Exists("D:\\" + batchID.ToString() + ".TXT"))
            if (File.Exists("\\\\mexico\\datafiles\\data import\\" + batchID.ToString() + ".TXT"))
            {
                String sql = "Update tData Set actioned = getdate()  from trans tData where batchId = '" + batchID.ToString() + "'";
                DataView updData = utility.readDataView("csWCGCloud", sql);
                // copy file to location
                if (!File.Exists("\\\\mexico\\datafiles\\WCGOS\\" + batchID.ToString() + ".TXT"))
                { 
                    File.Copy("\\\\mexico\\datafiles\\data import\\" + batchID.ToString() + ".TXT", "\\\\mexico\\datafiles\\WCGOS\\" + batchID.ToString() + ".TXT");
                }
            }

            //String GL07Line = fw.BatchId + fw.Interface + fw.VoucherType + fw.TransType + fw.Client
            //    + fw.Account + fw.Cat1 + fw.Cat2 + fw.Cat3 + fw.Cat4
            //    + fw.Cat5 + fw.Cat6 + fw.Cat7 + fw.TaxCode + fw.TaxSystem + fw.Currency
            //    + fw.DcFlag + fw.CurAmount + fw.Amount + fw.Number1 + fw.Value1 + fw.Value2
            //    + fw.Value3 + fw.Description + fw.TransDate + fw.VoucherDate + fw.VoucherNo
            //    + fw.Period + fw.TaxFlag + fw.ExtInvRef + fw.ExtRef + fw.DueDate + fw.DiscDate
            //    + fw.Discount + fw.Commitment + fw.OrderId + fw.Kid + fw.PayTransfer + fw.Status
            //    + fw.AparType + fw.AparId + fw.PayFlag + fw.VoucherRef + fw.SequenceRef + fw.IntruleId
            //    + fw.FactorShort + fw.Responsible + fw.AparName + fw.Address + fw.Province + fw.Place
            //    + fw.BankAccount + fw.PayMethod + fw.VatRegNo + fw.ZipCode + fw.CurrLicence + fw.Account2
            //    + fw.BaseAmount + fw.BaseCurr + fw.PayTempId + fw.AllocationKey + fw.PeriodNo + fw.Clearingcode
            //    + fw.Swift + fw.Arriveid + fw.BankAccType
            //     ;
            //String GL07BankLine = fw.BatchId.Trim().PadRight(25) + fw.Interface.Trim().PadRight(25) + fw.VoucherType.Trim().PadRight(25) + fw.TransType.Trim().PadRight(2) + fw.Client.Trim().PadRight(25)
            //    + fw.Account.Trim().PadRight(25) + fw.Cat1.Trim().PadRight(25) + fw.Cat2.Trim().PadRight(25) + fw.Cat3.Trim().PadRight(25) + fw.Cat4.Trim().PadRight(25)
            //    + fw.Cat5.Trim().PadRight(25) + fw.Cat6.Trim().PadRight(25) + fw.Cat7.Trim().PadRight(25) + fw.TaxCode.Trim().PadRight(25) + fw.TaxSystem.Trim().PadRight(25) + fw.Currency.Trim().PadRight(25)
            //    + fw.DcFlag.Trim().PadRight(2) + fw.CurAmount.Trim().PadLeft(20) + fw.Amount.Trim().PadLeft(20) + fw.Number1.Trim().PadLeft(11) + fw.Value1.Trim().PadLeft(20) + fw.Value2.Trim().PadLeft(20)
            //    + fw.Value3.Trim().PadLeft(20) + fw.Description.Trim().PadRight(255) + fw.TransDate.Trim().PadRight(8) + fw.VoucherDate.Trim().PadRight(8) + fw.VoucherNo.Trim().PadRight(15)
            //    + fw.Period.Trim().PadRight(6) + fw.TaxFlag.Trim().PadRight(1) + fw.ExtInvRef.Trim().PadRight(100) + fw.ExtRef.Trim().PadRight(255) + fw.DueDate.Trim().PadRight(8) + fw.DiscDate.Trim().PadRight(8)
            //    + fw.Discount.Trim().PadRight(20) + fw.Commitment.Trim().PadLeft(25) + fw.OrderId.Trim().PadRight(15) + fw.Kid.Trim().PadRight(27) + fw.PayTransfer.Trim().PadRight(2) + fw.Status.Trim().PadRight(1)
            //    + fw.AparType.Trim().PadRight(1) + fw.AparId.Trim().PadRight(25) + fw.PayFlag.Trim().PadRight(1) + fw.VoucherRef.Trim().PadRight(15) + fw.SequenceRef.Trim().PadRight(9) + fw.IntruleId.Trim().PadRight(25)
            //    + fw.FactorShort.Trim().PadRight(25) + fw.Responsible.Trim().PadRight(25) + fw.AparName.Trim().PadRight(255) + fw.Address.Trim().PadRight(160) + fw.Province.Trim().PadRight(40) + fw.Place.Trim().PadRight(40)
            //    + fw.BankAccount.Trim().PadRight(35) + fw.PayMethod?.Trim().PadRight(2).Substring(0, 2) ?? "".PadRight(2) + fw.VatRegNo.Trim().PadRight(25) + fw.ZipCode.Trim().PadRight(15) + fw.CurrLicence.Trim().PadRight(3) + fw.Account2.Trim().PadRight(25)
            //    + fw.BaseAmount.Trim().PadLeft(20) + fw.BaseCurr.Trim().PadLeft(20) + fw.PayTempId.Trim().PadRight(4) + fw.AllocationKey.Trim().PadRight(2) + fw.PeriodNo.Trim().PadRight(2) + fw.Clearingcode.Trim().PadRight(13)
            //    + fw.Swift.Trim().PadRight(11) + fw.Arriveid.Trim().PadRight(15) + fw.BankAccType.Trim().PadRight(2)
            //     ;

        }
    }
}
