using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Newtonsoft.Json;
using System;
using System.Linq;

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

            //Selecting usernames for autocompleteon list
            await App.UpdateUserList();
            var receivers = App.Users
                .OrderBy(u => u.Name)
                .Select(u => u.Name)
                .ToArray();

            var btnSend = FindViewById<Button>(Resource.Id.btnSend);
            btnSend.Click += OnSendClick;

            //Attaching username list
            var edtReceiver = FindViewById<AutoCompleteTextView>(Resource.Id.edtReceiver);
            var acAdapter = new ArrayAdapter(this, 17367050, receivers);
            edtReceiver.Adapter = acAdapter;

            var edtAmount = FindViewById<EditText>(Resource.Id.edtAmount);

            //Trying get data from a "template" (if user taps log entry)
            edtReceiver.Text = Intent.GetStringExtra("Receiver") ?? "";
            edtAmount.Text = Intent.GetStringExtra("Amount") ?? "";

            FindViewById<Button>(Resource.Id.btnLog).Click += (sender, e) => StartActivity(typeof(LogActivity));
        }

        private async void OnSendClick (object sender, EventArgs e)
        {
            var edtReceiver = FindViewById<TextView>(Resource.Id.edtReceiver);

            //Check if user enters correct receiver name
            var receiver = App.Users.FirstOrDefault(u => u.Name == edtReceiver.Text);
            if (receiver == null)
            {
                Toast.MakeText(this, "Please enter a valid receiver name", ToastLength.Short).Show();
                return;
            }

            //Validation transaction amount
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

            //Creation anonymous DTO and its serialization
            var json = JsonConvert.SerializeObject(new
            {
                SenderId = App.UserId,
                ReceiverId = receiver.Id,
                Amount = amount
            });

            //Trying to make a transaction
            Toast.MakeText(this, "Please wait...", ToastLength.Short).Show();
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