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
    public class TopFoodFragmentAll : Fragment, IBackGroundResult
    {
        private int m_member_id = 0;
        private List<Resto> _restos;
        GridView grid;
        EditText m_etSearch;
        public TopFoodFragmentAll(int member_id)
        {
            this.RetainInstance = true;
            loadAllRestoBackgroud();
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
            View view = inflater.Inflate(Resource.Layout.fragment_gofoodAll, null);
            grid = view.FindViewById<GridView>(Resource.Id.grid);
            grid.ItemClick += GridOnItemClick;
            //m_etSearch = view.FindViewById<EditText>(Resource.Id.editTextSearch);
            //m_etSearch.EditorAction += M_etSearch_EditorAction;
            return view;
        }

        //private void M_etSearch_EditorAction(object sender, EditorActionEventArgs e)
        //{
        //    if (e.ActionId == Android.Views.InputMethods.ImeAction.Search)
        //    {
        //        loadAllRestoBackgroudSearch();
        //    }
        //}
        //private void loadAllRestoBackgroudSearch()
        //{
        //    RestoService service = new RestoService(this);
        //    service.Execute("GetAllSearch", m_etSearch.Text);
        //}
        private void loadAllRestoBackgroud()
        {
            RestoService service = new RestoService(this);
            service.Execute("GetAllTopTen");
        }


        public void SetBackGroundResult(string key, object result)
        {
            if (!CommonService.CheckInternetConnection(Activity)) { Toast.MakeText(Activity, "Please check your internet connection", ToastLength.Short).Show(); return; }
            if (result != null)
            {
                _restos = (List<Resto>) result;
                grid.Adapter = new RestoAdapter(Activity, _restos);
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
            Intent intent = new Intent(Activity, typeof(RestoActivity));
            //intent.PutExtra("resto", _restos[itemClickEventArgs.Position]);
            intent.PutExtra("resto_id", _restos[itemClickEventArgs.Position].resto_id.ToString());
            intent.PutExtra("resto_name", _restos[itemClickEventArgs.Position].resto_name);
            intent.PutExtra("resto_address", _restos[itemClickEventArgs.Position].resto_address);
            intent.PutExtra("resto_url_image", _restos[itemClickEventArgs.Position].resto_url_image);
            intent.PutExtra("member_id", m_member_id.ToString());
            StartActivity(intent);
        }

        //public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        //{
        //    inflater.Inflate(Resource.Menu.refresh, menu);
        //}
    }
}