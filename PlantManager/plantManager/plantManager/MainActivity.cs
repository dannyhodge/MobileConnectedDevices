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
using System.Json;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Android.Locations;
using Android.Util;
using Android;
using Android.Support.V4.App;
using Android.Content.PM;
using Android.Graphics;
using Xamarin.Forms;

namespace plantManager
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : Activity
    {

        Queue<double> temps = new Queue<double>();
        Queue<double> humids = new Queue<double>();
        List<Bitmap> images = new List<Bitmap>();

        int currentIndex = 0;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            // Create your application here
            // Set our view from the "weatherLayout" layout resource
            SetContentView(Resource.Layout.content_main);

            string url1 = "http://35.204.36.81/index3.php/sensors/getsensordata?sensorid=1";
            string url2 = "http://35.204.36.81/index3.php/sensors/getsensordata?sensorid=2";
            string url3 = "http://35.204.36.81/index3.php/images/getimagedata";


            // Fetch the weather information asynchronously using 'await'
            JsonValue jsonTemp = await FetchRestAPIAsync(url1);
            JsonValue jsonHumid = await FetchRestAPIAsync(url2);
            JsonValue jsonImages = await FetchRestAPIAsync(url3);
            ParseAndDisplay(jsonTemp, jsonHumid, jsonImages);

            TimeSpan interval = new TimeSpan(0, 0, 1);
           

            ChangeImage();

            Device.StartTimer(new TimeSpan(0, 0, 2), () =>
            {
                ChangeImage();
                return true;

            });




        }

        public void ChangeImage()
        {

            ImageView image = FindViewById<ImageView>(Resource.Id.imageView1);
            image.SetImageBitmap(images[currentIndex]);

            //   Console.WriteLine("current index: " + currentIndex);
            //    Console.WriteLine("images count: " + images.Count);
            if (currentIndex < (images.Count - 1))
            {
                currentIndex++;
            }
            else
            {
                currentIndex = 0;
            }



        }




        // Gets weather data from the passed URL.
        private async Task<JsonValue> FetchRestAPIAsync(string url)
        {
            // Create an HTTP web request using the URL:
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            request.ContentType = "application/json";
            request.Method = "GET";

            // Send the request to the server and wait for the response:
            using (WebResponse response = await request.GetResponseAsync())
            {
                // Get a stream representation of the HTTP web response:
                using (Stream stream = response.GetResponseStream())
                {
                    // Use this stream to build a JSON document object:
                    JsonValue jsonDoc = await Task.Run(() => JsonObject.Load(stream));
                    //   Console.Out.WriteLine("Response: {0}", jsonDoc.ToString());

                    // Return the JSON document:
                    return jsonDoc;
                }
            }
        }





        // Parse the weather data, then write temperature, humidity, 
        // conditions, and location to the screen.
        private void ParseAndDisplay(JsonValue jsonTemp, JsonValue jsonHumid, JsonValue jsonImages)
        {

            TextView currTemp = FindViewById<TextView>(Resource.Id.currentTemp);
            TextView avTemp = FindViewById<TextView>(Resource.Id.averageTemp);

            TextView maxTempBox = FindViewById<TextView>(Resource.Id.maxTemp);
            TextView minTempBox = FindViewById<TextView>(Resource.Id.minTemp);

            TextView currHumid = FindViewById<TextView>(Resource.Id.currentHumid);
            TextView avHumid = FindViewById<TextView>(Resource.Id.averageHumid);

            TextView maxHumidBox = FindViewById<TextView>(Resource.Id.maxHumid);
            TextView minHumidBox = FindViewById<TextView>(Resource.Id.minHumid);



            // Extract the array of name/value results for the field name "weatherObservation". 



            foreach (JsonValue j in jsonImages)
            {

                string image64 = j["image"];
                byte[] imageBytes = Convert.FromBase64String(image64);
                Bitmap im = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                images.Add(im);

            }




            foreach (JsonValue j in jsonTemp)
            {
                string theDate = j["time_stamp"];
                double temp = j["value"];

                string dateString = theDate;
                string tempString = temp.ToString();

                if (temps.Count < 60)
                {
                    temps.Enqueue(temp);
                }
                else if (temps.Count >= 60)
                {
                    temps.Enqueue(temp);
                    temps.Dequeue();
                }




            }
            int tempNum = temps.Count;
            double recentTemp = temps.ElementAt(tempNum - 1);
            string recenttempString = recentTemp.ToString();
            currTemp.Text = recenttempString;

            double allTemps = 0;
            double minTemp = 100000;
            double maxTemp = 0;

            foreach (double tempDouble in temps)
            {
                if (tempDouble > maxTemp)
                {
                    maxTemp = tempDouble;
                }
                if (tempDouble < minTemp)
                {
                    minTemp = tempDouble;
                }
                allTemps += tempDouble;

            }

            double averageTemp = allTemps / 60;
            int avTempInt = (int)averageTemp;
            string avtempString = avTempInt.ToString();
            avTemp.Text = avtempString;


            int maxTempInt = (int)maxTemp;
            string maxtempString = maxTempInt.ToString();
            maxTempBox.Text = maxtempString;


            int minTempInt = (int)minTemp;
            string mintempString = minTempInt.ToString();
            minTempBox.Text = mintempString;

            foreach (JsonValue j in jsonHumid)
            {
                string theDate = j["time_stamp"];
                double humid = j["value"];

                string dateString = theDate;
                string humidString = humid.ToString();

                if (humids.Count < 60)
                {
                    humids.Enqueue(humid);
                }
                else if (humids.Count >= 60)
                {
                    humids.Enqueue(humid);
                    humids.Dequeue();
                }

            }
            minHumidBox.Text = humids.Count.ToString();
            int humidNum = humids.Count;
            double recentHumid = humids.ElementAt(humidNum - 1);
            string recentHumidString = recentHumid.ToString();
            currHumid.Text = recentHumidString;

            double allHumids = 0;
            double minHumid = 100000;
            double maxHumid = 0;

            foreach (double humidDouble in humids)
            {
                if (humidDouble > maxHumid)
                {
                    maxHumid = humidDouble;
                }
                if (humidDouble < minHumid)
                {
                    minHumid = humidDouble;
                }
                allHumids += humidDouble;

            }

            double averageHumid = allHumids / 60;
            int avHumidInt = (int)averageHumid;
            string avHumidString = avHumidInt.ToString();
            avHumid.Text = avHumidString;


            int maxHumidInt = (int)maxHumid;
            string maxHumidString = maxHumidInt.ToString();
            maxHumidBox.Text = maxHumidString;


            int minHumidInt = (int)minHumid;
            string minHumidString = minHumidInt.ToString();
            minHumidBox.Text = minHumidString;





        }



    }
}