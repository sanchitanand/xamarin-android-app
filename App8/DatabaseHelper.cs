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

namespace App8
{
    class DatabaseHelper<T> where T:new()
    {
        
        private string docpath;
        private string dbpath;

        public DatabaseHelper(string doc,string db)
            {
            docpath = doc;
            dbpath = db;

            }
        public string create()
        {
            try
            {
                var connection = new SQLite.SQLiteConnection(System.IO.Path.Combine(docpath,dbpath));
                connection.CreateTable<T>();
                return "Database Created";
            }
            catch (Exception ex)
            {
                return ex.Message + ex.ToString();
            }
        }


        public string insert(T user)
        {
            try
            {
                var connection = new SQLite.SQLiteConnection(System.IO.Path.Combine(docpath, dbpath));

                if(connection.Insert(user)==-1)
                    connection.Update(user);

                return "inserted/updated entry";
            }
            catch (Exception ex)
            {
                try
                {
                    var connection = new SQLite.SQLiteConnection(System.IO.Path.Combine(docpath, dbpath));
                        connection.Update(user);

                    return "updated entry";

                }
                catch (Exception e)
                {
                    return e.Message + e.ToString();
                }
            }
        }

        public List<T> query(string table, string column, string query) 
        {
            try
            {
                var connection = new SQLite.SQLiteConnection(System.IO.Path.Combine(docpath, dbpath));

                var results = connection.Query<T>(string.Format("select * from {0} where {1}='{2}'", table, column, query));


                return results;
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message + ex.ToString());
                return new List<T>();
            }
        }

        public string deleteAll()
        {
            try
            {
                var connection = new SQLite.SQLiteConnection(System.IO.Path.Combine(docpath, dbpath));

                connection.DeleteAll<T>();
                return "Deleted data";
            }

            catch (Exception ex)
            {
                return ex.Message + ex.ToString();
            }
        }

        public List<T> queryAll(string table)
        {
            try
            {
                var connection = new SQLite.SQLiteConnection(System.IO.Path.Combine(docpath, dbpath));
                return connection.Query<T>(String.Format("select * from {0}", table));
            }

            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return new List<T>();
            }
        }

        public string delete(string table, string column, string query)
        {
            try
            {
                var connection = new SQLite.SQLiteConnection(System.IO.Path.Combine(docpath, dbpath));
                connection.Execute(String.Format("delete from {0} where {1}={2}",table,column,query));
                return "Entry deleted ";
            }

            catch(Exception ex)
            {

                return ex.Message + ex.ToString();
            }
        }


        public string insertAll(List<T> users)
        {
            try
            {
                var connection = new SQLite.SQLiteConnection(System.IO.Path.Combine(docpath, dbpath));
                connection.InsertAll(users);
                return "Entries inserted";
            }

            catch(Exception ex)
            {
                return ex.Message + ex.ToString();
            }
        }
       
    }
}