using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Telephony;
using System.Timers;
using MrGo.SMS.Services;

namespace MrGo.SMS.Service
{
   // [Activity(Label = "MrGo SMS Service", MainLauncher = true, Icon = "@drawable/icoMrGoSMSService")]
    public class MainActivity : Activity, ISetMemberActivity
    {
        Timer m_timer;
        Button buttonStart, buttonStop;
        TextView textViewStatus, textViewMessage;
        bool isstart = false;
        //List<SimInfo> sims;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            m_timer = new Timer(3000);
            m_timer.Elapsed += M_timer_Elapsed;

            buttonStart = FindViewById<Button>(Resource.Id.buttonStart);
            buttonStop = FindViewById<Button>(Resource.Id.buttonStop);
            textViewStatus = FindViewById<TextView>(Resource.Id.textViewStatus);
            textViewMessage = FindViewById<TextView>(Resource.Id.textViewMessage);
            buttonStart.Click += ButtonStart_Click;
            buttonStop.Click += ButtonStop_Click;
            // sims = SimInfo.getSIMInfo(this);
            // TelephonyManager mTelephonyMgr = (TelephonyManager)GetSystemService(TelephonyService);
            // string Number = mTelephonyMgr.Line1Number;
        }

        private void ButtonStart_Click(object sender, EventArgs e)
        {
            if (!isstart)
            {
                if (!CommonService.CheckInternetConnection(this))
                {
                    Toast.MakeText(this, "Please check your internet connection", ToastLength.Short).Show();
                    return;
                }
                textViewStatus.Text = "Service is running...";
                m_timer.Start();
                isstart = true;
            }
        }

        private void ButtonStop_Click(object sender, EventArgs e)
        {
            if (isstart)
            {
                textViewStatus.Text = "Service is stop...";
                m_timer.Stop();
                isstart = false;
            }
        }

        private void M_timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (isstart)
            {
               SendSMSToMember();
            }
        }

        private void SendSMSToMember()
        {
            MemberService svc = new MemberService(this);
            svc.Execute("GetMemberByPendingSMS");
        }
        public void SetMemberActivity(string key, Object members)
        {
            if (!CommonService.CheckInternetConnection(this))
                return;

            if (members != null)
            {
                List<Member> listMember = (List<Member>)members;
                int count = 0;
                string menuIds = "";
                textViewMessage.Text = DateTime.Now.ToString() + "= Sending SMS : " + listMember.Count;
                foreach (Member mbr in listMember)
                {
                    try
                    {
                        SmsManager smsMgr = SmsManager.Default;
                        smsMgr.SendTextMessage(mbr.member_phone, null, "Your MrGo code is " + mbr.member_activationcode + ". Enjoy!", null, null);
                        if (count == 0)
                            menuIds = mbr.member_id.ToString();
                        else
                            menuIds += ("," + mbr.member_id.ToString());
                        count++;
                    }
                    catch (Java.Lang.IllegalArgumentException x)
                    {
                        textViewMessage.Text = DateTime.Now.ToString()+"= Sending SMS : failed " + x.Message;

                        return;
                    }
                }
                MemberService svc = new MemberService(this);
                svc.Execute("UpdateSMSStatus", menuIds);
            }
            //if (key == "UpdateSMSStatus")
              //  jobstarted = false;
            //if (members != null)
            //{
            //    if(members.ToString() =="")
            //    m_timer.Start();
            //}
        }

        public Context GetContext()
        {
            return this;
        }
    }
}

