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
using MrGo.Entity;
using Android.Content.PM;
using MrGo.Service;

namespace MrGo
{
    [Activity(Label = "User Sign Up", LaunchMode = LaunchMode.SingleTop, Icon = "@drawable/ic_launcher")]
    public class SignUpActivity : Activity,ISetMemberActivity
    {
        EditText etName, etEmail, etPhone,  etPassword, etCPassword;
        Button btnSignUp;
        AlertDialog.Builder builder;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SignUpScreen);
            builder = new AlertDialog.Builder(this);
            etName = (EditText)FindViewById(Resource.Id.etName);
            etEmail = (EditText)FindViewById(Resource.Id.etEmail);
            etPhone = (EditText)FindViewById(Resource.Id.etPhone);
            etPassword = (EditText)FindViewById(Resource.Id.etPassword);
            etCPassword = (EditText)FindViewById(Resource.Id.etConfirmPassword);
            btnSignUp = (Button)FindViewById(Resource.Id.btnSignUp);
            btnSignUp.Click += btnSignUp_Click;
        }
        private void OkWrongAction(object sender, DialogClickEventArgs e)
        {

        }
        private void OkCorrectAction(object sender, DialogClickEventArgs e)
        {
            etPassword.Text = "";
            etCPassword.Text = "";
        }
        private void btnSignUp_Click(object sender, EventArgs e)
        {
            if (etName.Text.Equals("") ||
                        etEmail.Text.Equals("") ||
                        etPassword.Text.Equals("")
                        )
            {
                builder = new AlertDialog.Builder(this);
                builder.SetTitle("Something went wrong...");
                builder.SetMessage("Fill all field...");
                builder.SetPositiveButton("OK", OkWrongAction);
                AlertDialog alert = builder.Create();
                alert.Show();
            }
            else if (!etPassword.Text.Equals(etCPassword.Text))
            {
                builder = new AlertDialog.Builder(this);
                builder.SetTitle("Something went wrong...");
                builder.SetMessage("Password are not matching...");
                builder.SetPositiveButton("OK", OkCorrectAction);
                AlertDialog alert = builder.Create();
                alert.Show();
            }
            else
            {
                Service.MemberService backGroundTask = new Service.MemberService(this);
                backGroundTask.Execute("getbyemail", etEmail.Text);
            }
        }
        public void SetMemberActivity(string key, Member member)
        {
            if (!CommonService.CheckInternetConnection(this)) { Toast.MakeText(this, "Please check your internet connection", ToastLength.Short).Show(); return; }
            if (key == "getbyemail")
            {
                if (member != null)
                {
                    builder = new AlertDialog.Builder(this);
                    builder.SetTitle("Something went wrong...");
                    builder.SetMessage("Email already registered.");
                    builder.SetPositiveButton("OK", OkCorrectAction);
                    AlertDialog alert = builder.Create();
                    alert.Show();
                }
                else
                {
                    Service.MemberService backGroundTask = new Service.MemberService(this);
                    backGroundTask.Execute("register", etName.Text, etPhone.Text, etEmail.Text, etCPassword.Text);
                }
            }
            if (key == "register")
            {
                Intent i = new Intent(this, typeof(ConfirmationActivity));
                i.PutExtra("memberEmail", etEmail.Text);
                i.PutExtra("memberPhone", etPhone.Text);
                this.StartActivityForResult(i,1);
                //Finish();
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
        public Context GetContext()
        {
            return this;
        }
    }
}