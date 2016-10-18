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
using Android.Net;

namespace MrGo.Service
{
    public class CommonService
    {
        public static bool CheckInternetConnection(Context ctx)
        {
            if (ctx == null) return true;
            ConnectivityManager cm = (ConnectivityManager)ctx.GetSystemService(Context.ConnectivityService);
            NetworkInfo[] networkInfo = cm.GetAllNetworkInfo();
            bool connectedWifi = false;
            bool connectedMobile = false;
            foreach (NetworkInfo ni in networkInfo)
            {
                if (ni.TypeName.ToLower().Equals("wifi"))
                { connectedWifi = ni.IsConnected; }
                if (ni.TypeName.ToLower().Equals("mobile"))
                { connectedMobile = ni.IsConnected; }
            }

            bool result = connectedWifi || connectedMobile;
          //  if (!result)
            //    Toast.MakeText(ctx, "Please check your internet connection", ToastLength.Short).Show();
            return result;
        }
    }
}