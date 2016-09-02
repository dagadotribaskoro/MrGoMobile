using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using MrGo.Service;
using MrGo.Entity;
using Android.Content.PM;

namespace MrGo
{
    [Activity(Label = "MrGo", LaunchMode = LaunchMode.SingleTop, Icon = "@drawable/ic_launcher")]
    public class SignInActivity : Activity, ISetMemberActivity
    {
        Button btnLogin;
        EditText  etPassword;
        AutoCompleteTextView etEmail;
        Member mCurrentMember;
        AlertDialog.Builder builder;
        private ArrayAdapter<String> adapter;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.SignInScreen);
            //ActionBar.Hide();
            builder = new AlertDialog.Builder(this);
            btnLogin = (Button)FindViewById(Resource.Id.btnLogIn);
            etEmail = (AutoCompleteTextView)FindViewById(Resource.Id.etEmail);
            etPassword = (EditText)FindViewById(Resource.Id.etPassword);

            btnLogin.Click += BtnLogin_Click;

            string[] emails = SettingsStringAutoComplete.GetAutoComplete(SettingName.Email, this);
            adapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleDropDownItem1Line, emails);
            etEmail.Adapter = adapter;


        }
        public void SetMemberActivity(string key, Member member)
        {
            if (!CommonService.CheckInternetConnection(this)) { Toast.MakeText(this, "Please check your internet connection", ToastLength.Short).Show(); return; }
            if (member == null)
            {
                builder.SetMessage("Oops! Login failed!.. Please try again.");
                builder.SetPositiveButton("OK", OkCorrectAction);
                builder.Create().Show();
            }
            else
            {
                if (member == null) return;
                if (member.member_status == "Aktif")
                {
                    mCurrentMember = member;
                    Intent i = new Intent();
                    i.PutExtra("member_id", member.member_id.ToString());
                    SetResult(Result.Ok, i);
                    Finish();
                }
                else
                {
                    mCurrentMember = member;
                    builder = new AlertDialog.Builder(this);
                    builder.SetTitle("Information");
                    builder.SetMessage("User is not activate yet. You want to activate?");
                    builder.SetNegativeButton("NO", OkCorrectAction);
                    builder.SetPositiveButton("YES", YESAction);
                    AlertDialog alert = builder.Create();
                    alert.Show();
                }
            }
        }
        private void OkCorrectAction(object sender, DialogClickEventArgs e)
        {
            //dialogInterface.dismiss();
            //activity.Finish();
        }
        private void YESAction(object sender, DialogClickEventArgs e)
        {
            if (mCurrentMember != null)
            {
                MemberService svc = new MemberService(this);
                svc.Execute("updateReSentCodeByEmail", mCurrentMember.member_email);

                Intent i = new Intent(this, typeof(ConfirmationActivity));
                i.PutExtra("memberEmail", mCurrentMember.member_email);
                i.PutExtra("memberPhone", "xxxxxxxxx");
                this.StartActivityForResult(i, 1);
            }
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (data != null)
            {
                var memberIdstr = data.GetStringExtra("member_id");
                if (memberIdstr != null)
                {
                    Intent i = new Intent();
                    i.PutExtra("member_id", memberIdstr.ToString());
                    SetResult(Result.Ok, i);
                    Finish();
                }
            }
        }
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                if (etEmail.Text == "" || etPassword.Text == "")
                {
                    builder.SetMessage("Please fill all field..");
                    builder.SetPositiveButton("OK", OkCorrectAction);
                    builder.Create().Show();
                }
                else
                {
                    SettingsStringAutoComplete.UpdateAutocomplete(SettingName.Email, etEmail.Text, this);
                    Service.MemberService backGroundTask = new Service.MemberService(this);
                    backGroundTask.Execute("login", etEmail.Text, etPassword.Text);
                }
            }
            catch (Exception x)
            {
                Toast.MakeText(this, "Please check your internet connection.", ToastLength.Short);
            }
        }
        public Context GetContext()
        {
            return this;
        }
    }
}

