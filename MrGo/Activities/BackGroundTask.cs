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
using Org.Json;
using Java.Net;
using Java.IO;
using System.IO;

namespace MrGo
{
    class BackGroundTask : AsyncTask
    {
        string register_url = "http://192.168.1.5/latihanandroid/register.php";
        string login_url = "http://192.168.1.5/latihanandroid/login.php";
        Context ctx;
        Activity activity;
        AlertDialog.Builder builder;
        ProgressDialog progressDialog;
        public BackGroundTask(Context context)
        {
            ctx = context;
            activity = (Activity)ctx;
        }
        protected override void OnPreExecute()
        {
            builder = new AlertDialog.Builder(activity);
            progressDialog = new ProgressDialog(ctx);
            progressDialog.SetTitle("Please wait...");
            progressDialog.SetMessage("Connecting to server...");
            //progressDialog.SetIndeterminateDrawable(true);
            progressDialog.SetCancelable(false);
            progressDialog.Show();
        }

        protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] sparams)
        {
            string method = sparams[0].ToString();
            if (method.Equals("register"))
            {
                try
                {
                    URL url = new URL(register_url);
                    HttpURLConnection urlConn = (HttpURLConnection)url.OpenConnection();
                    urlConn.RequestMethod = "POST";
                    urlConn.DoInput = true;
                    urlConn.DoOutput = true;

                    Stream oStream = urlConn.OutputStream;
                    BufferedWriter bw = new BufferedWriter(new OutputStreamWriter(oStream, "UTF-8"));

                    string name = sparams[1].ToString();
                    string email = sparams[2].ToString();
                    string phone = sparams[3].ToString();
                    string address = sparams[4].ToString();
                    string password = sparams[5].ToString();

                    //    Log.e("LOG - NAME",name);
                    //Log.e("LOG - email",email);

                    string data = URLEncoder.Encode("name", "UTF-8") + "=" + URLEncoder.Encode(name, "UTF-8") + "&" +
                            URLEncoder.Encode("email", "UTF-8") + "=" + URLEncoder.Encode(email, "UTF-8") + "&" +
                            URLEncoder.Encode("phone", "UTF-8") + "=" + URLEncoder.Encode(phone, "UTF-8") + "&" +
                            URLEncoder.Encode("address", "UTF-8") + "=" + URLEncoder.Encode(address, "UTF-8") + "&" +
                            URLEncoder.Encode("password", "UTF-8") + "=" + URLEncoder.Encode(password, "UTF-8");

                    //Log.e("LOG","bw.write(data)");
                    bw.Write(data);
                    bw.Flush();
                    bw.Close();
                    oStream.Close();

                    Stream iStream = urlConn.InputStream;
                    BufferedReader br = new BufferedReader(new InputStreamReader(iStream));
                    System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
                    string line = "";
                    // Log.e("LOG","line=br.readLine(");

                    while ((line = br.ReadLine()) != null)
                    {
                        stringBuilder.Append(line + "\n");
                    }
                    urlConn.Disconnect();
                    // Log.e("LOG","result: "+stringBuilder.ToString().trim());
                    try
                    {
                        Thread.Sleep(3000);
                    }
                    catch (InterruptedException e)
                    {
                        // e.printStackTrace();
                    }
                    return stringBuilder.ToString().Trim();
                }
                catch (System.Exception x) { }
            }
            if (method.Equals("login"))
            {
                URL url = new URL(login_url);
                HttpURLConnection urlConn = (HttpURLConnection)url.OpenConnection();
                urlConn.RequestMethod = "POST";
                urlConn.DoInput = true;
                urlConn.DoOutput = true;

                Stream oStream = urlConn.OutputStream;
                BufferedWriter bw = new BufferedWriter(new OutputStreamWriter(oStream, "UTF-8"));

                string email = sparams[1].ToString();
                string password = sparams[2].ToString();

                string data = URLEncoder.Encode("email", "UTF-8") + "=" + URLEncoder.Encode(email, "UTF-8") + "&" +
                            URLEncoder.Encode("password", "UTF-8") + "=" + URLEncoder.Encode(password, "UTF-8");

                //Log.e("LOG","bw.write(data)");
                bw.Write(data);
                bw.Flush();
                bw.Close();
                oStream.Close();
                try
                {
                    Stream iStream = urlConn.InputStream;
                    BufferedReader br = new BufferedReader(new InputStreamReader(iStream));
                    System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
                    string line = "";
                    // Log.e("LOG","line=br.readLine(");

                    while ((line = br.ReadLine()) != null)
                    {
                        stringBuilder.Append(line + "\n");
                    }
                    urlConn.Disconnect();
                    // Log.e("LOG","result: "+stringBuilder.ToString().trim());
                    try
                    {
                        Thread.Sleep(3000);
                    }
                    catch (InterruptedException e)
                    {
                        // e.printStackTrace();
                    }
                    return stringBuilder.ToString().Trim();
                }
                catch (System.Exception x)
                {
                    Toast.MakeText(ctx, x.Message, ToastLength.Short);
                }
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
            if (result == null) return;
            string json = result.ToString();
            //ShowDialog("Server Response",json);
            //Log.e("LOG", "progressDialog.dismiss();");
            try
            {
                JSONObject jsonObject = new JSONObject(json);
                JSONArray jsonArray = jsonObject.GetJSONArray("server_response");
                JSONObject jo = jsonArray.GetJSONObject(0);
                string code = jo.GetString("code");
                string message = jo.GetString("message");
                // Log.e("LOG", "code: " + code);
                //Log.e("LOG", "message: " + message);
                if (code.Equals("reg_true"))
                {
                    ShowDialog("Registration Success", message, code);
                }
                else if (code.Equals("reg_false"))
                {
                    ShowDialog("Registration Failed", message, code);
                }
                else if (code.Equals("login_true"))
                {
                    Intent i = new Intent(activity, typeof(WelcomeUser));
                    i.PutExtra("message", message);
                    activity.StartActivity(i);
                }
                else if (code.Equals("login_false"))
                {
                    ShowDialog("Login Failed", message, code);
                }

            }
            catch (JSONException e)
            {
                // e.printStackTrace();
            }
        }
        public void ShowDialog(string title, string message, string code)
        {
            builder.SetTitle(title);
            if (code.Equals("reg_true") || code.Equals("reg_false"))
            {
                builder.SetMessage(message);
                builder.SetPositiveButton("OK", OkCorrectAction);
                builder.Create().Show();
            }
            else if (code.Equals("login_false"))
            {
                builder.SetMessage(message);
                builder.SetPositiveButton("OK", OkLognFalse);
                builder.Create().Show();
            }

        }

        private void OkCorrectAction(object sender, DialogClickEventArgs e)
        {
            //dialogInterface.dismiss();
            activity.Finish();
        }
        private void OkLognFalse(object sender, DialogClickEventArgs e)
        {

        }

    }
}