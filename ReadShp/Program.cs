using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadShp
{
    class Program
    {
        static void Main(string[] args)
        {

            //0–3 int32 big File code(always hex value 0x0000270a)
            //4–23    int32 big Unused; five uint32
            //24–27   int32 big File length(in 16 - bit words, including the header)
            //28–31   int32 little  Version
            //32–35   int32 little  Shape type(see reference below)
            //36–67   double little  Minimum bounding rectangle(MBR) of all shapes contained within the dataset; four doubles in the following order: min X, min Y, max X, max Y
            //68–83   double little  Range of Z; two doubles in the following order: min Z, max Z
            //84–99   double little  Range of M; two doubles in the following order: min M, max M

            //            0–3
            //4–23
            //24–27
            //28–31
            //32–35
            //36–67
            //68–83
            //84–99



            var shp = @"C:\test\continents.shp";
            var bytes = File.ReadAllBytes(shp);
            var stream = new FileStream(@"C:\test\continents.shp", FileMode.Open);

            var fl = new ArraySegment<byte>(bytes, 24, 4).ToArray();
            var r0i = new ArraySegment<byte>(bytes, 104, 4).ToArray();

            //new StringBuilder()
            //var b0 = bytes[0];
            ;
        }
    }

    public class ShpFile
    {
        private const int recordsStartIndex = 100;
        public string Filepath { get; private set; }
        public ShpFile(string filepath)
        {
            this.Filepath = filepath;
        }

        public static ShpFile Open(string filepath)
        {
            return new ShpFile(filepath);
        }

        public int GetCount()
        {
            var bytes = File.ReadAllBytes(Filepath);
            var fileLength = bytes.Length;

            //FileStream fs = new FileStream(Filepath, FileMode.Open);
            //StreamReader streamReader = new StreamReader(fs);
            //streamReader.rea

            int count = 0;
            int cursor = recordsStartIndex;
            while (cursor < fileLength)
            {
                //var thisContentLength =           
                var bytesOfLength = new ArraySegment<byte>(bytes, cursor + 4, 4).Reverse().ToArray();
                var contentLength = BitConverter.ToInt32(bytesOfLength, 0) * 2;
                cursor += contentLength + 8;
                count++;
            }


            return count;



            //throw new NotImplementedException();
        }



    }

    public static class Utils
    {


    }

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
        public void ShapefileReadRecords()
        {
            var shp = new ShpFile(@"C:\test\KG_YD_DY_STATIC.shp");
            var count = shp.GetCount();
            Trace.WriteLine(count);
        }


    }
}
