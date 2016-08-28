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
    class TimerClass<T> where T:new()
    {
        private string classString;
        private System.Diagnostics.Stopwatch watch;
        public TimerClass()
        {
            watch = System.Diagnostics.Stopwatch.StartNew();
        }

        public void Start(string _class)
        {
            classString = _class;
            watch.Start();
        }

        public void Stop()
        {
            watch.Stop();
            System.Diagnostics.Debug.WriteLine(typeof(T).ToString()+":"+classString+":"+watch.ElapsedMilliseconds.ToString());
            
        }
        public void Reset()
        {
            watch.Reset();
        }


    }
}