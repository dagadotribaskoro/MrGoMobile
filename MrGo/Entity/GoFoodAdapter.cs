using Android.Support.V4.App;
using MrGo.Fragments;

namespace MrGo.Entity
{
    public class GoFoodAdapter : FragmentPagerAdapter
    {
        private int m_member_id = 0;
        private static readonly string[] Content = new[]
            {
                "TOP 10", "MR RECOMMENDED", "MR FOOD"//, "RECENT", "HOT LIST"
            };

        public GoFoodAdapter(FragmentManager p0, int member_id)
                : base(p0)
        { m_member_id = member_id; }

        public override int Count
        {
            get { return Content.Length; }
        }

        public override Fragment GetItem(int index)
        {
            switch (index)
            {//PAGE SELECT / SLIDE
                case 0:
                    return new TopFoodFragmentAll(m_member_id);
                case 1:
                    return new MrRecommendFragment(m_member_id);
                case 2:
                    return new MrFoodFragment(m_member_id);
            }

            return new TopFoodFragmentAll(m_member_id);
            //return new BrowseFragment();
        }

        public override Java.Lang.ICharSequence GetPageTitleFormatted(int p0) { return new Java.Lang.String(Content[p0 % Content.Length].ToUpper()); }
    }
}