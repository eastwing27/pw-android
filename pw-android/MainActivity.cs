using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Newtonsoft.Json;
using System;

namespace PW.Android
{
    //Here - login page
    [Activity(Label = "Parrot Wings", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView (Resource.Layout.Main);

            //Trying to get some stored values
            var prefs = Application.Context.GetSharedPreferences("PW", FileCreationMode.Private);
            var server = prefs.GetString("Server", "http://pwings.azurewebsites.net");
            var email = prefs.GetString("Email", "");

            FindViewById<EditText>(Resource.Id.editText1).Text = server;
            FindViewById<EditText>(Resource.Id.editText2).Text = email;

            var btnLogin = FindViewById<Button>(Resource.Id.button1);
            btnLogin.Click += OnLoginClick;

            FindViewById<Button>(Resource.Id.button2).Click += OnRegClick;
        }

        /// <summary>
        /// Open registration page and send it server address if present
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnRegClick(object sender, EventArgs e)
        {
            var regActivity = new Intent(this, typeof(RegActivity));
            regActivity.PutExtra("Server", FindViewById<EditText>(Resource.Id.editText1).Text);
            StartActivity(regActivity);
        }

        private async void OnLoginClick(object sender, EventArgs e)
        {
            var edtServer = FindViewById<EditText>(Resource.Id.editText1);
            var edtEmail = FindViewById<EditText>(Resource.Id.editText2);
            var edtPassword = FindViewById<EditText>(Resource.Id.editText3);

            App.Initialize(edtServer.Text);

            //Creating anonymous DTO and serializing it
            var json = JsonConvert.SerializeObject(new
            {
                Email = edtEmail.Text,
                PasswordHash = edtPassword.Text.GetPasswordHash()
            });

            //Trying to authorize user on server
            Toast.MakeText(this, "Please wait...", ToastLength.Long).Show();
            var loginSuccessful = await App.TryLogin(json);

            if (!loginSuccessful)
            {
                Toast.MakeText(this, App.Service.StatusMessage, ToastLength.Long).Show();
                return;
            }

            //Is login successful store server name and e-mail
            var prefs = Application.Context.GetSharedPreferences("PW", FileCreationMode.Private);
            var prefEditor = prefs.Edit();
            prefEditor.PutString("Server", edtServer.Text);
            prefEditor.PutString("Email", edtEmail.Text);
            prefEditor.Commit();

            Toast.MakeText(this, $"Welcome back, {App.UserName}!", ToastLength.Short).Show();
            StartActivity(typeof(SendActivity));
        }
    }
}

