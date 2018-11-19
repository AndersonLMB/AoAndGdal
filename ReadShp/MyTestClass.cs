using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ReadShp
{
    [TestClass]
    public class MyTestClass
    {
        [TestMethod]
        public void MyTestMethod()
        {
            var shp = @"C:\test\continents.shp";
            var bytes = File.ReadAllBytes(shp);
            var fileCode = BitConverter.ToInt32(new ArraySegment<byte>(bytes, 0, 4).Reverse().ToArray(), 0);
            var length = BitConverter.ToInt32(new ArraySegment<byte>(bytes, 24, 4).Reverse().ToArray(), 0);
            Trace.WriteLine(length);
        }

        [TestMethod]
        public void ShapefileReadRecordsCount()
        {
            var shp = new ShpFile(@"C:\test\continents.shp");
            var count = shp.GetCount();
            var boxStr = shp.GetBoxDoubleArr();
            Trace.WriteLine(count);
            var featureString = shp.GetFeatureString(0);
            Trace.WriteLine(featureString);
            var featureType = shp.GetFeatureType(2);
        }

        [TestMethod]
        public void ShapefileGetFeaturesTest()
        {
            var shp = new ShpFile(@"C:\test\continents.shp");
            var features = shp.GetFeatures();
        }

        [TestMethod]
        public void FSReaderTest()
        {
            Stream s = new MemoryStream();
            for (int i = 0; i < 122; i++)
            {
                s.WriteByte((byte)i);
            }
            s.Position = 0;

            // Now read s into a byte buffer with a little padding.
            byte[] bytes = new byte[s.Length + 10];
            int numBytesToRead = (int)s.Length;
            int numBytesRead = 0;
            do
            {
                // Read may return anything from 0 to 10.
                int n = s.Read(bytes, numBytesRead, 10);
                numBytesRead += n;
                numBytesToRead -= n;
            } while (numBytesToRead > 0);
            s.Close();

            Console.WriteLine("number of bytes read: {0:d}", numBytesRead);


            //fs.Position = 0;

        }


    }



}
