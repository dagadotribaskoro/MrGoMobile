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
using com.refractored.monodroidtoolkit.imageloader;
using MrGo.Models;
using MrGo.Entity;
using Android.Support.V4.App;
using MrGo.Service;

namespace MrGo.Activities
{
    [Activity(Label = "Resto", ParentActivity = typeof(HomeActivity), HardwareAccelerated = false)]
    [MetaData("android.support.PARENT_ACTIVITY", Value = "MrGo.HomeActivity")]
    public class RestoActivity : BaseActivity, IBackGroundResult
    {
        private ImageLoaderMrGo m_ImageLoader;
        private Resto m_resto;
        //private List<MenuResto> m_restoMenuMakanan = new List<MenuResto>();
        //private List<MenuResto> m_restoMenuMinuman = new List<MenuResto>();
        private List<MenuResto> m_restoMenuAll = new List<MenuResto>();
        private List<MenuResto> m_restoMenuSpesial = new List<MenuResto>();
        private List<MenuResto> m_restoMenuMakanan = new List<MenuResto>();
        private List<MenuResto> m_restoMenuMinuman = new List<MenuResto>();
        private GridView m_menuGridSpesial, m_menuGridMakanan, m_menuGridMinuman;
        Button buttonPesan;//mBtnMenuMakanan, mBtnMenuMinuman, 
        //private GridView m_menuGridMinuman;
        string menuIdMakanan, menuJumlahMakanan = "";
        string menuIdMinuman, menuJumlahMinuman = "";
        int m_member_id = 0;
        protected override int LayoutResource
        {
            get
            {
                return Resource.Layout.page_resto_menub;
            }
        }

