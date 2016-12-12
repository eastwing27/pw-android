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

namespace PW.Android
{
    [Activity(Label = "Transactions")]
    public class LogActivity : Activity
    {
        private ListView lstLog;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Log);

            var update = App.UpdateTransactions();
            update.Start(); update.Wait();

            if (App.Transactions != null)
            {
                lstLog = FindViewById<ListView>(Resource.Id.lstLog);
                var adapter = new Adapters.LogAdapter(this, App.Transactions);
                lstLog.Adapter = adapter;
                lstLog.ItemClick += OnListItemClick;
                Toast.MakeText(this, "Tap on any entry to use it as a template", ToastLength.Short);
            }
            else
                Toast.MakeText(this, App.Service.StatusMessage, ToastLength.Long);
        }

        void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var transact = App.Transactions[e.Position];

            var edtReceiver = FindViewById<EditText>(Resource.Id.edtReceiver);
            var edtAmount = FindViewById<EditText>(Resource.Id.edtAmount);

            edtReceiver.Text = (transact.SenderId == App.UserId) ? transact.ReceiverName : transact.SenderName;
            edtAmount.Text = transact.Amount.ToString();
            StartActivity(typeof(SendActivity));
        }
    }
}