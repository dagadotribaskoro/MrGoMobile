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
    internal class TransactionAdapterWrapper : Java.Lang.Object
    {
        public TextView TransactionCode { get; set; }
        public TextView TransactionDate { get; set; }
        public TextView RestoName { get; set; }
        public ImageView RestoImageView { get; set; }
        public TextView TransactionStatus { get; set; }
    }
    public class TransactionAdapter : BaseAdapter
    {
        private readonly Activity m_Context;
        private readonly IEnumerable<Transaction> m_transactions;
        public ImageLoaderMrGo ImageLoader { get; set; }
        public TransactionAdapter(Activity context, IEnumerable<Transaction> transactions)
        {
            m_Context = context;
            this.m_transactions = transactions;
            ImageLoader = new ImageLoaderMrGo(context, 800, 0);
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (position < 0)
                return null;
            View view = (convertView ?? m_Context.LayoutInflater.Inflate(Resource.Layout.item_transaction, parent, false));
            if (view == null)
                return null;
            var wrapper = view.Tag as TransactionAdapterWrapper;
            if (wrapper == null)
            {
                wrapper = new TransactionAdapterWrapper
                {
                    TransactionCode = view.FindViewById<TextView>(Resource.Id.textViewTransactionNo),
                    TransactionDate = view.FindViewById<TextView>(Resource.Id.textViewTansactionDate),
                    RestoName = view.FindViewById<TextView>(Resource.Id.textViewRestoName),
                    TransactionStatus = view.FindViewById<TextView>(Resource.Id.textViewTransactionStatus),
                    RestoImageView = view.FindViewById<ImageView>(Resource.Id.imageViewResto)
                };
                view.Tag = wrapper;
            }
            Transaction resto = this.m_transactions.ElementAt(position);
            wrapper.TransactionCode.Text = "No. "+resto.transaction_code;
            wrapper.TransactionDate.Text = resto.transaction_date.ToString(CommonUtils.DATE_FORMAT_VIEW);
            wrapper.RestoName.Text = resto.resto_name;
            wrapper.TransactionStatus.Text = resto.transaction_status.ToString();
            ImageLoader.DisplayImage(resto.resto_url_image, wrapper.RestoImageView, -1);
            return view;
        }
        public override int Count
        {
            get { return this.m_transactions.Count(); }
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