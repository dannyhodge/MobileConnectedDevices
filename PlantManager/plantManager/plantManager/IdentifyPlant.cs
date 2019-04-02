using System;
using System.IO;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Widget;
using System.Net.Http;
using System.Threading.Tasks;
using System.Json;

namespace plantManager
{

    [Activity(Label = "IdentifyPlant")]
    public class IdentifyPlant : Activity
    {

        string im64 = "";


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.identifyform);
            // Create your application here

            Button showCam = FindViewById<Button>(Resource.Id.takePhoto);
            Button identButton = FindViewById<Button>(Resource.Id.identifyButton);


            showCam.Click += delegate {
                Intent intent = new Intent(MediaStore.ActionImageCapture);
                StartActivityForResult(intent, 0);
            };


            identButton.Click += delegate {
                Console.WriteLine("Hello");



             };


        }



        

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            // It's a good idea that you check this before accessing the data
            if (requestCode == 0 && resultCode == Result.Ok)
            {
                //get the image bitmap from the intent extras
                var image = (Android.Graphics.Bitmap)data.Extras.Get("data");

                // you might also like to check whether image is null or not
                // if (image == null) do something

                //convert bitmap into byte array
                byte[] bitmapData;

               

                using (var stream = new MemoryStream())
                {
                    image.Compress(Android.Graphics.Bitmap.CompressFormat.Png, 0, stream);
                    bitmapData = stream.ToArray();
                }

                
                im64 = Convert.ToBase64String(bitmapData);


                var myImage = FindViewById<ImageView>(Resource.Id.imageView1);

                myImage.SetImageBitmap(image);

            }
            // if you got here something bad happened...
        }

    }
}