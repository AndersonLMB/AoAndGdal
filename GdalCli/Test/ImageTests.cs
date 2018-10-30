using GdalCli.GdalImaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSGeo.OGR;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Forms.VisualStyles;
using System.Windows.Forms.Design;
using System.Collections.Generic;
using System.Net;
using System.Timers;

namespace GdalCli.Test
{
    [TestClass]
    public class ImageTests
    {
        [TestMethod]
        public void BmpTestMethod()
        {
            Ogr.RegisterAll();
            //var shpDriver = Ogr.GetDriverByName("ESRI Shapefile");
            //var shpDs = shpDriver.Open(@"C:\test\continents.shp", 0);

            var pgDriver = Ogr.GetDriverByName("PostgreSQL");
            var pgDs = pgDriver.Open(@"PG:dbname=postgis_24_sample host=localhost port=5432 user=postgres password=123", 0);
            var layerName = "tiger.continents";
            var layer = pgDs.GetLayerByName(layerName);

            //Drawer.DrawLayer(layer);
            var firstFeature = layer.Features().First();
            var feature = layer.Features().ToList()[2];

            Drawer.DrawGeometry(feature.GetGeometryRef());
        }

        [TestMethod]
        public void BasicBitmapGraphicsDrawTest()
        {
            var ms = new MemoryStream();
            Bitmap bitmap = new Bitmap(512, 512);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.DrawString("testing", new Font("Consolas", 12), new SolidBrush(Color.Yellow), 6, 6);
            }

            bitmap.Save(ms, ImageFormat.Png);
            var b64s = Convert.ToBase64String(ms.ToArray());
            Trace.WriteLine(String.Format(@"data:image/png;base64,{0}", b64s));
        }

        [TestMethod]
        public void MyTestMethod()
        {
            //Uri.UnescapeDataString("")
            var uds = Uri.UnescapeDataString("uon-we%2C");
            ;

            System.Drawing.FontFamily.Families.ToList().ForEach(fontFamily => Trace.WriteLine(fontFamily.Name));
            var variables = System.Environment.GetEnvironmentVariables();


            NotifyCollectionChangedEventHandler notifyCollectionChangedEventHandler = (a, b) =>
            {

            };


            foreach (var item in variables)
            {
                var de = ((DictionaryEntry)item);
                Trace.WriteLine(String.Format("{0} : {1}", de.Key, de.Value));
            }

        }

        [TestMethod]
        public void PopWin()
        {
            //MessageBox.Show("Hello World");
            FontDialog fontDialog = new FontDialog();
            DialogResult result = fontDialog.ShowDialog();
            Trace.WriteLine(fontDialog.Font.Size);
            Trace.WriteLine(result.ToString());
        }

        [TestMethod]
        public void LinqTest()
        {
            var collection = new List<int> { 1, 2, 3, 5, 7, 192 };
            IEnumerable<int> nc = from n in collection
                                  where n % 3 == 0
                                  select n - 2;
            ;
        }

        [TestMethod]
        public void WcTest()
        {
            WebClient webClient = new WebClient();
            var downTask = webClient.DownloadDataTaskAsync(new Uri("http://www.baidu.com"));
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 1.0;
            timer.Elapsed += (a, b) =>
            {
                Trace.WriteLine(String.Format("{0} {1}", b.SignalTime, downTask.Status));
            };
            timer.Enabled = true;

            downTask.Wait();
            Trace.WriteLine(downTask.Result.Length);



        }


        [TestMethod]
        public void LTest()
        {
            var lb = new List<bool> { };
            Trace.WriteLine(lb.Any(item => item == true));
            Trace.WriteLine(lb.All(item => item == true));
        }

        [TestMethod]
        public void GifExampleMethod()
        {
            var fs = new FileStream(@"C:\test\gife.gif", FileMode.Open);


            var byteArr = new byte[fs.Length];
            fs.Read(byteArr, 0, (int)(fs.Length));


            StreamReader streamReader = new StreamReader(fs);
            //var str = streamReader.ReadToEnd();

            var bm = Bitmap.FromStream(fs);
            var im = Image.FromStream(fs);

            var fdl = im.FrameDimensionsList;
            FrameDimension fd = new FrameDimension(im.FrameDimensionsList[0]);


            Trace.WriteLine(im.GetFrameCount(dimension: fd));




        }

    }





}
