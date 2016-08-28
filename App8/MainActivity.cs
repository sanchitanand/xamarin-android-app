using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Text;
using Java.Lang;
using System.Collections.Generic;
using System.Reactive.Linq;
using Java.IO;
using Environment = Android.OS.Environment;
using Uri = Android.Net.Uri;
using Android.Graphics;
using Android.Provider;
using Android.Content.PM;
using SQLite;
using Exception = System.Exception;
using Android.Database;

namespace App8
{
    [Activity(Label = "Claims Form")]
    public class MainActivity : Activity
    {
        private EditText idText;
        private EditText mobileText;
        private EditText billText;
        private EditText fromText;
        private EditText toText;
        private EditText amountText;
        private EditText valueText;
        private EditText valueServiceText;
        private EditText personalText;
        private EditText officialText;
        private EditText personalServiceText;
        private EditText officialServiceText;
        private EditText totalApprovedText;
        private ImageButton fromButton;
        private ImageButton toButton;
        private ImageButton cameraButton;
        private ImageButton galleryButton;
        private ImageView imageCanvas;
        private Spinner claimSpinner;
        private Button submitButton;
        private ImageButton pdfButton;
        private double maxAmount = 1500;
        private DataModel datamodel=new DataModel();
        private DatabaseHelper<DataModel> database;
        private int type;
        private Uri uri;
        private TimerClass<MainActivity> timer=new TimerClass<MainActivity>();

        public string Selected { get; set; }

        public static class App
        {
            public static File _file;
            public static File _dir;
            public static Bitmap bitmap;

        }


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            IList<string> l=Intent.GetStringArrayListExtra("data");
            datamodel.user = l[0];

            foreach(var s in GetExternalFilesDirs(Environment.DirectoryPictures))
            {
                System.Diagnostics.Debug.WriteLine(s);
                System.Diagnostics.Debug.WriteLine(Environment.GetExternalStorageState(s));

            }

            try
            {
                File f = new File("/sdcard/mysdfile.txt");
                if (!f.Exists())
                    System.Diagnostics.Debug.WriteLine("Nope"); 
            }

            catch
            {
                System.Diagnostics.Debug.WriteLine("Nope");
            }

            
            // Set our view from the "main" layout resource
            
            SetContentView(Resource.Layout.Main);
            pdfButton = FindViewById<ImageButton>(Resource.Id.pdfButton);
            idText = FindViewById<EditText>(Resource.Id.idText);
            mobileText = FindViewById<EditText>(Resource.Id.mobileText);
            claimSpinner = FindViewById<Spinner>(Resource.Id.claimSpinner);
            billText = FindViewById<EditText>(Resource.Id.billText);
            fromText = FindViewById<EditText>(Resource.Id.fromText);
            toText = FindViewById<EditText>(Resource.Id.toText);
            amountText = FindViewById<EditText>(Resource.Id.amountText);
            valueText = FindViewById<EditText>(Resource.Id.valueText);
            valueServiceText = FindViewById<EditText>(Resource.Id.valueServiceText);
            personalText = FindViewById<EditText>(Resource.Id.personalText);
            personalServiceText = FindViewById<EditText>(Resource.Id.personalServiceText);
            officialText = FindViewById<EditText>(Resource.Id.officialText);
            officialServiceText = FindViewById<EditText>(Resource.Id.officialServiceText);
            totalApprovedText = FindViewById<EditText>(Resource.Id.totalApprovedText);
            fromButton = FindViewById<ImageButton>(Resource.Id.fromButton);
            toButton = FindViewById<ImageButton>(Resource.Id.toButton);
            cameraButton = FindViewById<ImageButton>(Resource.Id.cameraButton);
            galleryButton = FindViewById<ImageButton>(Resource.Id.galleryButton);
            imageCanvas = FindViewById<ImageView>(Resource.Id.imageCanvas);
            submitButton = FindViewById<Button>(Resource.Id.submitButton);

