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
using Android.Util;
using System.Threading;
using MrGo.SMS.Services;
using Android.Telephony;

namespace MrGo.SMS.Service
{
    [Activity (Label = "MrGo SMS Service", MainLauncher = true, Icon = "@drawable/icoMrGoSMSService", LaunchMode = Android.Content.PM.LaunchMode.SingleInstance)]
    public class DemoActivity : Activity
    {
        bool isBound = false;
        bool isConfigurationChange = false;
        SMSServiceBinder binder;
        DemoServiceConnection demoServiceConnection;
        Button buttonStart, buttonStop;
        TextView textViewStatus, textViewMessage;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);
            textViewStatus = FindViewById<TextView>(Resource.Id.textViewStatus);
            textViewMessage = FindViewById<TextView>(Resource.Id.textViewMessage);
            buttonStart = FindViewById<Button>(Resource.Id.buttonStart);
            textViewMessage.Visibility = ViewStates.Gone;

            buttonStart.Click += delegate {
                Intent demoServiceIntent = new Intent("MrGo.SMS.Service");
                demoServiceConnection = new DemoServiceConnection(this);
                this.ApplicationContext.BindService(demoServiceIntent, demoServiceConnection, Bind.AutoCreate);
                StartService(new Intent("MrGo.SMS.Service"));
                textViewStatus.Text = "MrGO SMS Service has started";
            };

            buttonStop = FindViewById<Button>(Resource.Id.buttonStop);

            buttonStop.Click += delegate {
                //StopService (new Intent (this, typeof(DemoService)));
                binder.GetDemoService().startThread = false;
                StopService(new Intent("MrGo.SMS.Service"));
                textViewStatus.Text = "MrGO SMS Service has stoped";
            };

            //Button callService = FindViewById<Button>(Resource.Id.callService);

            //callService.Click += delegate {
            //    if (isBound)
            //    {
            //        RunOnUiThread(() => {
            //            string text = binder.GetDemoService().GetText();
            //            Console.WriteLine("{0} returned from DemoService", text);
            //        }
            //        );
            //    }
            //};

            // restore from connection there was a configuration change, such as a device rotation
            //demoServiceConnection = LastNonConfigurationInstance as DemoServiceConnection;

