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
using System.IO;

namespace MrGo.SMS.Services
{
    public class Member 
    {
        public int member_id { get; set; }//0
        public string member_code { get; set; }//1
        public string member_address { get; set; }//2
        public string member_phone { get; set; }//4
        public string member_email { get; set; }//12
        public string member_password { get; set; }//13
        public string member_name { get; set; }//10
        public string member_status { get; set; }//9
        public decimal member_balance { get; set; }//9
        public string member_activationcode { get; set; }//14
        public int member_sms_sent { get; set; }


        public static string GetMemberByEmailPasswordSQL(string email, string password)
        {
            //select * from user_table where email = 'test' and password = '1234';
            return "select * from member where member_email = '" + email + "' and member_password = '" + password + "';";
        }
        public static string GetAllSQL()
        {
            return "SELECT * FROM member m;";
        }
        public static string GetMemberByIdSQL(int id)
        {
            return "SELECT * FROM member m where member_id = " + id; 
        }
        public static string GetMemberByEmail(string email)
        {
            return "select * from member where member_email = '" + email + "'";
        }
        public static string GetMemberByPendingSMS()
        {
            return "select * from member where member_sms_sent = 0";
        }
        public static string GetActivateUserSQL(string email, string member_activationcode)
        {
            return @"update member set member_status='Aktif' where member_email = '" + email + "' and member_activationcode='"+ member_activationcode + "'";
        }
        public static string GetInsertSQL(string member_name, string member_phone, string member_email
            , string member_password, string member_activationcode)
        {
            return @"INSERT INTO member
           (member_code
           ,member_name
           ,member_phone
           ,member_joindate
           ,member_saving
            ,member_withdrawl
           ,member_balance
           ,member_status
            ,member_email
            ,member_password
            ,member_address
            ,member_dob
            ,member_pob
            ,member_activationcode)
     VALUES
           ('" + member_email + @"'
           ,'" + member_name + @"'
           ,'" + member_phone + @"'
           ,'" + DateTime.Today.ToString("yyyy-MM-dd") + @"'
           ,0
            ,0
           ,0
           ,'TidakAktif'
            ,'" + member_email + @"'
            ,'" + member_password + @"'
            ,''
            ,'" + DateTime.Today.ToString("yyyy-MM-dd") + @"'
            ,'','"+ member_activationcode + "')";
        }

        internal static string UpdateSMSStatusSQL(string menus)
        {
            return "Update member set member_sms_sent = 1 where member_id in (" + menus + ")";
        }

        public static Member GetByServerResponse(string response)
        {
            if (response == "") return null;
            Member m = new Member();
            string[] lines = response.Split(new string[] { "<BR>" }, StringSplitOptions.None);
            foreach (string line in lines)
            {
                if (line.Trim() == "") continue;
                string[] datas = line.Split(';');
                if (datas.Length < 3) continue;
                m.member_id = Convert.ToInt32(datas[0].Trim());
                m.member_code = datas[1];
                m.member_address = datas[2];
                m.member_phone = datas[4];
                m.member_password = datas[13];
                m.member_email = datas[12];
                m.member_name = datas[10];
                m.member_status = datas[9];
                m.member_balance = Convert.ToDecimal(datas[8]);
                m.member_activationcode = datas[14];
                m.member_sms_sent = Convert.ToInt32(datas[15].Trim());
            }
            return m;
        }
        public static List<Member> GetByListServerResponse(string response)
        {
            List<Member> members = new List<Member>();
            if (response == "") return members;
            string[] lines = response.Split(new string[] { "<BR>" }, StringSplitOptions.None);
            foreach (string line in lines)
            {
                if (line.Trim() == "") continue;
                string[] datas = line.Split(';');
                Member m = new Member();
                m.member_id = Convert.ToInt32(datas[0].Trim());
                m.member_code = datas[1];
                m.member_address = datas[2];
                m.member_phone = datas[4];
                m.member_password = datas[13];
                m.member_email = datas[12];
                m.member_status = datas[9];
                m.member_name = datas[10];
                m.member_balance = Convert.ToDecimal(datas[8]);
                m.member_activationcode = datas[14];
                m.member_sms_sent = Convert.ToInt32(datas[15].Trim());
                members.Add(m);
            }
            return members;
        }       
    }
}