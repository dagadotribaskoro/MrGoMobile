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
using MrGo.Entity;

namespace MrGo.Service
{
    public class MemberService : AsyncTask
    {
        //string login_url = "http://192.168.1.5/mrgowebserver/commonquery.php";
        string sqlquery_url = "http://mrgoapps.hol.es/mobileserver/sqlquery.php";
        string sqlnonquery_url = "http://mrgoapps.hol.es/mobileserver/sqlnonquery.php";
        ISetMemberActivity activity;
        ProgressDialog progressDialog;
        public Member Member;
        string key = "";
        
        public MemberService(Context context)
        {
            activity = (ISetMemberActivity)context;
        }
        protected override void OnPreExecute()
        {
            progressDialog = new ProgressDialog(activity.GetContext());
            progressDialog.SetTitle("Please wait...");
            progressDialog.SetMessage("Connecting to server...");
            //progressDialog.SetIndeterminateDrawable(true);
            progressDialog.SetCancelable(false);
            progressDialog.Show();
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
                query = MrGo.Entity.Member.GetMemberByEmailPasswordSQL(@params[1].ToString(), @params[2].ToString());
            }
            if (key == "getbyid")
            {
                query = MrGo.Entity.Member.GetMemberByIdSQL(Convert.ToInt32(@params[1].ToString()));
            }
            if (key == "updateReSentCodeByEmail")
            {
                query = MrGo.Entity.Member.UpdateReSentCodeByEmail(@params[1].ToString());
            }
            if (key == "register")
            {
                url = new URL(sqlnonquery_url);
                query = MrGo.Entity.Member.GetInsertSQL(
                    @params[1].ToString()
                    , @params[2].ToString()
                    , @params[3].ToString()
                    , @params[4].ToString()
                    ,"1234"
                    );
            }
            if (key == "getbyemail")
            {
                query = MrGo.Entity.Member.GetMemberByEmail(@params[1].ToString());
            }
            if (key == "activateuser")
            {
                url = new URL(sqlnonquery_url);
                query = MrGo.Entity.Member.GetActivateUserSQL(@params[1].ToString(), @params[2].ToString());
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
                Member = Member.GetByServerResponse(result);
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
            progressDialog.Dismiss();
            activity.SetMemberActivity(key, Member);
        }

    }
}