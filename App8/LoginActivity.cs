using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Akavache;
using SQLite;
using System.IO;
using Environment = Android.OS.Environment;
using System.Threading.Tasks;

namespace App8
{
    [Activity(Label = "Login", MainLauncher = true, Icon = "@drawable/icon")]
    public class LoginActivity : Activity
    {
        private EditText userText;
        private EditText passwordText;
        private Button loginButton;
        private UserModel user;
        private DatabaseHelper<UserModel> database;
        protected override void OnDestroy()
        {
            System.Diagnostics.Debug.WriteLine("Login:OnDestroy");
            base.OnDestroy();
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            
            SetContentView(Resource.Layout.Login);
            userText = FindViewById<EditText>(Resource.Id.userText);
            passwordText = FindViewById<EditText>(Resource.Id.passwordText);
            loginButton = FindViewById<Button>(Resource.Id.loginButton);
            loginButton.Click += loginButtonClick;

            string result = "";
            database = new DatabaseHelper<UserModel>(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "dbdemo.db");
            result = database.create();
            System.Diagnostics.Debug.WriteLine(result);


        }

        private void loginButtonClick(object sender, EventArgs ex)
        {
            string result = "";
            
            user = new UserModel { username = userText.Text, password = passwordText.Text };
            List<UserModel> results = database.query("user","username",user.username);

            foreach (var item in results)
            {
                result += item.ToString();
            }
            System.Diagnostics.Debug.WriteLine(result);

            if (results.Count==0)
            {
                result = database.insert(user);
                System.Diagnostics.Debug.WriteLine(result);


                Intent intent = new Intent(this, typeof(MyListActivity));
                intent.PutExtra("user", user.username);
                StartActivity(intent);

            }

            else
            {
                if(String.Equals(user.password,results[0].password))
                {
                    Intent intent = new Intent(this, typeof(MyListActivity));
                    intent.PutExtra("user", user.username);
                    StartActivity(intent);
                }

                else
                {
                    AlertDialog.Builder builder = new AlertDialog.Builder(this);
                    builder.SetMessage("Incorrect credentials");
                    builder.SetNeutralButton("Ok",delegate { });
                    builder.Create().Show();

                }

            }
            
            

            


        }


       
    }

}
