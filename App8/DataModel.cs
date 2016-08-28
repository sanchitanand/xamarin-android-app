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
using SQLite;
using Newtonsoft.Json;

namespace App8
{
    [Table("data")]
    public class DataModel
    {
        [PrimaryKey,Column("id")]
        public int id { get; set; }
        public string user { get; set; }
        public DateTime from { get; set; }
        public DateTime to { get; set; }
        public string employeeID { get; set; }
        public string mobileNumber { get; set; }
        [SQLite.Column("bill")]
        public string billNumber { get; set; }
        public string claimType { get; set; }
        public double maxAmount { get; set; }
        public double totalAmount { get; set; }
        public double totalService { get; set; }
        public double personalAmount { get; set; }
        public double personalService { get; set; }
        public double officialAmount { get; set; }
        public double officialService { get; set; }

        public string imageUri { get; set; }

        public override string ToString()
        {
            return String.Format("ID:{15},UserID:{0},FromDate:{1},ToDate:{2},EmployeeID:{3},Mobile Number:{4}\nBill Number:{5},Claim Type:{6},Max Amount:{7},Total Amount:{8}\nTotal Service Tax:{9},Personal Calls Amount:{10},Personal Service Tax:{11},Official Calls Amount:{12}\nOfficial Calls Service Tax:{13},Image URI:{14}\n",user,from.ToShortDateString(),to.ToShortDateString(),employeeID,mobileNumber,billNumber,claimType,maxAmount,totalAmount,totalService,personalAmount,personalService,officialAmount,officialService,imageUri,id);
        }
        
        public string serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}