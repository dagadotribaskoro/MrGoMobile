using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MrGo.Entity;
using MrGo.Activities;
using Java.Net;
using System.IO;
using Java.IO;
using Android.Support.V4.App;

namespace MrGo.Service
{
    public class MenuRestoService : AsyncTask
    {
        string sqlquery_url = "http://mrgoapps.hol.es/mobileserver/sqlquery.php";
        string sqlnonquery_url = "http://mrgoapps.hol.es/mobileserver/sqlnonquery.php";
        IBackGroundResult activity;
        public Object m_result;
        string key = "";

        public MenuRestoService(IBackGroundResult parent)
        {
            activity = (IBackGroundResult)parent;
        }
        protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] @params)
        {
            if (!CommonService.CheckInternetConnection(activity.Context))
                return null;
            key = @params[0].ToString();
            URL url = new URL(sqlquery_url);
            string query = "";
            if (key == "GetMenuByRestoID")
                query = MenuResto.GetByRestoID(@params[1].ToString());
            if (key == "GetMenuMakananByRestoID")
                query = MenuResto.GetMenuMakananByRestoID(@params[1].ToString());
            if (key == "GetMenuMinumanByRestoID")
                query = MenuResto.GetMenuMinumanByRestoID(@params[1].ToString());
            if (key == "GetMenuSpesialByRestoID")
                query = MenuResto.GetMenuSpesialByRestoID(@params[1].ToString());
            if (key == "GetMenuByIDInSelect")
                query = MenuResto.GetMenuByIDInSelect(@params[1].ToString());
            string data = URLEncoder.Encode("query", "UTF-8") + "=" + URLEncoder.Encode(query, "UTF-8");
            HttpURLConnection urlConn = (HttpURLConnection)url.OpenConnection();
            urlConn.RequestMethod = "POST";
            urlConn.DoInput = true;
            urlConn.DoOutput = true;
            try
            {
                Stream oStream = urlConn.OutputStream;
                BufferedWriter bw = new BufferedWriter(new OutputStreamWriter(oStream, "UTF-8"));
                bw.Write(data);
                bw.Flush();
                bw.Close();
                oStream.Close();
                Stream iStream = urlConn.InputStream;
                BufferedReader br = new BufferedReader(new InputStreamReader(iStream));
                System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
                string line = "";
                while ((line = br.ReadLine()) != null)
                {
                    stringBuilder.Append(line + "\n");
                }
                urlConn.Disconnect();
                string result = stringBuilder.ToString().Trim();
                m_result = MenuResto.GetListByServerResponse(result);
                return result;
            }
            catch (Java.IO.IOException ex)
            {
                //   Toast.MakeText(activity.GetContext(), ex.Message, ToastLength.Short);
            }
            return null;
        }
        protected override void OnPostExecute(Java.Lang.Object result)
        {
            activity.SetBackGroundResult(key, m_result);
        }
    }
}