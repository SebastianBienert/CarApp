using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Object = Java.Lang.Object;

namespace CarApp
{
    class DeviceListAdapter : BaseAdapter<BluetoothDevice>
    {
        private List<BluetoothDevice> devices;
        private Context context;
        public DeviceListAdapter(Context ctx, List<BluetoothDevice> data )
        {
            devices = data;
            context = ctx;
        }

        public override Object GetItem(int position)
        {
            return devices[position];
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if (row == null)
            {
                row = LayoutInflater.From(context).Inflate(Resource.Layout.DeviceRow, null, false);
            }

            TextView nameTextView = row.FindViewById<TextView>(Resource.Id.deviceNametxtView);
            TextView macTextView = row.FindViewById<TextView>(Resource.Id.mactxtView);

            nameTextView.Text = devices[position].Name;
            macTextView.Text = devices[position].Address;

            return row;
        }

        public override int Count { get { return devices.Count; } }

        public override BluetoothDevice this[int position]
        {
            get { return devices[position]; }
        }
    }
}