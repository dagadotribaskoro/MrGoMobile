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
using Java.IO;

namespace MrGo.Entity
{
    public class RestoCategoty : Java.Lang.Object, ISerializable
    {
        public int restocategory_id { get; set; }//0
        public string restocategory_code { get; set; }//1
        public string restocategory_name { get; set; }//2
        public string restocategory_note { get; set; }//2

        internal static string GetAllCategory()
        {
            return "select * from restocategory";
        }
        public static List<RestoCategoty> GetListByServerResponse(string response)
        {
            if (response == "") return null;
            List<RestoCategoty> members = new List<RestoCategoty>();
            string[] lines = response.Split(new string[] { "<BR>" }, StringSplitOptions.None);
            foreach (string line in lines)
            {
                if (line.Trim() == "") continue;
                string[] datas = line.Split(';');
                RestoCategoty m = new RestoCategoty();
                m.restocategory_id = Convert.ToInt32(datas[0].Trim());
                m.restocategory_code = datas[1];
                m.restocategory_name = datas[2];
                m.restocategory_note = datas[3];
                members.Add(m);
            }
            return members;
        }

    }
        public class Resto : Java.Lang.Object, ISerializable
    {
        public int resto_id { get; set; }//0
        public string resto_code { get; set; }//1
        public string resto_name { get; set; }//2

      

        public string resto_note { get; set; }//3
        public string resto_address { get; set; }//4
        public string resto_latitude { get; set; }//5
        public string resto_longitude { get; set; }//6
        public string resto_url_image { get; set; }//7
        public string resto_image_file { get; set; }//8
        public decimal resto_jumlah_ongkir { get; set; }//7

        public Resto()
        {
            resto_jumlah_ongkir = 10000;//default sementara
        }
        public static List<Resto> GetListByServerResponse(string response)
        {
            if (response == "") return null;
            List<Resto> members = new List<Resto>();
            string[] lines = response.Split(new string[] { "<BR>" }, StringSplitOptions.None);
            foreach (string line in lines)
            {
                if (line.Trim() == "") continue;
                string[] datas = line.Split(';');
                Resto m = new Resto();
                m.resto_id = Convert.ToInt32(datas[0].Trim());
                m.resto_code = datas[1];
                m.resto_name = datas[2];
                m.resto_note = datas[3];
                m.resto_address = datas[4];
                m.resto_latitude = datas[5];
                m.resto_longitude = datas[6];
                m.resto_url_image = datas[7];
                m.resto_image_file = datas[8];
                members.Add(m);
            }
            return members;
        }

      

        public static string GetAllSQL()
        {
            return "SELECT * FROM resto";
        }
        internal static string GetAllTopTenSQL()
        {
            return "SELECT * FROM resto where resto_top_ten <> '' order by resto_top_ten";
        }
        public static string GetRestoByID(string resto_id)
        {
            return "SELECT * FROM resto where resto_id="+resto_id;
        }
        public static string GetAllSearchSQL(string key)
        {
            return string.Format(@"Select * from resto where concat(resto_code
           ,resto_name
           ,resto_note
           ,resto_address
          ) like '%{0}%'", key);
        }

        internal static string GetAllByCategory(string v)
        {
            return "select * from resto where restocategory_id =" + v;
        }
    }
}