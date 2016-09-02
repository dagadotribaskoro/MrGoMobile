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

namespace MrGo
{
    [Activity(Label = "Welcome User")]
    public class WelcomeUserXX : Activity, ISetMemberActivity
    {
        TextView tvWelcome;
        Member mCurrentMember;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.WelcomeUser);
            tvWelcome = (TextView)FindViewById(Resource.Id.txtUserName);
            string memberId =  Intent.GetStringExtra("memberId");
            Service.MemberService backGroundTask = new Service.MemberService(this);
            backGroundTask.Execute("getbyid", memberId);
            
        }
        public void SetMemberActivity(string key, Member member)
        {
            mCurrentMember = member;
            tvWelcome.Text = "Welcome " + mCurrentMember.member_name + "!...";
        }
        public Context GetContext()
        {
            return this;
        }
    }
}