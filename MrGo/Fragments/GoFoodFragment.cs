using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V4.View;
using com.refractored;
using Android.Support.V4.App;
using MrGo.Entity;

namespace MrGo.Fragments
{
    public class GoFoodFragment : Fragment
    {
        private ViewPager m_ViewPager;
        private PagerSlidingTabStrip m_PageIndicator;
        private FragmentPagerAdapter m_Adapter;
        private HomeActivity m_parentActivity;
        private int m_member_id = 0;

        public GoFoodFragment(HomeActivity act, int member_id)
        {
            this.RetainInstance = true;
            m_parentActivity = act;
            m_member_id = member_id;
        }

        public override Android.Views.View OnCreateView(Android.Views.LayoutInflater inflater, Android.Views.ViewGroup container, Bundle savedInstanceState)
        {
            this.HasOptionsMenu = true;
            var ignored = base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.fragment_gofood, null);

            // Create your application here
            this.m_ViewPager = view.FindViewById<ViewPager>(Resource.Id.viewPager);
            this.m_ViewPager.OffscreenPageLimit = 4;
            this.m_PageIndicator = view.FindViewById<PagerSlidingTabStrip>(Resource.Id.tabs);

            //Since we are a fragment in a fragment you need to pass down the child fragment manager!
            this.m_Adapter = new GoFoodAdapter(this.ChildFragmentManager, m_member_id);
            this.m_ViewPager.Adapter = this.m_Adapter;
            this.m_PageIndicator.SetViewPager(this.m_ViewPager);
            return view;
        }
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
          //  inflater.Inflate(Resource.Menu.refresh, menu);
            inflater.Inflate(Resource.Menu.home, menu);
        }
        //public override bool OnOptionsItemSelected(IMenuItem item)
        //{
        //    //Toast.MakeText(this, "Top ActionBar pressed: " + item.TitleFormatted, ToastLength.Short).Show();
        //    return base.OnOptionsItemSelected(item);
        //}
    }
}