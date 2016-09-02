using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MrGo.Service;

namespace MrGo.Entity
{
    public enum TransactionStatus { Waiting,OnProgress,Cancel,Done}
    public class Transaction : Java.Lang.Object
    {
        public int transaction_id = 0;
        public string name = "";
        public string transaction_code = "";
        public string transaction_note = "";
        public string member_status = "Member";
        public int member_id = 0;
        public string transaction_type = "Beli Makanan";//ddl
        public string address = "";
        public string phone = "";
        public string pickup_address = "";
        public string pickup_area = "";
        public string destination_address = "";
        //public string distance = "";//ddl pickup_area
        public string destination_area = "";//ddl
        public string item_type = "";
        public int employee_id = 0;
        public decimal charge = 0;
        public TransactionStatus transaction_status =  TransactionStatus.Waiting;
        public DateTime transaction_date = DateTime.Now;
        //----------------MrG0
        public int resto_id = 0;
        public decimal total_buy = 0;
        public decimal total_all = 0;
        public List<TransactionDetail> Items = new List<TransactionDetail>();

        public string resto_url_image = "";
        public string resto_name = "";

        public static string SelectMaxTransactionId(int trid)
        {
            return "Select max(transaction_id) from transaction where member_id=" + trid;
        }
        public static string SelectMaxTransactionId()
        {
            return "Select max(transaction_id) from transaction";
        }
        public static string GetByMemberByStatusSQLSQL(int member_id, TransactionStatus sts)
        {
            return @"SELECT tr.*,rt.resto_name, rt.resto_url_image FROM transaction tr, 
                resto rt WHERE tr.resto_id = rt.resto_id and member_id =" + member_id 
                + " and transaction_status ='" +sts.ToString()+ "' order by transaction_id desc";
        }

        internal static string UpDateStatusSQL(int trId, TransactionStatus sts)
        {
            return @"update transaction set transaction_status='" + sts.ToString() + "' where transaction_id =" + trId;
        }
        public static string GetInsertSQL(Transaction tr)
        {
            return @"INSERT INTO transaction
           (member_status
           ,member_id
           ,transaction_type
           ,name
           ,address
           ,phone
           ,pickup_address
           ,destination_address
            ,pickup_area
           ,destination_area
           ,item_type
            , employee_id
            ,charge
            ,transaction_status
            ,transaction_note
            ,transaction_code
            ,transaction_date
            ,resto_id
            ,total_buy
            ,total_all
            )
     VALUES
            ('"+ tr.member_status + @"'
            ,'"+ tr.member_id + @"'
            ,'"+ tr.transaction_type + @"'
            ,'"+ tr.name + @"'
            ,'"+ tr.address + @"'
            ,'"+ tr.phone + @"'
            ,'"+ tr.pickup_address + @"'
            ,'"+ tr.destination_address + @"'
            ,'"+ tr.pickup_area + @"'
            ,'"+ tr.destination_area + @"'
            ,'"+ tr.item_type + @"'
            ,'"+ tr.employee_id + @"'
            ,'"+ tr.charge + @"'
            ,'"+ tr.transaction_status.ToString() + @"'
            ,'"+ tr.transaction_note + @"'
            ,'"+ tr.transaction_code + @"'
            ,'"+ tr.transaction_date.ToString(CommonUtils.DATE_FORMAT_MYSQL) + @"'
            ,'"+ tr.resto_id + @"'
            ,'"+ tr.total_buy + @"'
            ,'"+ tr.total_all + @"')";
        }

        internal static string GetByIDSQL(int trId)
        {
            return "Select * from transaction where transaction_id=" + trId;
        }

        public static List<Transaction> GetListByServerResponse(string response)
        {
            if (response == "") return null;
            List<Transaction> members = new List<Transaction>();
            string[] lines = response.Split(new string[] { "<BR>" }, StringSplitOptions.None);
            foreach (string line in lines)
            {
                if (line.Trim() == "") continue;
                string[] datas = line.Split(';');
                Transaction m = new Transaction();
                m.transaction_id = Convert.ToInt32(datas[0].Trim());
                m.destination_address = datas[8];
                m.transaction_code = datas[16];
                m.transaction_date = datas[17] == "" ? CommonUtils.NULL_DATE : Convert.ToDateTime(datas[17]);
                m.transaction_status = datas[14] == "" ? TransactionStatus.Waiting : (TransactionStatus)Enum.Parse(typeof(TransactionStatus), datas[14]);
                m.resto_name = datas[22];
                m.resto_url_image = datas[23];
                m.total_all = datas[20] == "" ? 0 : Convert.ToDecimal(datas[20]);
                m.charge = datas[13] == "" ? 0 : Convert.ToDecimal(datas[13]);
                m.total_buy = datas[19] == "" ? 0 : Convert.ToDecimal(datas[19]);
                //m.resto_longitude = datas[6];
                //m.resto_image_file = datas[8];
                members.Add(m);
            }
            return members;
        }
    }
    public class TransactionDetail : Java.Lang.Object
    {
        public int trdetail_id = 0;
        public int transaction_id = 0;
        public int menu_id = 0;
        public string menu_name = "";
        public int tr_unit = 0;
        public decimal tr_unit_price = 0;
        decimal tr_total_price = 0;

        public TransactionDetail()
        {

        }
        public decimal Tr_total_price
        {
            get { return tr_unit * tr_unit_price; }
        }
        public static List<TransactionDetail> GetListByServerResponse(string response)
        {
            if (response == "") return null;
            List<TransactionDetail> members = new List<TransactionDetail>();
            string[] lines = response.Split(new string[] { "<BR>" }, StringSplitOptions.None);
            foreach (string line in lines)
            {
                if (line.Trim() == "") continue;
                string[] datas = line.Split(';');
                TransactionDetail m = new TransactionDetail();
                m.trdetail_id = Convert.ToInt32(datas[0].Trim());
                m.transaction_id = Convert.ToInt32(datas[1].Trim());
                m.menu_id = Convert.ToInt32(datas[2].Trim());
                m.menu_name = datas[3];
                m.tr_unit = Convert.ToInt32(datas[4].Trim());
                m.tr_unit_price = Convert.ToDecimal(datas[5].Trim());
                m.tr_total_price = Convert.ToDecimal(datas[6].Trim());
                members.Add(m);
            }
            return members;
        }
        public static string GetInsertSQL(TransactionDetail tr)
        {
            return @"INSERT INTO transactiondetail    
           (transaction_id
           ,menu_id
           ,menu_name
           ,tr_unit
           ,tr_unit_price
           ,tr_total_price       
            )
     VALUES
           ('"+ tr.transaction_id + @"'
           ,'"+ tr.menu_id + @"'
           ,'"+ tr.menu_name + @"'
           ,'"+ tr.tr_unit + @"'
           ,'"+ tr.tr_unit_price + @"'
           ,'"+ tr.Tr_total_price + @"')";
        }

        internal static string GetTrDetailByTrIdSQL(int trId)
        {
            return "select * from transactiondetail where transaction_id=" + trId;
        }

    }
}