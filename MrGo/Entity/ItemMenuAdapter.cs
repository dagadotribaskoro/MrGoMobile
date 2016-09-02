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
    public class ItemMenuAdapter : BaseAdapter
    {
        private readonly Activity m_Context;
        private readonly IEnumerable<string> m_item;
        private decimal m_saldo = -1;
        public ItemMenuAdapter(Activity context, IEnumerable<string> item)
        {
            m_Context = context;
            this.m_item = item;
        }
        public ItemMenuAdapter(Activity context, IEnumerable<string> item, decimal saldo)
        {
            m_Context = context;
            this.m_item = item;
            m_saldo = saldo;
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (position < 0)
                return null;
            View view = (convertView ?? m_Context.LayoutInflater.Inflate(Resource.Layout.item_menu_details, parent, false));
            if (view == null)
                return null;
            ImageView imView = view.FindViewById<ImageView>(Resource.Id.imageViewProfile);
            TextView tvItemSaldo = view.FindViewById<TextView>(Resource.Id.textViewSaldo);
            TextView tvItemMenu = view.FindViewById<TextView>(Resource.Id.textViewMenuItem);
            tvItemMenu.Text = m_item.ElementAt(position);
            tvItemSaldo.Text = "Saldo : Rp." + m_saldo.ToString(CommonUtils.DECIMAL_FORMAT);
            if (position == 0)// bad programming-----
            {
                imView.Visibility = ViewStates.Visible;
                tvItemMenu.Visibility = ViewStates.Gone;
                tvItemSaldo.Visibility = ViewStates.Gone;
            }
            if (position == 1)// bad programming-----
            {
              //  if (tvItemMenu.Text == "Saldo")
               // {
                    tvItemSaldo.Visibility = m_saldo < 0 ? ViewStates.Gone : ViewStates.Visible;
                    imView.Visibility = ViewStates.Gone;
                    tvItemMenu.Visibility = ViewStates.Gone;
               // }
            }
            if (position > 1)// bad programming-----
            {
                imView.Visibility = ViewStates.Gone;
                tvItemSaldo.Visibility = ViewStates.Gone;
            }
          //  imView.Click += ImView_Click;
            return view;
        }

        //private void ImView_Click(object sender, EventArgs e)
        //{
        //    FragmentTransaction transaction = m_Context.FragmentManager.BeginTransaction();
        //    Fragments.RestoMenuPopUpImageFragment signUpDialog = new Fragments.RestoMenuPopUpImageFragment();
        //    signUpDialog.Show(transaction, "dialog fragment");
        //}

        public override int Count
        {
            get { return this.m_item.Count(); }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return m_item.ElementAt(position);
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