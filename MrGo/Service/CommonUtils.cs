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

namespace MrGo.Service
{
    public class CommonUtils
    {
        public const string DATE_FORMAT_MYSQL = "yyyy-MM-dd HH:mm:ss";//2016-08-29 00:00:00
        public const string DATE_FORMAT_VIEW = "dd-MMM-yyyy";//2016-08-29 00:00:00
        public static DateTime NULL_DATE = new DateTime(2000,1,1);//2016-08-29 00:00:00
        public const string DECIMAL_FORMAT = "#,#.00#;(#,#.00#)";
    }
}