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
using MrGo.Helpers;
using Android.Support.V4.Widget;
using MrGo.Activities;
using MrGo.Fragments;
using Android.Content.Res;
using Android.Content.PM;
using Java.IO;
using MrGo.Service;

namespace MrGo
{
   // [Activity(Label = "MrGo",  LaunchMode = LaunchMode.SingleTop, Icon = "@drawable/ic_launcher")]
   [Activity(Label = "MrGo", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, Icon = "@drawable/icoMrGo")]
    public class HomeActivity : BaseActivity, ISetMemberActivity, ISerializable
    {
        //TextView tvWelcomeUser;
        //Member mCurrentMember;
        //Toolbar tbMain;

        private MyActionBarDrawerToggle drawerToggle;
        private string drawerTitle;
        private string title;
        private int member_id = 0;
        private Member m_member;
        private DrawerLayout drawerLayout;
        private ListView drawerListView;
        private string[] SectionsLogin = new[] {"ImageDrawer","Saldo", "Beranda", "Status Pesanan", "My Profile", "LogOut", "Help" };
        private string[] SectionsNoLogin = new[] {"ImageDrawer", "Saldo", "Beranda", "Login", "Help" };
        SettingsServiceLocalDB m_settingSvc;
        Settings m_settingCurrentLogin;
        protected override int LayoutResource
        {
            get
            {
                return Resource.Layout.page_home_view;
            }
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.title = this.drawerTitle = this.Title;
            this.drawerLayout = this.FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            this.drawerListView = this.FindViewById<ListView>(Resource.Id.lv_drawer);
            //Create Adapter for drawer List
            this.drawerListView.Adapter = new ItemMenuAdapter(this, SectionsNoLogin);
                //new ArrayAdapter<string>(this, Resource.Layout.item_menu, Sections);
            //Set click handler when item is selected
            //this.drawerListView.ItemClick += (sender, args) => ListItemClicked(args.Position);
            this.drawerListView.ItemClick += DrawerListView_ItemClick;
            //Set Drawer Shadow
            this.drawerLayout.SetDrawerShadow(Resource.Drawable.drawer_shadow_dark, (int)GravityFlags.Start);
            //DrawerToggle is the animation that happens with the indicator next to the actionbar
            this.drawerToggle = new MyActionBarDrawerToggle(this, this.drawerLayout,
                this.Toolbar,
                Resource.String.drawer_open,
                Resource.String.drawer_close);
            //Display the current fragments title and update the options menu
            this.drawerToggle.DrawerClosed += (o, args) => {
                this.SupportActionBar.Title = this.title;
                this.InvalidateOptionsMenu();
            };
            //Display the drawer title and update the options menu
            this.drawerToggle.DrawerOpened += (o, args) => {
                this.SupportActionBar.Title = this.drawerTitle;
                this.InvalidateOptionsMenu();
            };
            //Set the drawer lister to be the toggle.
            this.drawerLayout.SetDrawerListener(this.drawerToggle);
            //if first time you will want to go ahead and click first item.
            //     if (savedInstanceState == null)
            //   {
            //  ListItemClicked(0);
            //   }
            //----------------------------------------------------------
            //SetContentView(Resource.Layout.Home);
            //tvWelcomeUser = (TextView)FindViewById(Resource.Id.tvWelcomeUser);
            //string memberId = Intent.GetStringExtra("memberId");
            //Service.MemberService backGroundTask = new Service.MemberService(this);
            //backGroundTask.Execute("getbyid", memberId);
            //tbMain = FindViewById<Toolbar>(Resource.Id.toolbar);
            //SetActionBar(tbMain);
            //ActionBar.Title = "Mr.Go!";
            //var toolbarBottom = FindViewById<Toolbar>(Resource.Id.toolbar_bottom);
            //toolbarBottom.Title = "Photo Editing";
            //toolbarBottom.InflateMenu(Resource.Menu.home);
            // Attach item selected handler to navigation view
            //var navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            //navigationView.NavigationItemSelected += NavigationView_NavigationItemSelected;
            //check Is LOGIN
            m_settingSvc = new SettingsServiceLocalDB(this);
            m_settingCurrentLogin = m_settingSvc.GetByName(SettingName.CurrentLogin);
            if (m_settingCurrentLogin == null)
            {
                m_settingCurrentLogin = new Settings();
                m_settingCurrentLogin.Name = SettingName.CurrentLogin;
                m_settingSvc.Insert(m_settingCurrentLogin);
            }
            if (m_settingCurrentLogin.Val_2 == "1")
            {
                member_id = Convert.ToInt32(m_settingCurrentLogin.Val_1);
                Service.MemberService backGroundTask = new Service.MemberService(this);

                    backGroundTask.Execute("getbyid", member_id.ToString());
              
               
            }
            //--------------------
            startBeranda();

        }
        private void startBeranda()
        {
            Android.Support.V4.App.Fragment fragment = new GoFoodFragment(this, member_id);
            SupportFragmentManager.BeginTransaction()
                  .Replace(Resource.Id.content_frame, fragment)
                  .Commit();

            this.drawerListView.SetItemChecked(2, true);
            SupportActionBar.Title = this.title = "Beranda";
            this.drawerLayout.CloseDrawers();
        }
        private void DrawerListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Android.Support.V4.App.Fragment fragment = null;
            TextView tvItemMenu = e.View.FindViewById<TextView>(Resource.Id.textViewMenuItem);
            switch (tvItemMenu.Text)
            {
                case "Beranda":
                    fragment = new GoFoodFragment(this, member_id);
                   // startBeranda();return;
                    break;
                case "Status Pesanan":
                    fragment = new MyOrderFragment(this, member_id);
                    break;
                case "My Profile":
                    fragment = new ProfileFragment();
                    break;
                case "Help":
                    fragment = new BrowseFragment();
                    break;
                case "LogOut":
                    m_member = null;
                    member_id = 0;
                    this.drawerListView.Adapter = new ItemMenuAdapter(this, SectionsNoLogin);
                    m_settingCurrentLogin.Val_2 = "0";
                    m_settingSvc.Update(m_settingCurrentLogin);
                    Toast.MakeText(this, "You has been logged out.", ToastLength.Short).Show();
                    startBeranda();
                    return;
                    //break;
                case "Login":
                    Intent intent = new Intent(this, typeof(StartScreenActivity));
                    StartActivityForResult(intent,1);
                    break;
            }
            if (fragment != null)
            {
                SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.content_frame, fragment)
                    .Commit();