            CreateDirectoryForPictures();
            List<string> claimOptions = new List<string> { "mobile", "tablet", "router", "laptop" };
            claimSpinner.Adapter = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, claimOptions);
            database = new DatabaseHelper<DataModel>(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),"db_data.db");
            timer.Start("Creating db");
            string result = database.create();
            timer.Stop();
            timer.Reset();
            System.Diagnostics.Debug.WriteLine(result);

            fromButton.Click += fromButtonClicked;
            toButton.Click += toButtonClicked;
            pdfButton.Click += pdfButtonClicked;
            cameraButton.Click += TakeAPicture;
            galleryButton.Click += delegate {

                var imageIntent = new Intent(Intent.ActionOpenDocument);
                imageIntent.AddCategory(Intent.CategoryOpenable);
                imageIntent.SetType("image/*");
                //imageIntent.SetAction(Intent.ActionGetContent);
                //imageIntent.AddFlags();
                StartActivityForResult(
                    Intent.CreateChooser(imageIntent, "Select Picture"), 1);
            };
            submitButton.Click += submitData;
            if (l.Count == 3)
            {
                timer.Start("Initial db query");

                datamodel=database.query("data","id",l[2])[0];
                timer.Stop();
                timer.Reset();
                idText.Text = datamodel.employeeID;
                mobileText.Text = datamodel.mobileNumber;
                billText.Text = datamodel.billNumber;
                valueText.Text = datamodel.totalAmount.ToString();
                valueServiceText.Text = datamodel.totalService.ToString();
                personalText.Text = datamodel.personalAmount.ToString();
                personalServiceText.Text = datamodel.personalService.ToString();
                officialServiceText.Text = datamodel.officialService.ToString();
                amountText.Text = datamodel.maxAmount.ToString();
                officialText.Text = datamodel.officialAmount.ToString();
                fromText.Text = datamodel.from.ToShortDateString();
                toText.Text = datamodel.to.ToShortDateString();
               
                    try
                    {
                    string filepath = datamodel.imageUri;
                    if (".jpg .png .bmp .jpeg .gif .tif .tiff .jif".Contains(filepath.Substring(filepath.LastIndexOf("."))))
                        imageCanvas.SetImageURI(Uri.Parse(datamodel.imageUri));
                    else
                        imageCanvas.SetImageDrawable(GetDrawable(Resource.Drawable.pdf_placeholder));
                    }

                    catch(Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e.Message + e.ToString());
                        imageCanvas.SetImageDrawable(GetDrawable(Resource.Drawable.placeholder));
                    }
                    
                
                claimSpinner.SetSelection(claimOptions.IndexOf(datamodel.claimType));
            }

            else
            {
                datamodel.id = int.Parse(l[1]);
                valueText.Text = "0";
                valueServiceText.Text = "0";
                personalText.Text = "0";
                personalServiceText.Text = "0";
                officialServiceText.Text = "0";
                officialText.Text = "0";

                imageCanvas.SetImageDrawable(GetDrawable(Resource.Drawable.placeholder));

                amountText.Text = maxAmount.ToString();
            }
            
            

            
            
            
        }

        private void pdfButtonClicked(object sender, EventArgs e)
        {
            Intent intent = new Intent(Intent.ActionOpenDocument);
            intent.AddCategory(Intent.CategoryOpenable);
            intent.SetType("application/*");
            StartActivityForResult(Intent.CreateChooser(intent,"Select a document"),2);
            
        }

        private void submitData(object sender, EventArgs e)
        {
            datamodel.maxAmount = maxAmount;
            datamodel.employeeID = idText.Text;
            datamodel.mobileNumber = mobileText.Text;
            datamodel.billNumber = billText.Text;
            datamodel.officialAmount = double.Parse(officialText.Text);
            datamodel.officialService=double.Parse(officialServiceText.Text);
            datamodel.personalAmount = double.Parse(personalText.Text);
            datamodel.personalService=double.Parse(personalServiceText.Text);
            datamodel.totalAmount = double.Parse(valueText.Text);
            datamodel.totalService= double.Parse(valueServiceText.Text);
            AlertDialog.Builder alert = new AlertDialog.Builder(this);
            alert.SetTitle("Confirm form submission");
            alert.SetPositiveButton("Confirm", submitFunc);

            alert.SetNegativeButton("Cancel", (senderAlert, args) => {
               
            });

            Dialog dialog = alert.Create();
            dialog.Show();
          
            
        }

        private void submitFunc(object sender, DialogClickEventArgs e)
        {
            File dir;
            File dst;
            System.IO.Stream instream;
            System.IO.Stream outstream;
            byte[] buffer;
            int bytesread;
            switch(type)
            {
                case 0:
                    
                    break;
                case 1:
                     
                    dir = new File(getDirectory(), "User_"+datamodel.id.ToString());
                    if (!dir.Exists())
                        dir.Mkdirs();
                   

                    
                    dst = new File(dir, queryName(ContentResolver, uri));
                    instream = ContentResolver.OpenInputStream(uri);
                    outstream = ContentResolver.OpenOutputStream(Uri.FromFile(dst));
                    timer.Start("File transfer");
                    buffer = new byte[2048];
                    bytesread = 0;
                    while ((bytesread = instream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        outstream.Write(buffer, 0, bytesread);
                    }
                    timer.Stop();
                    timer.Reset();
                    instream.Close();
                    outstream.Close();

                    datamodel.imageUri = Uri.FromFile(dst).ToString();
                    
                    break;
                case 2:
                    dir = new File(getDirectory(), "User_"+datamodel.id.ToString());
                    if (!dir.Exists())
                        dir.Mkdirs();


                    dst = new File(dir, queryName(ContentResolver, uri));
                    instream = ContentResolver.OpenInputStream(uri);
                    outstream = ContentResolver.OpenOutputStream(Uri.FromFile(dst));
                    timer.Start("File transfer");
                    buffer = new byte[2048];
                    bytesread = 0;
                    while ((bytesread = instream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        outstream.Write(buffer, 0, bytesread);
                    }
                    timer.Stop();
                    timer.Reset();
                    instream.Close();
                    outstream.Close();

                    datamodel.imageUri = Uri.FromFile(dst).ToString();
                    break;
                
            }
           
            idText.Text = "";
            mobileText.Text = "";
            billText.Text = "";
            valueText.Text = "0";
            valueServiceText.Text = "0";
            personalText.Text = "0";
            personalServiceText.Text = "0";
            officialServiceText.Text = "0";
            officialText.Text = "0";
            fromText.Text = "";
            toText.Text = "";
            imageCanvas.SetImageDrawable(GetDrawable(Resource.Drawable.placeholder));
            //convertImage(datamodel.imageUri);
            timer.Start("Storing db entries");
            string result = database.insert(datamodel);
            timer.Stop();
            timer.Reset();
            System.Diagnostics.Debug.WriteLine(result);
        }

        private void toButtonClicked(object sender, EventArgs e)
        {
            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
            {
                toText.Text = time.ToShortDateString();
                datamodel.to = time;
            });

            frag.Show(FragmentManager, DatePickerFragment.TAG);
        }

        private void fromButtonClicked(object sender, EventArgs e)
        {
            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
            {
                fromText.Text = time.ToShortDateString();
                datamodel.from = time;
            });

            frag.Show(FragmentManager, DatePickerFragment.TAG);
        }

        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            datamodel.claimType = (string)spinner.GetItemAtPosition(e.Position);
        }

      
     

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        private void CreateDirectoryForPictures()
        {
            App._dir = new File(getDirectory(),"User_"+datamodel.id.ToString());
            if (!App._dir.Exists())
            {
                App._dir.Mkdirs();
            }
        }

        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities =
                PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }

        private void TakeAPicture(object sender, EventArgs eventArgs)
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            App._file = new File(App._dir, System.String.Format("Claim_{0}.jpg", Guid.NewGuid()));
            intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(App._file));
            StartActivityForResult(intent, 0);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            // Make it available in the gallery
            if (requestCode == (int)0)
            {
                type = 0;
                //Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                uri = Uri.FromFile(App._file);
                datamodel.imageUri = uri.ToString();
                imageCanvas.SetImageURI(Uri.Parse(datamodel.imageUri));
                //mediaScanIntent.SetData(uri);
                //SendBroadcast(mediaScanIntent);
                //File dir = GetDir("ClaimsFormAttachments", FileCreationMode.Private);
                //if (!dir.Exists())
                //    dir.Mkdirs();
                //filepath = uri.Path;

                //System.Diagnostics.Debug.WriteLine(string.Format("filepath:{0}", filepath));
                //File src = App._file;
                //File dst = new File(dir, src.Name);
                //var instream = ContentResolver.OpenInputStream(uri);
                //var outstream = ContentResolver.OpenOutputStream(Uri.FromFile(dst));
                //byte[] buffer = new byte[2048];
                //int bytesread = 0;
                //while ((bytesread = instream.Read(buffer, 0, buffer.Length)) > 0)
                //{
                //    outstream.Write(buffer, 0, bytesread);
                //}
                //instream.Close();
                //outstream.Close();

                //datamodel.imageUri = Uri.FromFile(dst).ToString();


            }


            // Display in ImageView. We will resize the bitmap to fit the display.
            // Loading the full sized image will consume to much memory
            // and cause the application to crash.

            //int height = Resources.DisplayMetrics.HeightPixels;
            // int width = imageCanvas.Width;
            //App.bitmap = App._file.Path.LoadAndResizeBitmap(width, height);
            //if (App.bitmap != null)
            // {
            //     imageCanvas.SetImageBitmap(App.bitmap);
            //    App.bitmap = null;
            // }



            // Dispose of the Java side bitmap.
            //GC.Collect();


            if (requestCode == (int)1)
            {
                type = 1;
                uri = data.Data;
                imageCanvas.SetImageURI(uri);

                // File dir = GetDir("ClaimsFormAttachments", FileCreationMode.Private);


            }

            if(requestCode==2)
            {
                type = 2;
                uri = data.Data;
                imageCanvas.SetImageDrawable(GetDrawable(Resource.Drawable.pdf_placeholder));




            }

        }

       

        

        private string GetRealPathFromURI(Uri contentURI)
        {
            ICursor cursor = ContentResolver.Query(contentURI, null, null, null, null);
            cursor.MoveToFirst();
            string documentId = cursor.GetString(0);
            documentId = documentId.Split(':')[1];
            cursor.Close();

            cursor = ContentResolver.Query(
            Android.Provider.MediaStore.Images.Media.ExternalContentUri,
            null, MediaStore.Images.Media.InterfaceConsts.Id + " = ? ", new[] { documentId }, null);
            cursor.MoveToFirst();
            string path = cursor.GetString(cursor.GetColumnIndex(MediaStore.Images.Media.InterfaceConsts.Data));
            cursor.Close();

            return path;
        }


      

        public File getDirectory()
        {
            return Environment.GetExternalStoragePublicDirectory(Environment.DirectoryDocuments+"/ClaimsForm");
        }

        private string queryName(ContentResolver resolver, Uri uri)
        {
            if (uri.Scheme != ContentResolver.SchemeContent )
                return uri.ToString();
            ICursor returnCursor =
                    resolver.Query(uri, null, null, null, null);
            int nameIndex = returnCursor.GetColumnIndex(OpenableColumns.DisplayName);
            returnCursor.MoveToFirst();
            string name = returnCursor.GetString(nameIndex);
            returnCursor.Close();
            return name;
        }



    }


}
