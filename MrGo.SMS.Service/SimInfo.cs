
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Net;
using Android.Database;

namespace MrGo.SMS.Service
{
    public class ColumnVal
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
    public class SimInfo
    {
        public string _id;
        public string icc_id;
        public string sim_id;
        public string display_name;
        public string name_source;
        public string color;
        public string number;
        public string display_number_format;
        public string data_roaming;
        public string mcc;
        public string mnc;

        public SimInfo()
        {

        }

      
        public override string ToString()
        {
            return "SimInfo{" +
                    "id_=" + _id +
                    ", display_name='" + display_name + '\'' +
                    ", icc_id='" + icc_id + '\'' +
                    ", slot=" + sim_id +
                    '}';
        }
        public static List<SimInfo> getSIMInfo(Context context)
        {
            List<SimInfo> simInfoList = new List<SimInfo>();
            Uri URI_TELEPHONY = Uri.Parse("content://telephony/siminfo/");
            CursorWrapper c = (CursorWrapper)context.ContentResolver.Query(URI_TELEPHONY, null, null, null, null);
            string[] colnames = c.GetColumnNames();
            if (c.MoveToFirst())
            {
                do
                {
                    SimInfo simInfo = new SimInfo();
                    simInfo.color = c.GetString(c.GetColumnIndex("icc_id"));
                    simInfo.data_roaming = c.GetString(c.GetColumnIndex("icc_id"));
                    simInfo.display_name = c.GetString(c.GetColumnIndex("icc_id"));
                    simInfo.display_number_format = c.GetString(c.GetColumnIndex("icc_id"));
                    simInfo.icc_id = c.GetString(c.GetColumnIndex("icc_id"));
                    simInfo.mcc = c.GetString(c.GetColumnIndex("mcc"));
                    simInfo.mnc = c.GetString(c.GetColumnIndex("mnc"));
                    simInfo.name_source = c.GetString(c.GetColumnIndex("name_source"));
                    simInfo.number = c.GetString(c.GetColumnIndex("number"));
                    simInfo.sim_id = c.GetString(c.GetColumnIndex("sim_id"));
                    simInfo._id = c.GetString(c.GetColumnIndex("_id"));
                    simInfoList.Add(simInfo);
                } while (c.MoveToNext());
            }
            c.Close();

            return simInfoList;
        }
    }
}