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

namespace MrGo.Fragments
{
    public class MyOrderTabOnProgressFragment : Fragment, IBackGroundResult
    {
        private int m_member_id = 0;
        List<Transaction> m_listTransaction;
        GridView grid;
        TransactionStatus m_trStatus = TransactionStatus.Waiting;
        Android.App.Activity m_ctx;
        public MyOrderTabOnProgressFragment(int member_id, TransactionStatus trStstus)
        {
            this.RetainInstance = true;
            m_member_id = member_id;
          //  m_ctx = this.Activity;
            m_trStatus = trStstus;
            loadAllTransactionOnProgressBackgroud();
        }
        public void RefreshItems(Android.App.Activity ctx)
        {
            loadAllTransactionOnProgressBackgroud();
          //  m_ctx = ctx;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Android.OS.Bundle savedInstanceState)
        {
            View ignored = base.OnCreateView(inflater, container, savedInstanceState);
            View view = inflater.Inflate(Resource.Layout.fragment_transaction_status, null);
            grid = view.FindViewById<GridView>(Resource.Id.grid);
            grid.ItemClick += Grid_ItemClick;
            //m_etSearch = view.FindViewById<EditText>(Resource.Id.editTextSearch);
            //m_etSearch.EditorAction += M_etSearch_EditorAction;
            return view;
        }

        private void Grid_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Intent intent = new Intent(Activity, typeof(ViewOrderActivity));
            intent.PutExtra("transaction_id", m_listTransaction[ e.Position].transaction_id.ToString());
            intent.PutExtra("transaction_code", m_member_id.ToString());
            intent.PutExtra("member_id", m_member_id.ToString());
            StartActivity(intent);
        }

        private void loadAllTransactionOnProgressBackgroud()
        {
            TransactionService svc = new TransactionService(this);
            svc.Execute("getByMemberByStatus", m_trStatus.ToString(), m_member_id.ToString());
            //service.Execute("GetAllSearch", m_etSearch.Text);
        }
        public void SetBackGroundResult(string key, object result)
        {
            if (!CommonService.CheckInternetConnection(Activity)) { Toast.MakeText(Activity, "Please check your internet connection", ToastLength.Short).Show(); return; }
            if (key == "getByMemberByStatus")
            {
                if (result == null) return;
                m_listTransaction = (List<Transaction>)result;
                grid.Adapter = new TransactionAdapter(Activity, m_listTransaction);
            }
        }
        public Context Context
        {
            get
            {
                return Activity;
            }
        }
    }
}