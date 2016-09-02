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
using Java.Lang;
using Java.Net;
using System.IO;
using Java.IO;
using Org.Json;

namespace MrGo.SMS.Services
{
    public class MemberServiceNoSync
    {
        string sqlquery_url = "http://mrgoapps.hol.es/mobileserver/sqlquery.php";
        string sqlnonquery_url = "http://mrgoapps.hol.es/mobileserver/sqlnonquery.php";
         Context activity;
        List<Member> m_result = new List<Member>();

        public List<Member> GetResult()
        {
            return m_result;
        }
        public MemberServiceNoSync(Context context)
        {
            activity = context;
        }
        public bool Query(string key, params Java.Lang.Object[] @params)
        {
            if (!CommonService.CheckInternetConnection(activity))
                return false;
            URL url = new URL(sqlquery_url);
            string query = "";
            if (key == "login")
            {
                query = Member.GetMemberByEmailPasswordSQL(@params[0].ToString(), @params[1].ToString());
            }
            if (key == "getbyid")
            {
                query = Member.GetMemberByIdSQL(Convert.ToInt32(@params[0].ToString()));
            }
            if (key == "register")
            {
                url = new URL(sqlnonquery_url);
                query = Member.GetInsertSQL(
                    @params[0].ToString()
                    , @params[1].ToString()
                    , @params[2].ToString()
                    , @params[3].ToString()
                    , "1234"
                    );
            }
            if (key == "getbyemail")
            {
                query = Member.GetMemberByEmail(@params[0].ToString());
            }
            if (key == "activateuser")
            {
                url = new URL(sqlnonquery_url);
                query = Member.GetActivateUserSQL(@params[0].ToString(), @params[1].ToString());
            }
            if (key == "GetMemberByPendingSMS")
            {
                query = Member.GetMemberByPendingSMS();
            }
            if (key == "UpdateSMSStatus")
            {
                url = new URL(sqlnonquery_url);
                string menus = @params[0].ToString();
                query = Member.UpdateSMSStatusSQL(menus);
            }
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
                m_result = Member.GetByListServerResponse(result);
                return true;
            }
            catch (Java.IO.IOException ex)
            {
                //   Toast.MakeText(activity.GetContext(), ex.Message, ToastLength.Short);
            }
            return false;
        }
    }
    public class MemberService : AsyncTask
    {
        //string login_url = "http://192.168.1.5/mrgowebserver/commonquery.php";
        string sqlquery_url = "http://mrgoapps.hol.es/mobileserver/sqlquery.php";
        string sqlnonquery_url = "http://mrgoapps.hol.es/mobileserver/sqlnonquery.php";
        ISetMemberActivity activity;
        public List<Member> m_member = new List<Member>();
        string key = "";
        
        public MemberService(Context context)
        {
            activity = (ISetMemberActivity)context;
        }
        /// <summary>
        /// login,getbyid
        /// </summary>
        /// <param name="params"></param>
        /// <returns></returns>
        protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] @params)
        {
            if (!CommonService.CheckInternetConnection(activity.GetContext()))
                return null;
            key = @params[0].ToString();
            URL url = new URL(sqlquery_url);
            string query = "";
            if (key == "login")
            {
                query = Member.GetMemberByEmailPasswordSQL(@params[1].ToString(), @params[2].ToString());
            }
            if (key == "getbyid")
            {
                query = Member.GetMemberByIdSQL(Convert.ToInt32(@params[1].ToString()));
            }
            if (key == "register")
            {
                url = new URL(sqlnonquery_url);
                query = Member.GetInsertSQL(
                    @params[1].ToString()
                    , @params[2].ToString()
                    , @params[3].ToString()
                    , @params[4].ToString()
                    ,"1234"
                    );
            }
            if (key == "getbyemail")
            {
                query = Member.GetMemberByEmail(@params[1].ToString());
            }
            if (key == "activateuser")
            {
                url = new URL(sqlnonquery_url);
                query = Member.GetActivateUserSQL(@params[1].ToString(), @params[2].ToString());
            }
            if (key == "GetMemberByPendingSMS")
            {
                query = Member.GetMemberByPendingSMS();
            }
            if (key == "UpdateSMSStatus")
            {
                url = new URL(sqlnonquery_url);
                string menus = @params[1].ToString();
                query = Member.UpdateSMSStatusSQL(menus);
            }
            string data = URLEncoder.Encode("query", "UTF-8") + "=" + URLEncoder.Encode(query, "UTF-8");
            HttpURLConnection urlConn = (HttpURLConnection)url.OpenConnection();
            urlConn.RequestMethod = "POST";
            urlConn.DoInput = true;
            urlConn.DoOutput = true;
            try
            {
                Stream oStream = urlConn.OutputStream;

                BufferedWriter bw = new BufferedWriter(new OutputStreamWriter(oStream, "UTF-8"));
                //string query = MrGo.Entity.Member.GetMemberByEmailPasswordSQL(@params[1].ToString(), @params[2].ToString());
                
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
                m_member = Member.GetByListServerResponse(result);
                return result;
            }
            catch (Java.IO.IOException ex)
            {
             //   Toast.MakeText(activity.GetContext(), ex.Message, ToastLength.Short);
            }
            return null;
        }
        protected override void OnProgressUpdate(params Java.Lang.Object[] values)
        {
            //super.onProgressUpdate(values);
        }
        protected override void OnPostExecute(Java.Lang.Object result)
        {
            activity.SetMemberActivity(key, m_member);
        }

    }
}