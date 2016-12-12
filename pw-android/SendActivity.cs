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
        private ReceiverDTO receiver = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Send);

            FindViewById<TextView>(Resource.Id.txtUserInfo).Text = App.UserInfo;

            var updateUsers = App.UpdateUserList();
            updateUsers.Start(); updateUsers.Wait();
            var receivers = App.Users.Select(u => u.Name).ToArray();

            var edtReceiver = FindViewById<AutoCompleteTextView>(Resource.Id.edtReceiver);
            var acAdapter = new ArrayAdapter(this, Resource.Layout.Send, receivers);
            edtReceiver.Adapter = acAdapter;
            edtReceiver.ItemSelected += OnReceiverSelect;
        }

        private void OnReceiverSelect(object sender, EventArgs e)
        {
            receiver = App.Users.FirstOrDefault(u => u.Name == e.ToString());
        }

        private async void OnSendClick (object sender, EventArgs e)
        {
            if (receiver == null)
            {
                Toast.MakeText(this, "Please enter a valid receiver name", ToastLength.Short);
                return;
            }

            var edtAmount = FindViewById<EditText>(Resource.Id.edtReceiver);
            if (string.IsNullOrEmpty(edtAmount.Text))
            {
                Toast.MakeText(this, "Please enter transaction amount", ToastLength.Short);
                return;
            }
            double amount = Convert.ToDouble(edtAmount.Text);
            if(amount > App.Balance)
            {
                Toast.MakeText(this, "Sorry, you don't have enough PWs", ToastLength.Short);
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
            Toast.MakeText(this, "Operation success!", ToastLength.Short);
        }
    }
}