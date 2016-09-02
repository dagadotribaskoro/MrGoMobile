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
    public class MenuResto : Java.Lang.Object, ISerializable
    {
        public int menu_id { get; set; }//0
        public string menu_code { get; set; }//1
        public string menu_name { get; set; }//2
        public string menu_note { get; set; }//3
        public int resto_id { get; set; }//4
        public string menu_foodtype { get; set; }//5
        public decimal menu_price { get; set; }//6
        public string menu_url_image { get; set; }//7
        public int menu_jumlah_pesan{ get; set; }//7

        public MenuResto()
        {
           
        }
        public static List<MenuResto> GetListByServerResponse(string response)
        {
            if (response == "") return null;
            List<MenuResto> members = new List<MenuResto>();
            string[] lines = response.Split(new string[] { "<BR>" }, StringSplitOptions.None);
            foreach (string line in lines)
            {
                if (line.Trim() == "") continue;
                string[] datas = line.Split(';');
                MenuResto m = new MenuResto();
                m.menu_id = Convert.ToInt32(datas[0].Trim());
                m.menu_code = datas[1];
                m.menu_name = datas[2];
                m.menu_note = datas[3];
                m.resto_id = Convert.ToInt32(datas[4].Trim());
                m.menu_foodtype = datas[5];
                m.menu_price = Convert.ToDecimal(datas[6].Trim());
                m.menu_url_image = datas[7];
                members.Add(m);
            }
            return members;
        }
        public static string GetByRestoID(string resto_id)
        {
            return "SELECT * FROM menu where resto_id=" + resto_id;
        }
        public static string GetMenuMakananByRestoID(string resto_id)
        {
            return "SELECT * FROM menu where menu_foodtype = 'MAKANAN' and resto_id=" + resto_id;
        }
        public static string GetMenuMinumanByRestoID(string resto_id)
        {
            return "SELECT * FROM menu where menu_foodtype = 'MINUMAN' and resto_id=" + resto_id;
        }
        public static string GetMenuSpesialByRestoID(string resto_id)
        {
            return "SELECT * FROM menu where menu_foodtype = 'SPESIAL' and resto_id =" + resto_id+ " limit 3";
        }
        public static string GetMenuByIDInSelect(string menu_idInSelect)
        {
            return "SELECT * FROM menu where menu_id in (" + menu_idInSelect + ")";
        }
    }

}