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
using MrGo.Entity;
using MrGo.Service;
using Android.Content.PM;

namespace MrGo
{
    [Activity(Label = "UserConfirmation", LaunchMode = LaunchMode.SingleTop, Icon = "@drawable/ic_launcher")]
    public class ConfirmationActivity : Activity, ISetMemberActivity
    {
        TextView tvPhone;
        Button btnNext,btnEditNumber,btnSendAgain;
        EditText etCode;
        //Member mCurrentMember;
        AlertDialog.Builder builder;
        string email, phone;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ConfirmationScreen);
            builder = new AlertDialog.Builder(this);
             email = Intent.GetStringExtra("memberEmail");
             phone = Intent.GetStringExtra("memberPhone");
            etCode = FindViewById<EditText>(Resource.Id.etConfirmationCode);
            tvPhone = FindViewById<TextView>(Resource.Id.tvPhone);
            tvPhone.Text = phone;

            //Service.MemberService backGroundTask = new Service.MemberService(this);
            //backGroundTask.Execute("getbyemail", email);


            btnNext = FindViewById<Button>(Resource.Id.btnNext);
            btnEditNumber = FindViewById<Button>(Resource.Id.btnEditNumber);
            btnSendAgain = FindViewById<Button>(Resource.Id.btnSendAgain);

            btnNext.Click += BtnNext_Click;
            btnEditNumber.Click += BtnEditNumber_Click;
            btnSendAgain.Click += BtnSendAgain_Click;
            // Create your application here
        }

        private void BtnEditNumber_Click(object sender, EventArgs e)
        {
           
        }

        private void BtnSendAgain_Click(object sender, EventArgs e)
        {
            MemberService svc = new MemberService(this);
            svc.Execute("updateReSentCodeByEmail", email);
            Toast.MakeText(this, "Code re-sent. Please wait for a while.", ToastLength.Short).Show();
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            if (etCode.Text != "")
            {
                Service.MemberService backGroundTask = new Service.MemberService(this);
                backGroundTask.Execute("activateuser", email, etCode.Text);
                //backGroundTask = new Service.MemberService(this);
                //backGroundTask.Execute("getbyemail", mCurrentMember.member_email);
            }
            else if (etCode.Text == "")
            {
                builder = new AlertDialog.Builder(this);
                builder.SetTitle("Information");
                builder.SetMessage("Please fill the code.");
                builder.SetPositiveButton("OK", OkAction);
                AlertDialog alert = builder.Create();
                alert.Show();
            }
            else
            {
                builder = new AlertDialog.Builder(this);
                builder.SetTitle("Information");
                builder.SetMessage("Please fill correct code.");
                builder.SetPositiveButton("OK", OkAction);
                AlertDialog alert = builder.Create();
                alert.Show();
            }
        }
        private void OkAction(object sender, DialogClickEventArgs e)
        {
           
        }

        Member m_member;
        public void SetMemberActivity(string key, Member member)
        {
            if (!CommonService.CheckInternetConnection(this)) { Toast.MakeText(this, "Please check your internet connection", ToastLength.Short).Show(); return; }
            if (key == "activateuser")
            {
                Service.MemberService backGroundTask = new Service.MemberService(this);
                backGroundTask = new Service.MemberService(this);
                backGroundTask.Execute("getbyemail", email);
            }
            if (key == "getbyemail")
            {
                if (member == null) return;
                if (member.member_status == "Aktif")
                {
                    Toast.MakeText(this, "Your account is activated, Please enjoy.", ToastLength.Short).Show();
                    Intent i = new Intent();
                    i.PutExtra("member_id", member.member_id.ToString());
                    SetResult(Result.Ok, i);
                    Finish();
                }
                else
                {
                    m_member = member;
                    builder = new AlertDialog.Builder(this);
                    builder.SetTitle("Information");
                    builder.SetMessage("Please fill correct code. ");
                    builder.SetPositiveButton("OK", OkAction);
                    AlertDialog alert = builder.Create();
                    alert.Show();
                }
            }
        }
        public Context GetContext()
        {
            return this;
        }
    }
}