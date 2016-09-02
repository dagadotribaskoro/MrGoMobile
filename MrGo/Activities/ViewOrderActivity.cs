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
    [Activity(Label = "ViewOrderActivity", HardwareAccelerated = false)]
    //, HardwareAccelerated = false slow for lolipop......
    public class ViewOrderActivity : BaseActivity, IBackGroundResult
    {
        //string m_menuId, m_menuJumlah = "";
        List<MenuResto> m_orderedMenu = new List<MenuResto>();
        GridView m_gridReview;
        int m_member_id = 0;
        TextView m_tvTotalBeli, m_tvTotalSemua, m_tvBiayaKirim, m_tvStatus;
       // decimal m_ongkir = 0;
        Transaction m_transaction = new Transaction();
        TextView textViewDelAddress;
        Button m_btnCancelPesan;
        AlertDialog.Builder builder;
        TransactionService m_trService;
        int m_itemCountResult = 0;
        ProgressDialog progressDialog;
       // int transaction_id = 0;
       // string transaction_code = "";
        protected override int LayoutResource
        {
            get
            {
                return Resource.Layout.page_view_order;
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
            //m_menuId = Intent.GetStringExtra("menuId");
            //m_menuJumlah = Intent.GetStringExtra("menuJumlah");
            m_transaction.transaction_id = Convert.ToInt32(Intent.GetStringExtra("transaction_id"));
            m_transaction.transaction_code = Intent.GetStringExtra("transaction_code");

            //string resto_id = Intent.GetStringExtra("resto_id");
            //string resto_name = Intent.GetStringExtra("resto_name");
            //string resto_address = Intent.GetStringExtra("resto_address");
            //string resto_url_image = Intent.GetStringExtra("resto_url_image");
            //m_ongkir = Convert.ToDecimal(Intent.GetStringExtra("resto_jumlah_ongkir"));
            m_member_id = Convert.ToInt32(Intent.GetStringExtra("member_id"));

            //MenuRestoService mnService = new MenuRestoService(this);
            //mnService.Execute("GetMenuByIDInSelect", m_menuId);
            m_gridReview = FindViewById<GridView>(Resource.Id.gridMenuPesanan);
            // Create your application here
            SupportActionBar.Title = "Order : "+ m_transaction.transaction_code;
            m_tvTotalBeli = FindViewById<TextView>(Resource.Id.textViewTotalBeli);
            m_tvTotalSemua = FindViewById<TextView>(Resource.Id.textViewTotalSemua);
            m_tvBiayaKirim = FindViewById<TextView>(Resource.Id.textViewBiayaKirim);
            textViewDelAddress = FindViewById<TextView>(Resource.Id.textViewDelAddress);
            m_tvStatus = FindViewById<TextView>(Resource.Id.textViewStatus);
            m_btnCancelPesan = FindViewById<Button>(Resource.Id.buttonCancelPesan);
            m_btnCancelPesan.Click += M_btnCancelPesan_Click;
            m_transaction.member_id = m_member_id;
            m_transaction.resto_id = Convert.ToInt32(m_transaction.resto_id);

            m_trService = new TransactionService(this);
            m_trService.Execute("getByID", m_transaction.transaction_id.ToString());
        }

        private void M_btnCancelPesan_Click(object sender, EventArgs e)
        {
            m_trService = new TransactionService(this);
            m_transaction.transaction_status = TransactionStatus.Cancel;
            m_trService.Execute("updateStatus",TransactionStatus.Cancel.ToString() , m_transaction.transaction_id.ToString());
        }

        private void OkCorrectAction(object sender, DialogClickEventArgs e)
        {

        }
        public void SetBackGroundResult(string key, object result)
        {
            if (!CommonService.CheckInternetConnection(this)) { Toast.MakeText(this, "Please check your internet connection", ToastLength.Short).Show(); return; }
            if (key == "getByID")
            {
                if (result == null) return;
                else
                {
                    List<Transaction> m_listTr = (List<Transaction>)result;
                    if (m_listTr.Count == 0) return;
                    m_transaction = m_listTr[0];
                    loadItems();
                }
            }
            if (key == "getTrDetailByTrId")
            {
                if (result == null) return;
                else
                {
                    m_transaction.Items = (List<TransactionDetail>)result;
                    loadTransactionToView();

                    string menus = "";
                    int count = 0;
                    foreach (TransactionDetail dtl in m_transaction.Items)
                    {
                        if (count == 0)
                            menus = dtl.menu_id.ToString();
                        else
                            menus += ("," + dtl.menu_id.ToString());
                        count++;
                    }
                    MenuRestoService svc = new MenuRestoService(this);
                    svc.Execute("GetMenuByIDInSelect", menus);
                }
            }
            if (key == "GetMenuByIDInSelect")
            {
                if (result == null) return;
                m_orderedMenu = (List<MenuResto>)result;
                foreach (MenuResto menu in m_orderedMenu)
                {
                    foreach (TransactionDetail dtl in m_transaction.Items)
                    {
                        if (menu.menu_id == dtl.menu_id)
                            menu.menu_jumlah_pesan = dtl.tr_unit;
                    }
                }
                loadReview();
            }
            if (key == "updateStatus")
            {
                if (m_transaction.transaction_status == TransactionStatus.Cancel)
                {
                    m_btnCancelPesan.Visibility = (m_transaction.transaction_status == TransactionStatus.Waiting) ? ViewStates.Visible : ViewStates.Gone;
                    Toast.MakeText(this, "Order Cancelled", ToastLength.Short).Show();
                }
            }
        }

        private void loadTransactionToView()
        {
            m_tvTotalBeli.Text = "Rp. " + m_transaction.total_buy.ToString(CommonUtils.DECIMAL_FORMAT);
            m_tvBiayaKirim.Text = "Rp. " + m_transaction.charge.ToString(CommonUtils.DECIMAL_FORMAT);
            m_tvTotalSemua.Text = "Rp. " + m_transaction.total_all.ToString(CommonUtils.DECIMAL_FORMAT);
            m_btnCancelPesan.Visibility = (m_transaction.transaction_status == TransactionStatus.Waiting) ? ViewStates.Visible : ViewStates.Gone;
            textViewDelAddress.Text = m_transaction.destination_address;
            m_tvStatus.Text = m_transaction.transaction_status.ToString();
            loadReview();
        }

        private void loadItems()
        {
            m_trService = new TransactionService(this);
            m_trService.Execute("getTrDetailByTrId", m_transaction.transaction_id);
        }

        private void loadReview()
        {
            m_gridReview.Adapter = new ItemReviewMenuAdapter(this, m_orderedMenu, true);
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