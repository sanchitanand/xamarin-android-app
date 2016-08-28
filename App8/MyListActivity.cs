using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Akavache;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace App8
{
    [Activity(Label = "MyListActivity")]
    public class MyListActivity : ListActivity
    {
        public List<DataModel> entries=new List<DataModel>();
        private Toolbar toolbar;
        private string uid;
        private DatabaseHelper<DataModel> database;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            System.Diagnostics.Debug.WriteLine("OnCreate");
            base.OnCreate(savedInstanceState);
            uid = Intent.GetStringExtra("user");
            SetContentView(Resource.Layout.ListLayout);
            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "Entries";


       
            toolbar.FindViewById<ImageButton>(Resource.Id.addBtn).Click += addEvent;

        }

        protected override void OnStart()
        {
            base.OnStart();
            initializeListEntries();
            ListAdapter = new CustomAdapter(this, entries);
        }

        private void addEvent(object sender, EventArgs e)
        {
            CustomAdapter adapter= (CustomAdapter) this.ListAdapter;
            Intent intent = new Intent(this, typeof(MainActivity));
            intent.PutStringArrayListExtra("data",new List<string> {uid,adapter.data.Count.ToString() });
            StartActivity(intent);

            
        }

        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            CustomAdapter adapter = (CustomAdapter)l.Adapter;
            Intent intent = new Intent(this, typeof(MainActivity));
            intent.PutStringArrayListExtra("data", new List<string> { uid,adapter.data.Count.ToString(), adapter[position] });
            StartActivity(intent);
        }

        private void initializeListEntries()
        {
            string result = "";
            database = new DatabaseHelper<DataModel>(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) , "db_data.db");
            database.create();
            entries = database.queryAll("data");
            foreach (var item in entries)
            {
                result += item.ToString();
            }
            System.Diagnostics.Debug.WriteLine(result);
        }


        

    }
}