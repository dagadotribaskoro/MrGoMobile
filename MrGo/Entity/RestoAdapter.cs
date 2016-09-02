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
using Android.Graphics.Drawables;
using Java.Net;
using MrGo.Service;

namespace MrGo.Entity
{
    internal class RestoAdapterWrapper : Java.Lang.Object
    {
        public TextView RestoName { get; set; }
        public TextView RestoAddress { get; set; }
        //public ImageView RestoImageView { get; set; }
        public LinearLayout RestoBackGround { get; set; }
    }

    public class RestoAdapter : BaseAdapter
    {
        private readonly Activity m_Context;
        private readonly IEnumerable<Resto> m_restos;
        public ImageLoaderMrGo ImageLoader { get; set; }
        public RestoAdapter(Activity context, IEnumerable<Resto> restos)
        {
            m_Context = context;
            this.m_restos = restos;
            ImageLoader = new ImageLoaderMrGo(context, 800, 0);
            }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (position < 0)
                return null;
            View view = (convertView?? m_Context.LayoutInflater.Inflate(Resource.Layout.item_resto, parent, false));
            if (view == null)
                return null;
            var wrapper = view.Tag as RestoAdapterWrapper;
            if (wrapper == null)
            {
                wrapper = new RestoAdapterWrapper
                {
                    RestoName = view.FindViewById<TextView>(Resource.Id.item_title),
                    //RestoImageView = view.FindViewById<ImageView>(Resource.Id.item_image),
                    RestoAddress = view.FindViewById<TextView>(Resource.Id.item_address),
                    RestoBackGround = view.FindViewById<LinearLayout>(Resource.Id.item_background)
                };
                view.Tag = wrapper;
            }
            Resto resto = this.m_restos.ElementAt(position);
            wrapper.RestoName.Text = resto.resto_name;
            wrapper.RestoAddress.Text = resto.resto_address;
            //wrapper.RestoName.Alpha = 100;
            //wrapper.RestoAddress.Alpha = 100;

            //  System.IO.InputStream URLcontent = (System.IO.InputStream)new URL(resto.resto_url_image).GetContent();
            //  Drawable image = Drawable.CreateFromStream(URLcontent, "your source link");
            // wrapper.Art.SetImageDrawable(image);

            //wrapper.Art.SetImageBitmap(Android.Graphics.Bitmap.CreateBitmap())
            //LinearLayout ly;ly.SetBackgroundDrawable

            //ImageLoader.DisplayImage(resto.resto_url_image, wrapper.RestoImageView, -1);
            ImageLoader.DisplayImageLinearLayOut(resto.resto_url_image, wrapper.RestoBackGround, -1);
            return view;
        }

        public override int Count
        {
            get { return this.m_restos.Count(); }
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