using Android.App;
using Android.Widget;
using Android.OS;
using Newtonsoft.Json;

namespace PW.Android
{
    [Activity(Label = "Parrot Wings", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);

            var btnLogin = FindViewById<Button>(Resource.Id.button1);
            btnLogin.Click += OnLoginClick;
        }

        private async void OnLoginClick(object sender, System.EventArgs e)
        {
            var edtServer = FindViewById<EditText>(Resource.Id.editText1);
            var edtEmail = FindViewById<EditText>(Resource.Id.editText2);
            var edtPassword = FindViewById<EditText>(Resource.Id.editText3);

            App.Initialize(edtServer.Text);

            var json = JsonConvert.SerializeObject(new
            {
                Email = edtEmail.Text,
                PasswordHash = edtPassword.Text.GetPasswordHash()
            });

            var loginSuccessful = await App.TryLogin(json);

            if (!loginSuccessful)
            {
                Toast.MakeText(this, App.Service.StatusMessage, ToastLength.Long);
                return;
            }

            Toast.MakeText(this, $"Welcome back, {App.UserName}!", ToastLength.Short);
            StartActivity(typeof(SendActivity));
        }
    }
}

