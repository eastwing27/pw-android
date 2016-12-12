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
using Newtonsoft.Json;

namespace PW.Android
{
    [Activity(Label = "Register")]
    public class RegActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Register);

            FindViewById<TextView>(Resource.Id.edtRegServer).Text = Intent.GetStringExtra("Server") ?? "http://pwings.azurewebsites.net";
            FindViewById<Button>(Resource.Id.btnRegister).Click += OnRegisterClick;
            FindViewById<Button>(Resource.Id.btnBackToLogin).Click += (sender, e) => StartActivity(typeof(MainActivity));
        }

        private async void OnRegisterClick(object sender, EventArgs e)
        {
            var edtName = FindViewById<EditText>(Resource.Id.edtRegName);
            var edtEmail = FindViewById<EditText>(Resource.Id.edtRegEmail);
            var edtPassword = FindViewById<EditText>(Resource.Id.edtRegPassword);
            var edtConfirm = FindViewById<EditText>(Resource.Id.edtRegConfirm);

            const string nameSymbols = " abcdefghigklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            if (!edtName.Text.All(c => nameSymbols.Contains(c)) 
                || edtName.Text.Split(' ').Length != 2
                || string.IsNullOrEmpty(edtName.Text))
            {
                Toast.MakeText(this, "Please enter valid name in \"Firstname Lastname\" format contains latin letters only", ToastLength.Short).Show();
                return;
            }

            if (string.IsNullOrEmpty(edtEmail.Text) 
                || !edtEmail.Text.Contains("@") 
                || !edtEmail.Text.Contains(".") 
                || edtEmail.Text.Contains(" "))
            {
                Toast.MakeText(this, "Please enter a valid e-mail", ToastLength.Short).Show();
                return;
            }

            if (string.IsNullOrEmpty(edtPassword.Text))
            {
                Toast.MakeText(this, "Passwod cannot be empty", ToastLength.Short).Show();
                return;
            }

            if (edtPassword.Text != edtConfirm.Text)
            {
                Toast.MakeText(this, "Passwods does not match!", ToastLength.Short).Show();
                return;
            }

            var json = JsonConvert.SerializeObject(new
            {
                Name = edtName.Text,
                Email = edtEmail.Text,
                PasswordHash = edtPassword.Text.GetPasswordHash()
            });

            var registerSuccessful = await App.TryRegister(json);
            if (!registerSuccessful)
            {
                Toast.MakeText(this, $"Registration error: {App.Service.StatusMessage}", ToastLength.Short).Show();
                return;
            }

            Toast.MakeText(this, $"Welcome!", ToastLength.Short).Show();
            StartActivity(typeof(SendActivity));
        }
    }
}