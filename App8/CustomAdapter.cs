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

namespace App8
{
    class CustomAdapter : BaseAdapter<String>
    {
        public List<DataModel> data = new List<DataModel>();
        private Activity context;
        public CustomAdapter(Activity _context,List<DataModel> _data)
        {
            context = _context;
            data = _data;
        }
        public override string this[int position]
        {
            get
            {
                return data[position].id.ToString();
            }
        }

        public override int Count
        {
            get
            {try { return data.Count; }
              catch { return 0; }               
            }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView; // re-use an existing view, if one is available
            if (view == null) // otherwise create a new one
                view = context.LayoutInflater.Inflate(Resource.Layout.list_item, null);
            view.FindViewById<TextView>(Resource.Id.text1).Text = data[position].billNumber;
            return view;
        }
    }
}