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
using MrGo.Service;
using Android.Content.PM;

namespace MrGo
{
    [Activity(Label = "MrGo", LaunchMode = LaunchMode.SingleTop, Icon = "@drawable/ic_launcher")]
    //[Activity(Label = "MrGo", MainLauncher = true, Icon = "@drawable/icon", Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen")]
    public class StartScreenActivity : Activity
    {
        Button btnSignIn, btnSignUp;
        TextView tvTerms;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.StartScreen);
            btnSignIn = FindViewById<Button>(Resource.Id.btnSignIn);
            btnSignUp = FindViewById<Button>(Resource.Id.btnSignUp);
            tvTerms = FindViewById<TextView>(Resource.Id.tvTerms);

            btnSignIn.Click += BtnSignIn_Click;
            btnSignUp.Click += BtnSignUp_Click;
            tvTerms.Click += TvTerms_Click;
        }

        private void TvTerms_Click(object sender, EventArgs e)
        {
            Toast.MakeText(this, "Under Construction, please wait for our update.", ToastLength.Short).Show();
        }

        private void BtnSignUp_Click(object sender, EventArgs e)
        {
                StartActivityForResult(new Intent(this, typeof(SignUpActivity)), 1);
        }

        private void BtnSignIn_Click(object sender, EventArgs e)
        {
             StartActivityForResult(new Intent(this, typeof(SignInActivity)), 1);
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (data != null)
            {
                var memberIdstr = data.GetStringExtra("member_id");
                if (memberIdstr != null)
                {
                    Intent i = new Intent();
                    i.PutExtra("member_id", memberIdstr.ToString());
                    SetResult(Result.Ok, i);
                    Finish();
                }
            }
        }
    }
}