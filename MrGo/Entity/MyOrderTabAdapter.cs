using Android.Support.V4.App;
using MrGo.Fragments;

namespace MrGo.Entity
{
    public class MyOrderTabAdapter : FragmentPagerAdapter
    {
        private int m_member_id = 0;
        private static readonly string[] Content = new[]
            {
                "WAITING", "ON PROGRESS","CANCEL","DONE"//, "RECENT", "HOT LIST"
            };

        public MyOrderTabAdapter(FragmentManager p0, int member_id)
                : base(p0)
        { m_member_id = member_id; }

        public override int Count
        {
            get { return Content.Length; }
        }
        public void Refresh(Android.App.Activity ctx)
        {
            //for (int i = 0; i < Count; i++)
            //{
            //    MyOrderTabOnProgressFragment fr = (MyOrderTabOnProgressFragment)GetItem(i);
            //    fr.RefreshItems(ctx);
            //}
        }
        public override Fragment GetItem(int index)
        {
            switch (index)
            {//PAGE SELECT / SLIDE
                case 0:
                    return new MyOrderTabOnProgressFragment(m_member_id, TransactionStatus.Waiting);
                case 1:
                     return new MyOrderTabOnProgressFragment(m_member_id, TransactionStatus.OnProgress);
                case 2:
                    return new MyOrderTabOnProgressFragment(m_member_id, TransactionStatus.Cancel);
                case 3:
                    return new MyOrderTabOnProgressFragment(m_member_id, TransactionStatus.Done);
            }

            return null;
        }

        public override Java.Lang.ICharSequence GetPageTitleFormatted(int p0) { return new Java.Lang.String(Content[p0 % Content.Length].ToUpper()); }

    }
}