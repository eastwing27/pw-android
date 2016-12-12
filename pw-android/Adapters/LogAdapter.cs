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

namespace PW.Android.Adapters
{
    public class LogAdapter : BaseAdapter<TransactDTO>
    {
        TransactDTO[] items;
        Activity context;
        public LogAdapter(Activity context, TransactDTO[] items)
       : base()
        {
            this.context = context;
            this.items = items;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override TransactDTO this[int position]
        {
            get { return items[position]; }
        }
        public override int Count
        {
            get { return items.Length; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = items[position];
            View view = convertView;

            if (view == null) 
                view = context.LayoutInflater.Inflate(Resource.Layout.LogViewRow, null);

            var body = item.SenderId == App.UserId ? 
                $"To {item.ReceiverName}. Balance: {item.SenderResult}":
                $"From {item.SenderName}. Balance: {item.ReceiverResult} ";
            view.FindViewById<TextView>(Resource.Id.Header).Text = $"{item.Amount:### ##0.00}pws at {item.TransactTime.ToLocalTime()}";
            view.FindViewById<TextView>(Resource.Id.Body).Text = body;
            //view.FindViewById<ImageView>(Resource.Id.Image).SetImageResource(item.ImageResourceId);
            return view;
        }
    }
}