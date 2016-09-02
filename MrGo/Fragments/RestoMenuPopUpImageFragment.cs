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

namespace MrGo.Fragments
{
    class RestoMenuPopUpImageFragment : DialogFragment
    {
        //    public event EventHandler<OnSignUpEventArgs> mOnSignUpComplete;
        string m_url = "";
        Service.ImageLoaderMrGo m_ImageLoader;

        public RestoMenuPopUpImageFragment(string url, Service.ImageLoaderMrGo loader)
        { m_url = url;
            m_ImageLoader = loader;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.page_resto_menu_img, container, false);
            ImageView imgV = view.FindViewById<ImageView>(Resource.Id.imageViewMenu);
            m_ImageLoader.DisplayImage(m_url, imgV, -1);
            imgV.Click += ImgV_Click;
            return view;
        }

        private void ImgV_Click(object sender, EventArgs e)
        {
            Dismiss();
        }

        //void mBtnSignUp_Click(object sender, EventArgs e)
        //{
        //    //User has clicked the sign up button
        //    mOnSignUpComplete.Invoke(this, new OnSignUpEventArgs(mTxtFirstName.Text, mTxtEmail.Text, mTxtPassword.Text));
        //    this.Dismiss();
        //}

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle); //Sets the title bar to invisible
            base.OnActivityCreated(savedInstanceState);
             Dialog.Window.Attributes.WindowAnimations = Resource.Style.dialog_animation; //set the animation
        }
    }
}