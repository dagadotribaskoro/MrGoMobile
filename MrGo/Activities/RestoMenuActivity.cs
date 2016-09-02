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
    [Activity(Label = "Menu Resto" )]//, ParentActivity = typeof(HomeActivity)
    //[MetaData("android.support.PARENT_ACTIVITY", Value = "MrGo.HomeActivity")]
    public class RestoMenuActivity : BaseActivity, IBackGroundResult
    {
        private ImageLoaderMrGo m_ImageLoader;
        private Resto m_resto;
        private List<MenuResto> m_restoMenuMakanan = new List<MenuResto>();
        private GridView m_menuGridSpesial;
        string menuId, menuJumlah, menu_foodtype= "";
        protected override int LayoutResource
        {
            get
            {
                return Resource.Layout.page_resto_menu_details;
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
            menu_foodtype = Intent.GetStringExtra("menu_foodtype");
            menuId = Intent.GetStringExtra("menuId");
            menuJumlah = Intent.GetStringExtra("menuJumlah");
            RestoService service = new RestoService(this);
            service.Execute("GetRestoByID", resto_id);
            MenuRestoService mnService = new MenuRestoService(this);
            if(menu_foodtype == "MAKANAN")
                mnService.Execute("GetMenuMakananByRestoID", resto_id);
            else
                mnService.Execute("GetMenuMinumanByRestoID", resto_id);
            resto_name = string.IsNullOrWhiteSpace(resto_name) ? "Resto" : resto_name;
            this.Title = resto_name;
            this.FindViewById<TextView>(Resource.Id.textViewJenis).Text = menu_foodtype == "MAKANAN" ? "Menu Makanan" : "Menu Minuman";

            m_menuGridSpesial = FindViewById<GridView>(Resource.Id.gridMenuMakanan);
            //m_menuGridMinuman = FindViewById<GridView>(Resource.Id.gridMenuMinuman);
            // grid.Adapter = new MonkeyAdapter(this, _friends);
            m_menuGridSpesial.ItemClick += GridOnItemClick;
            //m_menuGridMinuman.ItemClick += GridOnItemClick;
            //set title here
            SupportActionBar.Title = Title;
        }
        
        private void GridOnItemClick(object sender, AdapterView.ItemClickEventArgs itemClickEventArgs)
        {
            //var intent = new Intent(this, typeof(FriendActivity));
            //intent.PutExtra("Title", _friends[itemClickEventArgs.Position].Title);
            //intent.PutExtra("Image", _friends[itemClickEventArgs.Position].Image);
            //StartActivity(intent);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            string menuId = "";
            string menuJumlah = "";
          //  List<MenuResto> result = new List<MenuResto>();
            int count = 0;
            foreach (MenuResto menu in m_restoMenuMakanan)
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
                  //  result.Add(menu);
                }
            }
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Intent intent = new Intent();
                    intent.PutExtra("menuId", menuId);
                    intent.PutExtra("menu_foodtype", menu_foodtype);
                    intent.PutExtra("menuJumlah", menuJumlah);
                    SetResult(Result.Ok, intent);
                    Finish();
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }
        public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            string menuId = "";
            string menuJumlah = "";
            //  List<MenuResto> result = new List<MenuResto>();
            int count = 0;
            foreach (MenuResto menu in m_restoMenuMakanan)
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
                    //  result.Add(menu);
                }
            }
            if (keyCode == Keycode.Back)
            {
                Intent intent = new Intent();
                intent.PutExtra("menuId", menuId);
                intent.PutExtra("menu_foodtype", menu_foodtype);
                intent.PutExtra("menuJumlah", menuJumlah);
                SetResult(Result.Ok, intent);
                Finish();
                return true;
            }
            return false;
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
            if (key == "GetMenuMakananByRestoID" || key == "GetMenuMinumanByRestoID")
            {
                m_restoMenuMakanan = (List<MenuResto>)result;
               

                if (menuId != null)
                {
                    string[] menuIds = menuId.Split(',');
                    string[] menuJumlahs = menuJumlah.Split(',');
                    if (menuIds.Length > 0)
                    {
                        foreach (MenuResto menu in m_restoMenuMakanan)
                        {
                            int pos = 0;
                            foreach (string id in menuIds)
                            {
                                if (menu.menu_id.ToString() == id)
                                {
                                    menu.menu_jumlah_pesan = Convert.ToUInt16(menuJumlahs[pos]);
                                }
                                pos++;
                            }
                        }
                    }
                }
                m_menuGridSpesial.Adapter = new MenuRestoAdapter(this, m_restoMenuMakanan);
            }
        }
    }
}