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
    public interface IBackGroundResult
    {
        void SetBackGroundResult(string key, Object result);
        Context Context { get; }
    }
}