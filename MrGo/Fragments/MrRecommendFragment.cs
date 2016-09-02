using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
using MrGo.Models;
using MrGo.Entity;
using MrGo.Activities;
using MrGo.Service;
using static Android.Widget.TextView;

namespace MrGo.Fragments
{
    
    public class MrRecommendFragment : Fragment, IBackGroundResult
    {
        int m_member_id = 0;
        private List<MenuResto> m_restoMenuAll = new List<MenuResto>();
        private List<MenuResto> m_restoMenuSpesial = new List<MenuResto>();
        private List<MenuResto> m_restoMenuMakanan = new List<MenuResto>();
        private List<MenuResto> m_restoMenuMinuman = new List<MenuResto>();
        private GridView m_menuGridSpesial, m_menuGridMakanan, m_menuGridMinuman;
        Button buttonPesan;//mBtnMenuMakanan, mBtnMenuMinuman, 
        //private GridView m_menuGridMinuman;
        string menuIdMakanan, menuJumlahMakanan = "";
        string menuIdMinuman, menuJumlahMinuman = "";
        private ImageLoaderMrGo m_ImageLoader;
        private Resto m_resto;

        public MrRecommendFragment(int member_id)
        {
            this.RetainInstance = true;
            m_ImageLoader = new ImageLoaderMrGo(Activity, 800, 0);
            // loadAllRestoBackgroud();
            m_member_id = member_id;
        }
        public Context Context
        {
            get
            {
                return Activity;
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Android.OS.Bundle savedInstanceState)
        {
            View ignored = base.OnCreateView(inflater, container, savedInstanceState);
            View view = inflater.Inflate(Resource.Layout.page_resto_menu_mrrecommend, null);
            string resto_id = "1";
            //string resto_name = "Mr Recommend";
            //string resto_address = "Mr Go";
            //string resto_url_image = "";
            RestoService service = new RestoService(this);
            service.Execute("GetRestoByID", resto_id);

            MenuRestoService mnService = new MenuRestoService(this);
            mnService.Execute("GetMenuByRestoID", resto_id);

            m_menuGridSpesial = view.FindViewById<GridView>(Resource.Id.gridMenuSpesial);
            m_menuGridMinuman = view.FindViewById<GridView>(Resource.Id.gridMenuMinuman);
            m_menuGridMakanan = view.FindViewById<GridView>(Resource.Id.gridMenuMakanan);
            
            buttonPesan = view.FindViewById<Button>(Resource.Id.buttonPesan);

            buttonPesan.Text = m_member_id > 0 ? "PESAN" : "SILAHKAN LOGIN / SIGN UP UNTUK PESAN";
            buttonPesan.Click += ButtonPesan_Click;

            return view;
        }

        void loadRestoInfo()
        {
            m_ImageLoader=new ImageLoaderMrGo(Activity, 800, 0);
            m_ImageLoader.DisplayImageLinearLayOut(m_resto.resto_url_image, this.View.FindViewById<LinearLayout>(Resource.Id.ll_background), -1);
            this.View.FindViewById<TextView>(Resource.Id.resto_name).Text = m_resto.resto_name;
            this.View.FindViewById<TextView>(Resource.Id.resto_address).Text = m_resto.resto_address;
        }

        private void ButtonPesan_Click(object sender, EventArgs e)
        {
            if (m_member_id == 0)
            {
                Intent intent = new Intent(Activity, typeof(StartScreenActivity));
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
                    Intent intent = new Intent(Activity, typeof(ReviewOrderActivity));
                    intent.PutExtra("menuId", menuId);
                    intent.PutExtra("menuJumlah", menuJumlah);
                    intent.PutExtra("resto_id", m_resto.resto_id.ToString());
                    intent.PutExtra("resto_name", m_resto.resto_name);
                    intent.PutExtra("resto_address", m_resto.resto_address);
                    intent.PutExtra("resto_url_image", m_resto.resto_url_image);
                    intent.PutExtra("resto_jumlah_ongkir", m_resto.resto_jumlah_ongkir.ToString());
                    intent.PutExtra("member_id", m_member_id.ToString());
                    StartActivityForResult(intent, 1);
                }
                else
                {
                    Toast.MakeText(Activity, "Anda belum memesan makanan / minuman.", ToastLength.Short).Show();
                }
            }
        }
        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (data != null)
            {
                var memberIdstr = data.GetStringExtra("member_id");
                if (memberIdstr != null) m_member_id = Convert.ToInt32(memberIdstr);

                //Service.MemberService backGroundTask = new Service.MemberService(this);
                //backGroundTask.Execute("getbyid", m_member_id.ToString());
                SettingsServiceLocalDB m_settingSvc = new SettingsServiceLocalDB(Activity);
                Settings m_settingCurrentLogin = m_settingSvc.GetByName(SettingName.CurrentLogin);
                m_settingCurrentLogin.Val_1 = m_member_id.ToString();
                m_settingCurrentLogin.Val_2 = "1";
                m_settingSvc.Update(m_settingCurrentLogin);
                buttonPesan.Text = m_member_id > 0 ? "PESAN" : "SILAHKAN LOGIN / SIGN UP UNTUK PESAN";

                var orderResult = data.GetStringExtra("ordered");
              //  if (orderResult != null)
              //  {
                   // if (orderResult.ToString() == "OK")
                      //  Finish();
                //}
            }
        }
        public void SetBackGroundResult(string key, object result)
        {
            if (!CommonService.CheckInternetConnection(Activity)) { Toast.MakeText(Activity, "Please check your internet connection", ToastLength.Short).Show(); return; }
            if (result == null) return;
            if (key == "GetRestoByID")
            {
                List<Resto> restoList = (List<Resto>)result;
                if (restoList.Count > 0)
                {
                    m_resto = restoList[0];
                    loadRestoInfo();
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
                m_menuGridSpesial.Adapter = new MenuRestoAdapter(Activity, m_restoMenuSpesial);
                m_menuGridMinuman.Adapter = new MenuRestoAdapter(Activity, m_restoMenuMinuman);
                m_menuGridMakanan.Adapter = new MenuRestoAdapter(Activity, m_restoMenuMakanan);
            }
        }
       
    }
}