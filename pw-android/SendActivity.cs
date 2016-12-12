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
using PW.DataTransciever.DTO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PW.Android
{
    [Activity(Label = "Parrot Wings")]
    public class SendActivity : Activity
    {
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Send);

            FindViewById<TextView>(Resource.Id.txtUserInfo).Text = App.UserInfo;

            await App.UpdateUserList();
            var receivers = App.Users
                .OrderBy(u => u.Name)
                .Select(u => u.Name)
                .ToArray();

            var btnSend = FindViewById<Button>(Resource.Id.btnSend);
            btnSend.Click += OnSendClick;

            var edtReceiver = FindViewById<AutoCompleteTextView>(Resource.Id.edtReceiver);
            var acAdapter = new ArrayAdapter(this, 17367050, receivers);
            edtReceiver.Adapter = acAdapter;

            var edtAmount = FindViewById<EditText>(Resource.Id.edtAmount);

            edtReceiver.Text = Intent.GetStringExtra("Receiver") ?? "";
            edtAmount.Text = Intent.GetStringExtra("Amount") ?? "";

            FindViewById<Button>(Resource.Id.btnLog).Click += (sender, e) => StartActivity(typeof(LogActivity));
        }

        private async void OnSendClick (object sender, EventArgs e)
        {
            var edtReceiver = FindViewById<TextView>(Resource.Id.edtReceiver);
            var receiver = App.Users.FirstOrDefault(u => u.Name == edtReceiver.Text);
            if (receiver == null)
            {
                Toast.MakeText(this, "Please enter a valid receiver name", ToastLength.Short).Show();
                return;
            }

            var edtAmount = FindViewById<EditText>(Resource.Id.edtAmount);
            if (string.IsNullOrEmpty(edtAmount.Text))
            {
                Toast.MakeText(this, "Please enter transaction amount", ToastLength.Short).Show();
                return;
            }
            double amount = Convert.ToDouble(edtAmount.Text);
            if(amount > App.Balance)
            {
                Toast.MakeText(this, "Sorry, you don't have enough PWs", ToastLength.Short).Show();
                return;
            }

            var json = JsonConvert.SerializeObject(new
            {
                SenderId = App.UserId,
                ReceiverId = receiver.Id,
                Amount = amount
            });

            var sendSuccessful = await App.Service.Send(json);

            if (!sendSuccessful)
            {
                Toast.MakeText(this, App.Service.StatusMessage, ToastLength.Short);
                return;
            }

            App.Balance -= amount;
            FindViewById<TextView>(Resource.Id.txtUserInfo).Text = App.UserInfo;
            Toast.MakeText(this, "Operation success!", ToastLength.Short).Show();
        }
    }
}