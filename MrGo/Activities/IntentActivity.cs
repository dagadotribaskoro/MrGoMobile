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

namespace MrGo.Activities
{
    public class IntentActivity : Intent
    {
        Activity m_parentactivity;
        public IntentActivity(Context ctx, Type type): base(ctx, type)
        {
            m_parentactivity = (Activity)ctx;
        }
        public Activity GetParentActivity()
        {
            return m_parentactivity;
        }
        
    }
}