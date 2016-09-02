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
    [Activity(Label = "ReviewOrderActivity", HardwareAccelerated = false)]
    //, HardwareAccelerated = false slow for lolipop......
    public class ReviewOrderActivity : BaseActivity, IBackGroundResult
    {
        string m_menuId, m_menuJumlah = "";
        List<MenuResto> m_orderedMenu = new List<MenuResto>();
        GridView m_gridReview;
        int m_member_id = 0;
        TextView m_tvTotalBeli, m_tvTotalSemua, m_tvBiayaKirim;
        decimal m_ongkir = 0;
        Transaction m_transaction = new Transaction();
        EditText m_etDelAddress;
        Button m_btnPesan;
        AlertDialog.Builder builder;
        TransactionService m_trService;
        int m_itemCountResult = 0;
        ProgressDialog progressDialog;
        protected override int LayoutResource
        {
            get
            {
                return Resource.Layout.page_review_order;
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
            m_trService = new TransactionService(this);
            builder = new AlertDialog.Builder(this);
            m_menuId = Intent.GetStringExtra("menuId");
            m_menuJumlah = Intent.GetStringExtra("menuJumlah");
            string resto_id = Intent.GetStringExtra("resto_id");
            string resto_name = Intent.GetStringExtra("resto_name");
            string resto_address = Intent.GetStringExtra("resto_address");
            string resto_url_image = Intent.GetStringExtra("resto_url_image");
            m_ongkir = Convert.ToDecimal(Intent.GetStringExtra("resto_jumlah_ongkir"));
            m_member_id = Convert.ToInt32(Intent.GetStringExtra("member_id"));
            MenuRestoService mnService = new MenuRestoService(this);
            mnService.Execute("GetMenuByIDInSelect", m_menuId);
            m_gridReview = FindViewById<GridView>(Resource.Id.gridMenuPesanan);
            // Create your application here
            SupportActionBar.Title = resto_name;
            m_tvTotalBeli = FindViewById<TextView>(Resource.Id.textViewTotalBeli);
            m_tvTotalSemua = FindViewById<TextView>(Resource.Id.textViewTotalSemua);
            m_tvBiayaKirim = FindViewById<TextView>(Resource.Id.textViewBiayaKirim);
            m_etDelAddress = FindViewById<EditText>(Resource.Id.editTextDelAddress);
            m_btnPesan = FindViewById<Button>(Resource.Id.buttonPesan);
            m_btnPesan.Click += M_btnPesan_Click;
            m_transaction.member_id = m_member_id;
            m_transaction.resto_id = Convert.ToInt32(resto_id);
        }

        private void M_btnPesan_Click(object sender, EventArgs e)
        {
            if (m_etDelAddress.Text == "")
            {
                builder.SetMessage("Data Alamat kosong..");
                builder.SetPositiveButton("OK", OkCorrectAction);
                builder.Create().Show();
                return;
            }
            else if (m_orderedMenu.Count == 0)
            {
                builder.SetMessage("Tidak ada yang dibeli..");
                builder.SetPositiveButton("OK", OkCorrectAction);
                builder.Create().Show();
                return;
            }
            m_transaction.member_id = m_member_id;
            m_transaction.destination_address = m_etDelAddress.Text;

            int totalbarang = 0;
            foreach (MenuResto menu in m_orderedMenu)// load jumlah beli-------------
            {
                totalbarang += menu.menu_jumlah_pesan;
                TransactionDetail trd = new TransactionDetail();
                trd.menu_id = menu.menu_id;
                trd.menu_name = menu.menu_name;
                trd.transaction_id = m_transaction.transaction_id;
                trd.tr_unit = menu.menu_jumlah_pesan;
                trd.tr_unit_price = menu.menu_price;
                m_transaction.total_buy += (menu.menu_jumlah_pesan * menu.menu_price);
                m_transaction.Items.Add(trd);
            }
            if (totalbarang == 0)
            {
                builder.SetMessage("Tidak ada yang dibeli..");
                builder.SetPositiveButton("OK", OkCorrectAction);
                builder.Create().Show();
                return;
            }
            m_transaction.total_all = m_ongkir + m_transaction.total_buy;
            m_transaction.charge = m_ongkir;
            StartProgress();
            m_trService = new TransactionService(this);
            m_trService.Execute("getmaxid");
        }
        private void OkCorrectAction(object sender, DialogClickEventArgs e)
        {
            //dialogInterface.dismiss();
            //activity.Finish();
        }
        public void SetBackGroundResult(string key, object result)
        {
            if (!CommonService.CheckInternetConnection(this)) { Toast.MakeText(this, "Please check your internet connection", ToastLength.Short).Show(); return; }
            if (key == "GetMenuByIDInSelect")
            {
                if (result == null) return;
                m_orderedMenu = (List<MenuResto>)result;
                string[] menus = m_menuId.Split(',');
                string[] jumlahs = m_menuJumlah.Split(',');
                foreach (MenuResto menu in m_orderedMenu)// load jumlah beli-------------
                {
                    int index = 0;
                    foreach (string id in menus)
                    {
                        if (menu.menu_id.ToString() == id.Trim())
                        {
                            menu.menu_jumlah_pesan = Convert.ToInt32(jumlahs[index]);
                        }
                        index++;
                    }
                }
                loadReview();
                CalculateTotal();
            }
            if (key == "getmaxid")
            {
                if (result == null) return;
                List<Transaction> tr = (List<Transaction>)result;
                if (tr.Count == 0) return;
                m_transaction.transaction_code = (tr[0].transaction_id+1).ToString().PadLeft(6, '0');
                m_trService = new TransactionService(this);
                m_trService.Execute("InsertTransaction", m_transaction);
            }
            if (key == "InsertTransaction")
            {
                m_trService = new TransactionService(this);
                m_trService.Execute("getmaxidbymember", m_member_id);
            }
            if (key == "getmaxidbymember")
            {
                if (result == null) return;
                List<Transaction> tr = (List<Transaction>)result;
                if (tr.Count == 0) return;
                foreach (TransactionDetail dt in m_transaction.Items)
                {
                    dt.transaction_id = tr[0].transaction_id;
                    m_trService = new TransactionService(this);
                    m_trService.Execute("InsertDetailsTransaction", dt);
                }
            }
            if (key == "InsertDetailsTransaction")
            {
                m_itemCountResult++;
                if (m_itemCountResult == m_transaction.Items.Count)
                {
                    Intent intent = new Intent();
                    intent.PutExtra("ordered", "OK");
                    SetResult(Result.Ok, intent);
                    Toast.MakeText(this, "Order berhasil, silahkan menunggu.", ToastLength.Long).Show();
                    endProgress();
                    this.Finish();
                }
            }
        }
        public void CalculateTotal()
        {
            decimal totalbeli = 0;
            decimal totalSemua = 0;
            foreach (MenuResto menu in m_orderedMenu)// load total beli-------------
            {
                totalbeli += (menu.menu_price * menu.menu_jumlah_pesan);
            }
            totalSemua = totalbeli + m_ongkir;

            m_tvTotalBeli.Text = "Rp. "+ totalbeli.ToString(CommonUtils.DECIMAL_FORMAT);
            m_tvBiayaKirim.Text = "Rp. " + m_ongkir.ToString(CommonUtils.DECIMAL_FORMAT);
            m_tvTotalSemua.Text = "Rp. " + totalSemua.ToString(CommonUtils.DECIMAL_FORMAT);

         //   m_transaction.total_all = totalSemua;
        //    m_transaction.total_buy = totalbeli;
          //  m_transaction.charge = m_ongkir;

        }
        private void loadReview()
        {
            m_gridReview.Adapter = new ItemReviewMenuAdapter(this, m_orderedMenu,false);
        }

        public void RemoveItem(MenuResto item)
        {
            m_orderedMenu.Remove(item);
            // m_gridReview.DeferNotifyDataSetChanged();
            m_gridReview.Adapter = new ItemReviewMenuAdapter(this, m_orderedMenu, false);
            CalculateTotal();
        }
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
        protected void StartProgress()
        {
            progressDialog = new ProgressDialog(this);
            progressDialog.SetTitle("Please wait...");
            progressDialog.SetMessage("Connecting to server...");
            //progressDialog.SetIndeterminateDrawable(true);
            progressDialog.SetCancelable(false);
            progressDialog.Show();
        }
        protected void endProgress()
        {
            progressDialog.Dismiss();
        }
    }
}