            //if (demoServiceConnection != null)
            //    binder = demoServiceConnection.Binder;
        }
        protected override void OnResume()
        {
            base.OnResume();
            if (isBound)
                textViewStatus.Text = "MrGO SMS Service has started";
        }
        protected override void OnStart()
        {
            base.OnStart();
          
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (!isConfigurationChange)
            {
                if (isBound)
                {
                    try
                    {
                        binder.GetDemoService().startThread = false;
                        isBound = false;
                        UnbindService(demoServiceConnection);
                    }
                    catch (Java.Lang.IllegalArgumentException x)
                    { }
                }
            }
        }

        // return the service connection if there is a configuration change
        public override Java.Lang.Object OnRetainNonConfigurationInstance()
        {
            base.OnRetainNonConfigurationInstance();
            isConfigurationChange = true;
            return demoServiceConnection;
        }

        class DemoServiceConnection : Java.Lang.Object, IServiceConnection
        {
            DemoActivity activity;
            SMSServiceBinder binder;
            public SMSServiceBinder Binder
            {
                get
                {
                    return binder;
                }
            }
            public DemoServiceConnection(DemoActivity activity)
            {
                this.activity = activity;
            }
            public void OnServiceConnected(ComponentName name, IBinder service)
            {
                var demoServiceBinder = service as SMSServiceBinder;
                if (demoServiceBinder != null)
                {
                    var binder = (SMSServiceBinder)service;
                    activity.binder = binder;
                    activity.isBound = true;
                    // keep instance for preservation across configuration changes
                    this.binder = (SMSServiceBinder)service;
                }
            }
            public void OnServiceDisconnected(ComponentName name)
            {
                activity.isBound = false;
            }
        }
    }
    [Service]
    [IntentFilter(new String[] { "MrGo.SMS.Service" })]
    public class SMSService : Android.App.Service, ISetMemberActivity
    {
        SMSServiceBinder binder;
        public bool startThread = true;
        public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)
        {
            StartServiceInForeground();
            binder.GetDemoService().startThread = true;
            DoWork();
            return StartCommandResult.NotSticky;
        }

        void StartServiceInForeground()
        {
            Notification ongoing = new Notification(Resource.Drawable.icoMrGoSMSService, "MrGO SMS Service");
            PendingIntent pendingIntent = PendingIntent.GetActivity(this, 0, new Intent(this, typeof(DemoActivity)), 0);
            ongoing.SetLatestEventInfo(this, "MrGO SMS Service", "Running", pendingIntent);
            StartForeground((int)NotificationFlags.ForegroundService, ongoing);
        }
        
        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        //void SendNotification()
        //{
        //    var nMgr = (NotificationManager)GetSystemService(NotificationService);
        //    var notification = new Notification(Resource.Drawable.icoMrGoSMSService, "MrGO SMS Service");
        //    var pendingIntent = PendingIntent.GetActivity(this, 0, new Intent(this, typeof(DemoActivity)), 0);
        //    notification.SetLatestEventInfo(this, "MrGO SMS Service", "MrGO SMS Service running in the foreground", pendingIntent);
        //    nMgr.Notify(0, notification);
        //}
        //int count = 0;
        void SendNotificationCount(int smsSend)
        {
            var nMgr = (NotificationManager)GetSystemService(NotificationService);
            var notification = new Notification(Resource.Drawable.icoMrGoSMSService, "MrGO SMS Service, sending : " + smsSend);
            PendingIntent pendingIntent = PendingIntent.GetActivity(this, 0, new Intent(this, typeof(DemoActivity)), 0);
            notification.SetLatestEventInfo(this, "MrGO SMS Service", "MrGO SMS Service, sending : " + smsSend, pendingIntent);
            nMgr.Notify(0, notification);
        }
        void SendNotificationInternetConnection(bool connect)
        {
            Notification ongoing = new Notification(Resource.Drawable.icoMrGoSMSService, "MrGO SMS Service");
            PendingIntent pendingIntent = PendingIntent.GetActivity(this, 0, new Intent(this, typeof(DemoActivity)), 0);
            ongoing.SetLatestEventInfo(this, "MrGO SMS Service", connect?"Running": "No Internet connection", pendingIntent);
            StartForeground((int)NotificationFlags.ForegroundService, ongoing);
        }
        public void DoWork()
        {
            Toast.MakeText(this, "MrGO SMS Service has started", ToastLength.Long).Show();
            var t = new Thread(() => {
                //SendNotification ();
                while (startThread)
                {
                    SendSMSToMember();
                    Thread.Sleep(5000);
                }
                //Log.Debug ("DemoService", "Stopping foreground");
                StopForeground(true);
                StopSelf();
            }
            );
            t.Start();
        }

        public override Android.OS.IBinder OnBind(Android.Content.Intent intent)
        {
            binder = new SMSServiceBinder(this);
            return binder;
        }

        public string GetText()
        {
            return "some text from the service";
        }

        private void SendSMSToMember()
        {
            bool connected = CommonService.CheckInternetConnection(this);
            SendNotificationInternetConnection(connected);
            if (!connected)return;
            //MemberService svc = new MemberService(this);
            //svc.Execute("GetMemberByPendingSMS");
            MemberServiceNoSync m_svc = new MemberServiceNoSync(this);
            if (m_svc.Query("GetMemberByPendingSMS"))
            {
                List<Member> listMember = m_svc.GetResult();
                int count = 0;
                string menuIds = "";
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
                        return;
                    }
                    if (m_svc.Query("UpdateSMSStatus", menuIds))
                    {
                        SendNotificationCount(count);
                    }
                }
            }

        }
        public void SetMemberActivity(string key, Object members)
        {
        //    if (!CommonService.CheckInternetConnection(this))
        //        return;

        //    if (members != null)
        //    {
        //        List<Member> listMember = (List<Member>)members;
        //        int count = 0;
        //        string menuIds = "";
        //        foreach (Member mbr in listMember)
        //        {
        //            try
        //            {
        //                SmsManager smsMgr = SmsManager.Default;
        //                //smsMgr.SendTextMessage(mbr.member_phone, null, "Your MrGo code is " + mbr.member_activationcode + ". Enjoy!", null, null);
        //                smsMgr.SendTextMessage(mbr.member_phone, "+6281100000", "Your MrGo code is " + mbr.member_activationcode + ". Enjoy!", null, null);
        //                if (count == 0)
        //                    menuIds = mbr.member_id.ToString();
        //                else
        //                    menuIds += ("," + mbr.member_id.ToString());
        //                count++;
        //            }
        //            catch (Java.Lang.IllegalArgumentException x)
        //            {
        //                return;
        //            }
        //        }
        //        if (menuIds != "")
        //        {
        //            SendNotificationCount(count);
        //            MemberService svc = new MemberService(this);
        //            svc.Execute("UpdateSMSStatus", menuIds);
        //        }
        //    }
        //    //if (key == "UpdateSMSStatus")
        //    //  jobstarted = false;
        //    //if (members != null)
        //    //{
        //    //    if(members.ToString() =="")
        //    //    m_timer.Start();
        //    //}
        }

        public Context GetContext()
        {
            return this;
        }
    }
    public class SMSServiceBinder : Binder
    {
        SMSService service;
        public SMSServiceBinder(SMSService service)
        {
            this.service = service;
        }
        public SMSService GetDemoService()
        {
            return service;
        }
    }
}