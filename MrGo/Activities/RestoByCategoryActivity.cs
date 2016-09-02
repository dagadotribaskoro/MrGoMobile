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
using MrGo.Entity;

namespace MrGo.Activities
{
    [Activity(Label = "Menu Resto")]
    public class RestoByCategoryActivity : BaseActivity, IBackGroundResult
    {
        private int member_id = 0;
        private int restocategory_id = 0;
        private string restocategory_name = "";
        private List<Resto> _restos;
        GridView grid;
        EditText m_etSearch;

        protected override int LayoutResource
        {
            get
            {
                return Resource.Layout.fragment_gofoodAllToolbar;
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
            restocategory_id = Convert.ToInt32( Intent.GetStringExtra("restocategory_id"));
            restocategory_name = Intent.GetStringExtra("restocategory_name");
            member_id = Convert.ToInt32(Intent.GetStringExtra("member_id"));
            SupportActionBar.Title = restocategory_name;
            loadAllRestoBackgroud();
            grid = FindViewById<GridView>(Resource.Id.grid);
            grid.ItemClick += GridOnItemClick;
        }

        private void loadAllRestoBackgroud()
        {
            RestoService service = new RestoService(this);
            service.Execute("GetAllByCategory", restocategory_id.ToString());
        }


        public void SetBackGroundResult(string key, object result)
        {
            if (!CommonService.CheckInternetConnection(this)) { Toast.MakeText(this, "Please check your internet connection", ToastLength.Short).Show(); return; }
            if (result != null)
            {
                _restos = (List<Resto>) result;
                grid.Adapter = new RestoAdapter(this, _restos);
                grid.RefreshDrawableState();
            }
        }

        private void loadResult()
        {
          //  _restos = Util.GenerateResto();
            //_restos.Reverse();
            //grid.Adapter = new RestoAdapter(Activity, _restos);
        }

        private void GridOnItemClick(object sender, AdapterView.ItemClickEventArgs itemClickEventArgs)
        {
            Intent intent = new Intent(this, typeof(RestoActivity));
            //intent.PutExtra("resto", _restos[itemClickEventArgs.Position]);
            intent.PutExtra("resto_id", _restos[itemClickEventArgs.Position].resto_id.ToString());
            intent.PutExtra("resto_name", _restos[itemClickEventArgs.Position].resto_name);
            intent.PutExtra("resto_address", _restos[itemClickEventArgs.Position].resto_address);
            intent.PutExtra("resto_url_image", _restos[itemClickEventArgs.Position].resto_url_image);
            intent.PutExtra("member_id", member_id.ToString());
            StartActivity(intent);
        }

        //public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        //{
        //    inflater.Inflate(Resource.Menu.refresh, menu);
        //}

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    break;
            }
            return false;
        }
    }
}