using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
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


             identButton.Click += async delegate {
                Console.WriteLine("Hello");

                 await SendImage();

             };


        }


        private async Task SendImage()
        {

            var client = new HttpClient();
            client.BaseAddress = new Uri("https://api.plant.id/identify");

            string myKey = "Bc7dm4WGHOCu6Zj8Dh3ooKIJb5Lq9ocfPYlLHCrkGEHPZ5nb5C";

           

            string jsonData = @"{""key"" : ""Bc7dm4WGHOCu6Zj8Dh3ooKIJb5Lq9ocfPYlLHCrkGEHPZ5nb5C"", ""images"" : ""jHGVK11rYnhZezjxj298+DXRuAtUhZMzEaxEMIow0haqpCTssppM1fSIZ2u7n/jztd/ev2nOcfd/sZ727Rt3Vb32vm9cFcOqp8OOHVAQmQUAF7romXTNIW1UnKchpJLISmAS2JljbdVZnKsSErtbH8IRNT1mVDKgsYqb+yikvO2yiEKo00yOYWcfZiYJuSAnKJwovFNXaub60sUPAy1ADBGdUNOgqcpU0Eh2Dod4ugrm2Ox3seYAUAIwSyklAAAAFIBFtBaE4snV8+OjuYiTJfPnx2f3kH04WoosWeKWikC0ACktcXMSukYkm5NLsUrh4ggZNynrhrmC5aG3vvd9/7xk38SQu7TyDiMOI6qt6JkyErITEQxK6m1YO89qMBciDORYmiP5nvgX/DJq9wwHFSXgwTBSg8MRoqQsgKaFK/eOj67vzLNEdcnjx9/fuj7HJMSw8XzZ/tHX3v4V7/7yR/+QYGyDxmNDQRI6Jy6sWntrS68rFbTeJETsaZZ0bWqHjWzZ9cbzGm4GCH1S72YmgLQB8WRIWJmBJKASFarCUE5JQQYLQNmBDFtttqI1+qHl+3nLxTderDuDtvVUb3fb70ysrh+3xGB8U6jto3MMWFJOaVO7wkttCYBz47nKYV61Q7DxABDCmXCAiqnoKPS0vu6KZQUcggsiZ3VhxyGYUhxmvb7w3lZ3lrZajbJ/Nbi7hGs5WxDnFOIkYbdfuNaOwwHzAggEbGqqso3wRXfOOsqpToqTFRYGmb8lUSY8VdyYcB33nv78nr/5IsnxqrrnNjy1KXp0NXGKedf+erXdpefEjAReuv0r6wJkATrCGWMkzaepAIAqRX2vOsuvW8RZ1yp7/3+33jx8SfPP/sc41Y59ej1V995611TW0pCgXBOpxxJMFPRQrI1nr2S/HIbOH2wWFLxNP/Wqv+jHG+CACTgQJyQE5BQspvp7TC9vz47v9i07JRPtjLWrWa1629u9tsruHOU7jzEq+cCOCpKCEXl2DqP+OBqdwLmbuMvOncFe""}";

            try
            {
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("data", content);
                
                // this result string should be something like: "{"token":"rgh2ghgdsfds"}"
                string result = await response.Content.ReadAsStringAsync();

           //     Console.WriteLine("BASE64 STRING: " + im64);
                Console.WriteLine("RESULT HERE: " + result);
                Console.WriteLine("Cool");
            }
            catch (Exception er)
            {
                var lb = er.ToString();
                var js = "xs";
            }



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