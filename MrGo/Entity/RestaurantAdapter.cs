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
using MrGo.Models;
using com.refractored.monodroidtoolkit.imageloader;
using Android.Graphics.Drawables;
using System.IO;

namespace MrGo.Entity
{
    internal class RestaurantAdapterWrapper : Java.Lang.Object
    {
        public TextView Title { get; set; }
        public ImageView Art { get; set; }
        public LinearLayout LayOut { get; set; }
    }
    public class RestaurantAdapter : BaseAdapter
    {
        private readonly Activity m_Context;
        private readonly IEnumerable<RestaurantViewModel> m_items;
        public ImageLoader ImageLoader { get; set; }

        public RestaurantAdapter(Activity context, IEnumerable<RestaurantViewModel> items)
        {
            m_Context = context;
            this.m_items = items;
            ImageLoader = new ImageLoader(context);
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (position < 0)
                return null;
            View view = (convertView??m_Context.LayoutInflater.Inflate(
                                    Resource.Layout.item_restaurant , parent, false)
                        );
            if (view == null)
                return null;
            RestaurantAdapterWrapper wrapper = view.Tag as RestaurantAdapterWrapper;
            if (wrapper == null)
            {
                wrapper = new RestaurantAdapterWrapper
                {
                    Title = view.FindViewById<TextView>(Resource.Id.item_title),
                    Art = view.FindViewById<ImageView>(Resource.Id.item_image),
                    LayOut = view.FindViewById<LinearLayout>(Resource.Id.llbackground)
                };
                view.Tag = wrapper;
               // wrapper.Art.SetScaleType(ImageView.ScaleType.FitXy);
            }
            RestaurantViewModel friend = this.m_items.ElementAt(position);
            wrapper.Title.Text = friend.Title;
            ImageLoader.DisplayImage(friend.Image, wrapper.Art, -1);

            //Java.Net.HttpURLConnection url = new Java.Net.HttpURLConnection("http://images.nationalgeographic.com/wpf/media-live/photos/000/005/overrides/japanese-macaque_589_100x75.jpg");
            //Stream iStream = url.InputStream;
            //System.IO.Stream istr = (System.IO.Stream)content;
            //Drawable d = Drawable.CreateFromStream(istr, "test");
            //wrapper.LayOut.SetBackgroundDrawable(d);


                       return view;
        }

        public override int Count
        {
            get { return this.m_items.Count(); }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override bool HasStableIds
        {
            get
            {
                return true;
            }
        }
    }
}