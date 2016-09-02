using System;
using System.Collections.Generic;

using Android.Content;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using MrGo.Models;
using MrGo.Entity;
using MrGo.Activities;

namespace MrGo.Fragments
{
    public class RestaurantFragment : Fragment
    {
        public RestaurantFragment()
        {
            this.RetainInstance = true;
        }
        private List<RestaurantViewModel> _items;
        public override Android.Views.View OnCreateView(Android.Views.LayoutInflater inflater, Android.Views.ViewGroup container, Android.OS.Bundle savedInstanceState)
        {
            this.HasOptionsMenu = true;
            View ignored = base.OnCreateView(inflater, container, savedInstanceState);
            View view = inflater.Inflate(Resource.Layout.fragment_restaurant, null);
            ListView grid = view.FindViewById<ListView>(Resource.Id.listview);
            _items = Util.GenerateRestaurants();
            grid.Adapter = new RestaurantAdapter(Activity, _items);
            //grid.ItemClick += GridOnItemClick;
            return view;
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.refresh, menu);
        }
    }
}