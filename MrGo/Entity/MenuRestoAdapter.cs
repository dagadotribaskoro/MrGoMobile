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
    internal class MenuRestoAdapterWrapper : Java.Lang.Object
    {
        public TextView TVNama { get; set; }
        public TextView TVHarga { get; set; }
        public TextView TVJumlah { get; set; }
        public ImageView IVGambar { get; set; }
        public Button BtnTambah { get; set; }
        public Button BtnKurang { get; set; }
        public int Jumlah { get; set; }
        public MenuResto MenuResto { get; set; }
    }

    public class MenuRestoAdapter : BaseAdapter
    {
        private readonly Activity m_Context;
        private readonly IEnumerable<MenuResto> m_restos;
        public ImageLoaderMrGo ImageLoader { get; set; }
        public MenuRestoAdapter(Activity context, IEnumerable<MenuResto> restos)
        {
            m_Context = context;
            this.m_restos = restos;
            ImageLoader = new ImageLoaderMrGo(context, 800, 0);
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (position < 0)
                return null;
            View view = (convertView ?? m_Context.LayoutInflater.Inflate(Resource.Layout.item_menu_resto, parent, false));
            if (view == null)
                return null;
            var wrapper = view.Tag as MenuRestoAdapterWrapper;
            MenuResto resto = this.m_restos.ElementAt(position);

            if (wrapper == null)
            {
                wrapper = new MenuRestoAdapterWrapper
                {
                    TVNama = view.FindViewById<TextView>(Resource.Id.textViewNama),
                    TVHarga = view.FindViewById<TextView>(Resource.Id.textViewHarga),
                    TVJumlah = view.FindViewById<TextView>(Resource.Id.textViewJumlah),
                    IVGambar = view.FindViewById<ImageView>(Resource.Id.imageViewURL),
                    BtnTambah = view.FindViewById<Button>(Resource.Id.buttonTambah),
                    BtnKurang = view.FindViewById<Button>(Resource.Id.buttonKurang)
                };
                view.Tag = wrapper;
                wrapper.TVNama.Text = resto.menu_name;
                wrapper.TVHarga.Text = "Rp. " + resto.menu_price.ToString();
                wrapper.TVJumlah.Text = resto.menu_jumlah_pesan.ToString();
                wrapper.Jumlah = resto.menu_jumlah_pesan;
                ImageLoader.DisplayImage(resto.menu_url_image, wrapper.IVGambar, -1);

                wrapper.BtnTambah.Click += BtnTambah_Click;
                wrapper.BtnKurang.Click += BtnKurang_Click;

                wrapper.BtnTambah.Tag = wrapper;
                wrapper.BtnKurang.Tag = wrapper;
                wrapper.IVGambar.Tag = wrapper;
                wrapper.TVNama.Tag = wrapper;
                wrapper.TVHarga.Tag = wrapper;

                wrapper.MenuResto = resto;
                wrapper.IVGambar.Click += IVGambar_Click;
                wrapper.TVNama.Click += IVGambar_Click;
                wrapper.TVHarga.Click += IVGambar_Click;
            }

            return view;
        }

        private void IVGambar_Click(object sender, EventArgs e)
        {
            MenuRestoAdapterWrapper wrapper = null;
            if (sender is ImageView)
            {
                ImageView imgv = (ImageView)sender;
                wrapper = (MenuRestoAdapterWrapper)imgv.Tag;
            }
            if (sender is TextView)
            {
                TextView tcv = (TextView)sender;
                wrapper = (MenuRestoAdapterWrapper)tcv.Tag;
            }
            string url = wrapper.MenuResto.menu_url_image;
            Android.App.FragmentTransaction transaction = m_Context.FragmentManager.BeginTransaction();
            Fragments.RestoMenuPopUpImageFragment signUpDialog = new Fragments.RestoMenuPopUpImageFragment(url, ImageLoader);
            signUpDialog.Show(transaction, "dialog fragment");
        }

        private void BtnKurang_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            MenuRestoAdapterWrapper wrapper = (MenuRestoAdapterWrapper)btn.Tag;
            wrapper.Jumlah = wrapper.Jumlah - 1;
            if (wrapper.Jumlah < 0)
                wrapper.Jumlah = 0;
            wrapper.TVJumlah.Text = wrapper.Jumlah.ToString();
            wrapper.MenuResto.menu_jumlah_pesan = wrapper.Jumlah;
        }

        private void BtnTambah_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            MenuRestoAdapterWrapper wrapper = (MenuRestoAdapterWrapper)btn.Tag;
            wrapper.Jumlah++;
            wrapper.TVJumlah.Text = wrapper.Jumlah.ToString();
            wrapper.MenuResto.menu_jumlah_pesan = wrapper.Jumlah;
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