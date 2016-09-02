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
    public class TransactionService : AsyncTask
    {
        string sqlquery_url = "http://mrgoapps.hol.es/mobileserver/sqlquery.php";
        string sqlnonquery_url = "http://mrgoapps.hol.es/mobileserver/sqlnonquery.php";
        IBackGroundResult activity;
        public Object m_result;
        string key = "";

        public TransactionService(IBackGroundResult parent)
        {
            activity = (IBackGroundResult)parent;
        }
        public void InsertTransaction(Transaction tr)
        {

        }

        protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] @params)
        {
            if (!CommonService.CheckInternetConnection(activity.Context))
                return null;
            key = @params[0].ToString();
            URL url = new URL(sqlquery_url);
            string query = "";
            if (key == "InsertTransaction")
            {
                url = new URL(sqlnonquery_url);
                Transaction tr = (Transaction)@params[1];
                query = MrGo.Entity.Transaction.GetInsertSQL(tr);
            }
            if (key == "getmaxidbymember")
            {
                query = MrGo.Entity.Transaction.SelectMaxTransactionId(Convert.ToInt32(@params[1].ToString()));
            }
            if (key == "getmaxid")
            {
                query = MrGo.Entity.Transaction.SelectMaxTransactionId();
            }
            if (key == "InsertDetailsTransaction")
            {
                url = new URL(sqlnonquery_url);
                TransactionDetail tr = (TransactionDetail)@params[1];
                query = MrGo.Entity.TransactionDetail.GetInsertSQL(tr);
            }
            if (key == "getByMemberByStatus")
            {
                TransactionStatus trSts = (TransactionStatus)Enum.Parse(typeof(TransactionStatus), @params[1].ToString());
                int trid = Convert.ToInt32(@params[2].ToString());
                query = MrGo.Entity.Transaction.GetByMemberByStatusSQLSQL(trid, trSts);
            }
            if(key == "getByID")
            {
                int trId = Convert.ToInt32(@params[1].ToString());
                query = MrGo.Entity.Transaction.GetByIDSQL(trId);
            }
            if (key == "getTrDetailByTrId")
            {
                int trId = Convert.ToInt32(@params[1].ToString());
                query = MrGo.Entity.TransactionDetail.GetTrDetailByTrIdSQL(trId);
            }
            if (key == "updateStatus")
            {
                url = new URL(sqlnonquery_url);
                TransactionStatus trSts = (TransactionStatus)Enum.Parse(typeof(TransactionStatus), @params[1].ToString());
                int trId = Convert.ToInt32(@params[2].ToString());
                query = MrGo.Entity.Transaction.UpDateStatusSQL(trId, trSts);
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
                if (key == "getTrDetailByTrId")
                    m_result = TransactionDetail.GetListByServerResponse(result);
                else
                    m_result = Transaction.GetListByServerResponse(result);
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