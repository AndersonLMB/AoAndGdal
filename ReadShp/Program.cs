using System;
using System.IO;
using System.Linq;
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



}
