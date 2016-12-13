using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using System.Linq;

namespace PW.Android
{
    //Transactions log
    [Activity(Label = "Transactions")]
    public class LogActivity : Activity
    {
        private ListView lstLog;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Log);

            var header = FindViewById<TextView>(Resource.Id.txtLogHead);

            //Trying to update transactions data
            Toast.MakeText(this, "Please wait...", ToastLength.Short).Show();
            await App.UpdateTransactions();

            if (App.Transactions != null)
            {
                //Building list view if succeed
                lstLog = FindViewById<ListView>(Resource.Id.lstLog);
                var adapter = new Adapters.LogAdapter(this, App.Transactions);
                lstLog.Adapter = adapter;
                lstLog.ItemClick += OnListItemClick;
                header.Text = $"Total entries: {App.Transactions.Count()}";
                Toast.MakeText(this, "Tap on any entry to use it as a template", ToastLength.Short).Show();
            }
            else
            {
                header.Text = "Error loading transactions";
                Toast.MakeText(this, App.Service.StatusMessage, ToastLength.Long).Show();
            }

            FindViewById<Button>(Resource.Id.btnBack).Click += (sender, e) => StartActivity(typeof(SendActivity));
        }

        void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var transact = App.Transactions[e.Position];

            var edtReceiver = FindViewById<EditText>(Resource.Id.edtReceiver);
            var edtAmount = FindViewById<EditText>(Resource.Id.edtAmount);

            //Use sender/receiver name and transaction amount as a new transaction template
            var sendActivity = new Intent(this, typeof(SendActivity));
            sendActivity.PutExtra("Receiver", (transact.SenderId == App.UserId) ? transact.ReceiverName : transact.SenderName);
            sendActivity.PutExtra("Amount", transact.Amount.ToString());
            StartActivity(sendActivity);
        }
    }
}