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
    class GL07
    {
        // This signature with the field names allows us to deserialise the JSONdata from the STRIPE transaction into the field names(the field names match the JSON data field names)
        // Newtonsoft.Json.JsonConvert.DeserializeObject<GL07>(jsonData)
        public String BatchId, Interface, VoucherType, TransType, Client, Account, Cat1, Cat2, Cat3, Cat4,
            Cat5, Cat6, Cat7, TaxCode, TaxSystem, Currency, DcFlag, CurAmount, Amount, Number1,
            Value1, Value2, Value3, Description, TransDate, VoucherDate, VoucherNo, Period, TaxFlag, ExtInvRef,
            ExtRef, DueDate, DiscDate, Discount, Commitment, OrderId, Kid, PayTransfer, Status, AparType,
            AparId, PayFlag, VoucherRef, SequenceRef, IntruleId, FactorShort, Responsible, AparName, Address, Province,
            Place, BankAccount, PayMethod, VatRegNo, ZipCode, CurrLicence, Account2, BaseAmount, BaseCurr, PayTempId,
            AllocationKey, PeriodNo, Clearingcode, Swift, Arriveid, BankAccType
        ;
        // OLD signature notice the case, this did NOT work with the deserialise
     //   public String batch_id, myinterface, voucher_type, trans_type, client, account, dim_1, dim_2, dim_3, dim_4,
     //    dim_5, dim_6, dim_7, tax_code, tax_system, currency, dc_flag, cur_amount, amount, number_1,
     //    value_1, value_2, value_3, description, trans_date, voucher_date, voucher_no, period, tax_flag, ext_inv_ref,
     //    ext_ref, due_date, disc_date, discount, commitment, order_id, kid, pay_transfer, status, apar_type,
     //    apar_id, pay_flag, voucher_ref, sequence_ref, intrule_id, factor_short, responsible, apar_name, address, province,
     //    place, bank_account, pay_method, vat_reg_no, zip_code, curr_licence, account2, base_amount, base_curr, pay_temp_id,
     //    allocation_key, period_no, clearing_code, swift, arrive_id, bank_acc_type
     //;
     
        public String beautifyGL07()
        {
            // this function will process the NULL values and return a GL07 fixed width line 
            // ? allows null to not give a runtime error
            // ?? is effectively a coalesce.  do the right side of the ?? if the left side is null else do the left side
            PayMethod = PayMethod?.Trim().PadRight(2).Substring(0, 2) ?? "".PadRight(2);
            BatchId = BatchId?.Trim().PadRight(25).Substring(0, 25) ?? "".PadRight(25);
            Interface = Interface?.Trim().PadRight(25).Substring(0, 25) ?? "".PadRight(25);
            VoucherType = VoucherType?.Trim().PadRight(25).Substring(0, 25) ?? "".PadRight(25);
            TransType = TransType?.Trim().PadRight(2).Substring(0, 2) ?? "".PadRight(2);
            Client = Client?.Trim().PadRight(25).Substring(0, 25) ?? "".PadRight(25);
            Account = Account?.Trim().PadRight(25).Substring(0, 25) ?? "".PadRight(25);
            Cat1 = Cat1?.Trim().PadRight(25).Substring(0, 25) ?? "".PadRight(25);
            Cat2 = Cat2?.Trim().PadRight(25).Substring(0, 25) ?? "".PadRight(25);
            Cat3 = Cat3?.Trim().PadRight(25).Substring(0, 25) ?? "".PadRight(25);
            Cat4 = Cat4?.Trim().PadRight(25).Substring(0, 25) ?? "".PadRight(25);
            Cat5 = Cat5?.Trim().PadRight(25).Substring(0, 25) ?? "".PadRight(25);
            Cat6 = Cat6?.Trim().PadRight(25).Substring(0, 25) ?? "".PadRight(25);
            Cat7 = Cat7?.Trim().PadRight(25).Substring(0, 25) ?? "".PadRight(25);
            TaxCode = TaxCode?.Trim().PadRight(25).Substring(0, 25) ?? "".PadRight(25);
            TaxSystem = TaxSystem?.Trim().PadRight(25).Substring(0, 25) ?? "".PadRight(25);
            Currency = Currency?.Trim().PadRight(25).Substring(0, 25) ?? "".PadRight(25);
            DcFlag = DcFlag?.Trim().PadRight(2).Substring(0, 2) ?? "".PadRight(2);
            CurAmount = CurAmount.Trim().PadLeft(20);
            Amount = Amount.Trim().PadLeft(20);
            Number1 = Number1.Trim().PadLeft(11);
            Value1 = Value1.Trim().PadLeft(20);
            Value2 = Value2.Trim().PadLeft(20);
            Value3 = Value3.Trim().PadLeft(20);
            Description = Description?.Trim().PadRight(255).Substring(0, 255) ?? "".PadRight(255);
            TransDate = TransDate?.Trim().PadRight(8).Substring(0, 8) ?? "".PadRight(8);
            VoucherDate = VoucherDate?.Trim().PadRight(8).Substring(0, 8) ?? "".PadRight(8);
            VoucherNo = VoucherNo?.Trim().PadRight(15).Substring(0, 15) ?? "".PadRight(15);
            Period = Period?.Trim().PadRight(6).Substring(0, 6) ?? "".PadRight(6);
            TaxFlag = TaxFlag?.Trim().PadRight(1).Substring(0, 1) ?? "".PadRight(1);
            ExtInvRef = ExtInvRef?.Trim().PadRight(100).Substring(0, 100) ?? "".PadRight(100);
            ExtRef = ExtRef?.Trim().PadRight(255).Substring(0, 255) ?? "".PadRight(255);
            DueDate = DueDate?.Trim().PadRight(8).Substring(0, 8) ?? "".PadRight(8);
            DiscDate = DiscDate?.Trim().PadRight(8).Substring(0, 8) ?? "".PadRight(8);
            Discount = Discount?.Trim().PadRight(20).Substring(0, 20) ?? "".PadRight(20);
            Commitment = Commitment?.Trim().PadLeft(25).Substring(0, 25) ?? "".PadRight(25);
            OrderId = OrderId?.Trim().PadRight(15).Substring(0, 15) ?? "".PadRight(15);
            Kid = Kid?.Trim().PadRight(27).Substring(0, 27) ?? "".PadRight(27);
            PayTransfer = PayTransfer?.Trim().PadRight(2).Substring(0, 2) ?? "".PadRight(2);
            Status = Status?.Trim().PadRight(1).Substring(0, 1) ?? "".PadRight(1);
            AparType = AparType?.Trim().PadRight(1).Substring(0, 1) ?? "".PadRight(1);
            AparId = AparId?.Trim().PadRight(25).Substring(0, 25) ?? "".PadRight(25);
            PayFlag = PayFlag?.Trim().PadRight(1).Substring(0, 1) ?? "".PadRight(1);
            VoucherRef = VoucherRef?.Trim().PadRight(15).Substring(0, 15) ?? "".PadRight(15);
            SequenceRef = SequenceRef?.Trim().PadRight(9).Substring(0, 9) ?? "".PadRight(9);
            IntruleId = IntruleId?.Trim().PadRight(25).Substring(0, 25) ?? "".PadRight(25);
            FactorShort = FactorShort?.Trim().PadRight(25).Substring(0, 25) ?? "".PadRight(25);
            Responsible = Responsible?.Trim().PadRight(25).Substring(0, 25) ?? "".PadRight(25);
            AparName = AparName?.Trim().PadRight(255).Substring(0, 255) ?? "".PadRight(255);
            Address = Address?.Trim().PadRight(160).Substring(0, 160) ?? "".PadRight(160);
            Province = Province?.Trim().PadRight(40).Substring(0, 40) ?? "".PadRight(40);
            Place = Place?.Trim().PadRight(40).Substring(0, 40) ?? "".PadRight(40);
            BankAccount = BankAccount?.Trim().PadRight(35).Substring(0, 35) ?? "".PadRight(35);
            PayMethod = PayMethod?.Trim().PadRight(2).Substring(0, 2) ?? "".PadRight(2);
            VatRegNo = VatRegNo?.Trim().PadRight(25).Substring(0, 25) ?? "".PadRight(25);
            ZipCode = ZipCode?.Trim().PadRight(15).Substring(0, 15) ?? "".PadRight(15);
            CurrLicence = CurrLicence?.Trim().PadRight(3).Substring(0, 3) ?? "".PadRight(3);
            Account2 = Account2?.Trim().PadRight(25).Substring(0, 25) ?? "".PadRight(25);
            BaseAmount = BaseAmount?.Trim().PadLeft(20).Substring(0, 20) ?? "".PadRight(20);
            BaseCurr = BaseCurr?.Trim().PadLeft(20).Substring(0, 20) ?? "".PadRight(20);
            PayTempId = PayTempId?.Trim().PadRight(4).Substring(0, 4) ?? "".PadRight(4);
            AllocationKey = AllocationKey?.Trim().PadRight(2).Substring(0, 2) ?? "".PadRight(2);
            PeriodNo = PeriodNo?.Trim().PadRight(2).Substring(0, 2) ?? "".PadRight(2);
            Clearingcode = Clearingcode?.Trim().PadRight(13).Substring(0, 13) ?? "".PadRight(13);
            Swift = Swift?.Trim().PadRight(11).Substring(0, 11) ?? "".PadRight(11);
            Arriveid = Arriveid?.Trim().PadRight(15).Substring(0, 15) ?? "".PadRight(15);
            BankAccType = BankAccType?.Trim().PadRight(2).Substring(0, 2) ?? "".PadRight(2);

            return BatchId + Interface + VoucherType + TransType + Client
                    + Account + Cat1 + Cat2 + Cat3 + Cat4
                    + Cat5 + Cat6 + Cat7 + TaxCode + TaxSystem + Currency
                    + DcFlag + CurAmount + Amount + Number1 + Value1 + Value2
                    + Value3 + Description + TransDate + VoucherDate + VoucherNo
                    + Period + TaxFlag + ExtInvRef + ExtRef + DueDate + DiscDate
                    + Discount + Commitment + OrderId + Kid + PayTransfer + Status
                    + AparType + AparId + PayFlag + VoucherRef + SequenceRef + IntruleId
                    + FactorShort + Responsible + AparName + Address + Province + Place
                    + BankAccount + PayMethod + VatRegNo + ZipCode + CurrLicence + Account2
                    + BaseAmount + BaseCurr + PayTempId + AllocationKey + PeriodNo + Clearingcode
                    + Swift + Arriveid + BankAccType
                     ;


            //       String GL07Line = fw.BatchId.Trim().PadRight(25) + fw.Interface?.Trim().PadRight(25) + fw.VoucherType.Trim().PadRight(25) + fw.TransType.Trim().PadRight(2) + fw.Client.Trim().PadRight(25)
            //+ fw.Account.Trim().PadRight(25) + fw.Cat1.Trim().PadRight(25) + fw.Cat2.Trim().PadRight(25) + fw.Cat3.Trim().PadRight(25) + fw.Cat4.Trim().PadRight(25)
            //+ fw.Cat5.Trim().PadRight(25) + fw.Cat6.Trim().PadRight(25) + fw.Cat7.Trim().PadRight(25) + fw.TaxCode.Trim().PadRight(25) + fw.TaxSystem.Trim().PadRight(25) + fw.Currency.Trim().PadRight(25)
            //+ fw.DcFlag.Trim().PadRight(2) + fw.CurAmount.Trim().PadLeft(20) + fw.Amount.Trim().PadLeft(20) + fw.Number1.Trim().PadLeft(11) + fw.Value1.Trim().PadLeft(20) + fw.Value2.Trim().PadLeft(20)
            //+ fw.Value3.Trim().PadLeft(20) + fw.Description.Trim().PadRight(255) + fw.TransDate.Trim().PadRight(8) + fw.VoucherDate.Trim().PadRight(8) + fw.VoucherNo.Trim().PadRight(15)
            //+ fw.Period.Trim().PadRight(6) + fw.TaxFlag.Trim().PadRight(1) + fw.ExtInvRef.Trim().PadRight(100) + fw.ExtRef.Trim().PadRight(255) + fw.DueDate.Trim().PadRight(8) + fw.DiscDate.Trim().PadRight(8)
            //+ fw.Discount.Trim().PadRight(20) + fw.Commitment.Trim().PadLeft(25) + fw.OrderId.Trim().PadRight(15) + fw.Kid.Trim().PadRight(27) + fw.PayTransfer.Trim().PadRight(2) + fw.PayTransfer.Trim().PadRight(1)
            //+ fw.AparType.Trim().PadRight(1) + fw.AparId.Trim().PadRight(25) + fw.PayFlag.Trim().PadRight(1) + fw.VoucherRef.Trim().PadRight(15) + fw.SequenceRef.Trim().PadRight(9) + fw.IntruleId.Trim().PadRight(25)
            //+ fw.FactorShort.Trim().PadRight(25) + fw.Responsible.Trim().PadRight(25) + fw.AparName.Trim().PadRight(255) + fw.Address.Trim().PadRight(160) + fw.Province.Trim().PadRight(40) + fw.Place.Trim().PadRight(40)
            //+ fw.BankAccount.Trim().PadRight(35) + fw.PayMethod + fw.VatRegNo.Trim().PadRight(25) + fw.ZipCode.Trim().PadRight(15) + fw.CurrLicence.Trim().PadRight(3) + fw.Account2.Trim().PadRight(25)
            //+ fw.BaseAmount.Trim().PadLeft(20) + fw.BaseCurr.Trim().PadLeft(20) + fw.PayTempId.Trim().PadRight(4) + fw.AllocationKey.Trim().PadRight(2) + fw.PeriodNo.Trim().PadRight(2) + fw.Clearingcode.Trim().PadRight(13)
            //+ fw.Swift.Trim().PadRight(11) + fw.Arriveid.Trim().PadRight(15) + fw.BankAccType.Trim().PadRight(2)
            // ;

        }
        public GL07()
        {
            String x = "we are here";
 

        }
        public GL07(String[] fields)
        {
            // Need to set out the GL07 schema here
            // may use a structure or a name value pairing
            //batch_id = fields[0];
            //myinterface = fields[1];
            //voucher_type = fields[2];
            //trans_type = fields[3];
            //client = fields[4];
            //account = fields[5];
            //dim_1 = fields[6];
            //dim_2 = fields[7];
            //dim_3 = fields[8];
 
            //dim_4 = fields[9];
            //dim_5 = fields[10];
            //dim_6 = fields[11];
            //dim_7 = fields[12];
            //tax_code = fields[13];
            //tax_system = fields[14];
            //currency = fields[15];
            //dc_flag = fields[16];
            //cur_amount = fields[17];
            //amount = fields[18];

            //number_1 = fields[19];
            //value_1 = fields[20];
            //value_2 = fields[21];
            //value_3 = fields[22];
            //description = fields[23];
            //trans_date = fields[24];
            //voucher_date = fields[25];
            //voucher_no = fields[26];
            //period = fields[27];
            //tax_flag = fields[28];
            //ext_inv_ref = fields[29];
            //ext_ref = fields[30];

            //due_date = fields[31];
            //disc_date = fields[32];
            //discount = fields[33];
            //commitment = fields[34];
            //order_id = fields[35];
            //kid = fields[36];
            //pay_transfer = fields[37];
            //status = fields[38];
            //apar_type = fields[39];
            //apar_id = fields[40];

            //pay_flag = fields[41];
            //voucher_ref = fields[42];
            //sequence_ref = fields[43];
            //intrule_id = fields[44];
            //factor_short = fields[45];
            //responsible = fields[46];
            //apar_name = fields[47];
            //address = fields[48];
            //province = fields[49];
            //place = fields[50];

            //bank_account = fields[51];
            //pay_method = fields[52];
            //vat_reg_no = fields[53];
            //zip_code = fields[54];
            //curr_licence = fields[55];
            //account2 = fields[56];
            //base_amount = fields[57];
            //base_curr = fields[58];
            //pay_temp_id = fields[59];
            //allocation_key = fields[60];

            //period_no = fields[61];
            //clearing_code = fields[62];
            //swift = fields[63];
            //arrive_id = fields[64];
            //bank_acc_type = fields[65];

        }

        public DataView readDataView(String aCSName, String aSqlQuery)
        {
            // ConnectionStrings are in the machine.config of the .net4 x86 framework
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