                this.drawerListView.SetItemChecked(e.Position, true);
                if(member_id>0)
                    SupportActionBar.Title = this.title = SectionsLogin[e.Position];
                else
                    SupportActionBar.Title = this.title = SectionsNoLogin[e.Position];

                this.drawerLayout.CloseDrawers();
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (data != null)
            {
                var memberIdstr = data.GetStringExtra("member_id");
                if (memberIdstr != null) member_id = Convert.ToInt32(memberIdstr);
                

                Service.MemberService backGroundTask = new Service.MemberService(this);
                backGroundTask.Execute("getbyid", member_id.ToString());

                m_settingCurrentLogin.Val_1 = member_id.ToString();
                m_settingCurrentLogin.Val_2 = "1";
                m_settingSvc.Update(m_settingCurrentLogin);
                startBeranda();
            }
        }
        protected override void OnResume()
        {
            base.OnResume();
            m_settingCurrentLogin = m_settingSvc.GetByName(SettingName.CurrentLogin);
            if (m_settingCurrentLogin.Val_2 == "1")
            {
                member_id = Convert.ToInt32(m_settingCurrentLogin.Val_1);
                Service.MemberService backGroundTask = new Service.MemberService(this);
                backGroundTask.Execute("getbyid", member_id.ToString());
            }
        }

        //private void ListItemClicked(int position)
        //{
        //    Android.Support.V4.App.Fragment  fragment = null;
        //    switch (position)
        //    {
        //        case 0:
        //            fragment = new GoFoodFragment(this);
        //            break;
        //        case 1:
        //            fragment = new RestaurantFragment();
        //            break;
        //        case 2:
        //            fragment = new ProfileFragment();
        //            break;
        //        case 3:
        //            fragment = new BrowseFragment();
        //            break;
        //    }

        //    SupportFragmentManager.BeginTransaction()
        //        .Replace(Resource.Id.content_frame, fragment)
        //        .Commit();

        //    this.drawerListView.SetItemChecked(position, true);
        //    SupportActionBar.Title = this.title = Sections[position];
        //    this.drawerLayout.CloseDrawers();
        //}
        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            var drawerOpen = this.drawerLayout.IsDrawerOpen((int)GravityFlags.Left);
            //when open don't show anything
            for (int i = 0; i < menu.Size(); i++)
                menu.GetItem(i).SetVisible(!drawerOpen);
            return base.OnPrepareOptionsMenu(menu);
        }

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            this.drawerToggle.SyncState();
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            this.drawerToggle.OnConfigurationChanged(newConfig);
        }
        // Pass the event to ActionBarDrawerToggle, if it returns
        // true, then it has handled the app icon touch event
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (this.drawerToggle.OnOptionsItemSelected(item))
                return true;
            return base.OnOptionsItemSelected(item);
        }
        public void SetMemberActivity(string key, Member member)
        {
            if (!CommonService.CheckInternetConnection(this)) { Toast.MakeText(this, "Please check your internet connection", ToastLength.Short).Show(); return; }
            m_member = member;
            this.drawerListView.Adapter = new ItemMenuAdapter(this, SectionsLogin, m_member.member_balance);
            //tvWelcomeUser.Text = "Welcome " + mCurrentMember.member_name + "!...";
        }
        public Context GetContext()
        {
            return this;
        }
        //public override bool OnCreateOptionsMenu(IMenu menu)
        //{
            //MenuInflater.Inflate(Resource.Menu.home, menu);
            //return base.OnCreateOptionsMenu(menu);
       // }

       // public override bool OnOptionsItemSelected(IMenuItem item)
       // {
            //Toast.MakeText(this, "Top ActionBar pressed: " + item.TitleFormatted, ToastLength.Short).Show();
            //return base.OnOptionsItemSelected(item);
       // }
    }
}