        public Context Context
        {
            get
            {
                return this;
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
           // var act = Intent.GetSerializableExtra("resto");
            m_ImageLoader = new ImageLoaderMrGo(this,800,0);
            string resto_id = Intent.GetStringExtra("resto_id");
            string resto_name = Intent.GetStringExtra("resto_name");
            string resto_address = Intent.GetStringExtra("resto_address");
            string resto_url_image = Intent.GetStringExtra("resto_url_image");
            m_member_id = Convert.ToInt32(Intent.GetStringExtra("member_id"));
            RestoService service = new RestoService(this);
            service.Execute("GetRestoByID", resto_id);

            MenuRestoService mnService = new MenuRestoService(this);
            mnService.Execute("GetMenuByRestoID", resto_id);

            //_friends = Util.GenerateFriends();
            //_friends.RemoveRange(0, _friends.Count - 2);
            //string title = Intent.GetStringExtra("Title");
            //string image = Intent.GetStringExtra("Image");

            resto_name = string.IsNullOrWhiteSpace(resto_name) ? "Resto" : resto_name;
            this.Title = resto_name;

            //if (string.IsNullOrWhiteSpace(resto_url_image))
            //    image = _friends[0].Image;


            m_ImageLoader.DisplayImageLinearLayOut(resto_url_image, this.FindViewById<LinearLayout>(Resource.Id.ll_background), -1);
            this.FindViewById<TextView>(Resource.Id.resto_name).Text = resto_name;
            this.FindViewById<TextView>(Resource.Id.resto_address).Text = resto_address;

            m_menuGridSpesial = FindViewById<GridView>(Resource.Id.gridMenuSpesial);
            m_menuGridMinuman = FindViewById<GridView>(Resource.Id.gridMenuMinuman);
            m_menuGridMakanan = FindViewById<GridView>(Resource.Id.gridMenuMakanan);


            // grid.Adapter = new MonkeyAdapter(this, _friends);
            //m_menuGridSpesial.ItemClick += GridOnItemClick;
            //m_menuGridMinuman.ItemClick += GridOnItemClick;

            //set title here
            SupportActionBar.Title = Title;

         //   mBtnMenuMakanan = FindViewById<Button>(Resource.Id.btnMenuMakanan);
         //   mBtnMenuMinuman = FindViewById<Button>(Resource.Id.btnMenuMinuman);
            buttonPesan = FindViewById<Button>(Resource.Id.buttonPesan);

            buttonPesan.Text = m_member_id > 0 ? "PESAN" : "SILAHKAN LOGIN / SIGN UP UNTUK PESAN";
           // mBtnMenuMakanan.Click += MBtnMenuMakanan_Click;
          //  mBtnMenuMinuman.Click += MBtnMenuMinuman_Click;
            buttonPesan.Click += ButtonPesan_Click;

        }

        private void ButtonPesan_Click(object sender, EventArgs e)
        {
            if (m_member_id == 0)
            {
                Intent intent = new Intent(this, typeof(StartScreenActivity));
                StartActivityForResult(intent, 1);
                //Finish();
            }
            else
            {
                string menuId = "";
                string menuJumlah = "";
                int count = 0;
                foreach (MenuResto menu in m_restoMenuAll)
                {
                    if (menu.menu_jumlah_pesan > 0)
                    {
                        if (count == 0)
                        {
                            menuId += menu.menu_id.ToString();
                            menuJumlah += menu.menu_jumlah_pesan.ToString();
                        }
                        else
                        {
                            menuId += ("," + menu.menu_id.ToString());
                            menuJumlah += ("," + menu.menu_jumlah_pesan.ToString());
                        }
                        count++;
                    }
                }

                if (count > 0)
                {
                    Intent intent = new Intent(this, typeof(ReviewOrderActivity));
                    intent.PutExtra("menuId", menuId);
                    intent.PutExtra("menuJumlah", menuJumlah);
                    intent.PutExtra("resto_id", m_resto.resto_id.ToString());
                    intent.PutExtra("resto_name", m_resto.resto_name);
                    intent.PutExtra("resto_address", m_resto.resto_address);
                    intent.PutExtra("resto_url_image", m_resto.resto_url_image);
                    intent.PutExtra("resto_jumlah_ongkir", m_resto.resto_jumlah_ongkir.ToString() );
                    intent.PutExtra("member_id", m_member_id.ToString());
                    StartActivityForResult(intent ,1);
                }
                else
                {
                    Toast.MakeText(this, "Anda belum memesan makanan / minuman.", ToastLength.Short).Show();
                }
            }
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (data != null)
            {
                var memberIdstr = data.GetStringExtra("member_id");
                if (memberIdstr != null) m_member_id = Convert.ToInt32(memberIdstr);

                //Service.MemberService backGroundTask = new Service.MemberService(this);
                //backGroundTask.Execute("getbyid", m_member_id.ToString());
                SettingsServiceLocalDB m_settingSvc = new SettingsServiceLocalDB(this);
                Settings m_settingCurrentLogin = m_settingSvc.GetByName(SettingName.CurrentLogin);
                m_settingCurrentLogin.Val_1 = m_member_id.ToString();
                m_settingCurrentLogin.Val_2 = "1";
                m_settingSvc.Update(m_settingCurrentLogin);
                buttonPesan.Text = m_member_id > 0 ? "PESAN" : "SILAHKAN LOGIN / SIGN UP UNTUK PESAN";

                var orderResult = data.GetStringExtra("ordered");
                if (orderResult != null)
                {
                    if (orderResult.ToString() == "OK")
                        Finish();
                }
            }
        }
        //protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        //{
        //    base.OnActivityResult(requestCode, resultCode, data);
        //    if (data != null)
        //    {
        //        var memberIdstr = data.GetStringExtra("member_id");
        //        if (memberIdstr != null) member_id = Convert.ToInt32(memberIdstr);


        //        Service.MemberService backGroundTask = new Service.MemberService(this);
        //        backGroundTask.Execute("getbyid", member_id.ToString());
        //    }
        //}
        private void MBtnMenuMinuman_Click(object sender, EventArgs e)// sementara tutup
        {

            Intent intent = new Intent(this, typeof(RestoMenuActivity));
            //intent.PutExtra("resto", _restos[itemClickEventArgs.Position]);
            intent.PutExtra("resto_id", m_resto.resto_id.ToString());
            intent.PutExtra("resto_name", m_resto.resto_name);
            intent.PutExtra("resto_address", m_resto.resto_address);
            intent.PutExtra("resto_url_image", m_resto.resto_url_image);
            intent.PutExtra("menu_foodtype", "MINUMAN");
            intent.PutExtra("menuId", menuIdMinuman);
            intent.PutExtra("menuJumlah", menuJumlahMinuman);
            StartActivityForResult(intent,1);
        }

        private void MBtnMenuMakanan_Click(object sender, EventArgs e)// sementara tutup
        {
            Intent intent = new Intent(this, typeof(RestoMenuActivity));
            //intent.PutExtra("resto", _restos[itemClickEventArgs.Position]);
            intent.PutExtra("resto_id", m_resto.resto_id.ToString());
            intent.PutExtra("resto_name", m_resto.resto_name);
            intent.PutExtra("resto_address", m_resto.resto_address);
            intent.PutExtra("resto_url_image", m_resto.resto_url_image);
            intent.PutExtra("menu_foodtype", "MAKANAN");
            intent.PutExtra("menuId", menuIdMakanan);
            intent.PutExtra("menuJumlah", menuJumlahMakanan);
            StartActivityForResult(intent,1);
        }
        //tidak digunakan--------------
        protected void OnActivityResultXXX(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
           // base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok)
            {
                string menu_foodtype = data.GetStringExtra("menu_foodtype");
                string menuId = data.GetStringExtra("menuId");
                string menuJumlah = data.GetStringExtra("menuJumlah");
                if (menu_foodtype == "MAKANAN")
                {
                    menuJumlahMakanan = menuJumlah;
                    menuIdMakanan = menuId;
                }
                else
                {
                    menuJumlahMinuman = menuJumlah;
                    menuIdMinuman = menuId;
                }
                
            }
        }
        private void GridOnItemClick(object sender, AdapterView.ItemClickEventArgs itemClickEventArgs)
        {
            //var intent = new Intent(this, typeof(FriendActivity));
            //intent.PutExtra("Title", _friends[itemClickEventArgs.Position].Title);
            //intent.PutExtra("Image", _friends[itemClickEventArgs.Position].Image);
            //StartActivity(intent);
            //string url = m_restoMenuSpesial[itemClickEventArgs.Position].menu_url_image;
            //Android.App.FragmentTransaction transaction = FragmentManager.BeginTransaction();
            //Fragments.RestoMenuPopUpImageFragment signUpDialog = new Fragments.RestoMenuPopUpImageFragment(url, m_ImageLoader);
            //signUpDialog.Show(transaction, "dialog fragment");
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:

                    NavUtils.NavigateUpFromSameTask(this);

                    //Wrong:
                    //var intent = new Intent(this, typeof(HomeView));
                    //intent.AddFlags(ActivityFlags.ClearTop);
                    //StartActivity(intent);


                    //if this could be launched externally:

                    /*var upIntent = NavUtils.GetParentActivityIntent(this);
                    if (NavUtils.ShouldUpRecreateTask(this, upIntent))
                    {
                        // This activity is NOT part of this app's task, so create a new task
                        // when navigating up, with a synthesized back stack.
                        Android.Support.V4.App.TaskStackBuilder.Create(this).
                            AddNextIntentWithParentStack(upIntent).StartActivities();
                    }
                    else
                    {
                        // This activity is part of this app's task, so simply
                        // navigate up to the logical parent activity.
                        NavUtils.NavigateUpTo(this, upIntent); 
                    }*/

                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

        public void SetBackGroundResult(string key, object result)
        {
            if (!CommonService.CheckInternetConnection(this)) { Toast.MakeText(this, "Please check your internet connection", ToastLength.Short).Show(); return; }
            if (result == null) return;
            if (key=="GetRestoByID")
            {
                List<Resto> restoList = (List<Resto>)result;
                if (restoList.Count > 0)
                {
                    m_resto = restoList[0];
                }
            }
            if (key == "GetMenuByRestoID")
            {
                m_restoMenuAll = (List<MenuResto>)result;
                foreach (MenuResto menu in m_restoMenuAll)
                {
                    if (menu.menu_foodtype == "MAKANAN")
                        m_restoMenuMakanan.Add(menu);
                    if (menu.menu_foodtype == "MINUMAN")
                        m_restoMenuMinuman.Add(menu);
                    if (menu.menu_foodtype == "SPESIAL")
                        m_restoMenuSpesial.Add(menu);
                }
                m_menuGridSpesial.Adapter = new MenuRestoAdapter(this, m_restoMenuSpesial);
                m_menuGridMinuman.Adapter = new MenuRestoAdapter(this, m_restoMenuMinuman);
                m_menuGridMakanan.Adapter = new MenuRestoAdapter(this, m_restoMenuMakanan);
            }
        }
    }
}