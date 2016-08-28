using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;

namespace App8
{
    [Table("user")]
    public class UserModel
    {
        [Unique]
       public string username { get; set; }
       public string password { get; set; }

        public override string ToString()
        {
            return String.Format("[Username:{0},Password:{1}]",username,password);
        }

    }
}