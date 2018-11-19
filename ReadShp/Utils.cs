using System;
using System.Linq;

namespace ReadShp
{
    public static class Utils
    {
        public static string PolygonBytesToFeatureString(byte[] polygonBytes)
        {
            var b0 = BitConverter.ToDouble(new ArraySegment<byte>(polygonBytes, 4, 8).ToArray(), 0);
            var b1 = BitConverter.ToDouble(new ArraySegment<byte>(polygonBytes, 12, 8).ToArray(), 0);
            var b2 = BitConverter.ToDouble(new ArraySegment<byte>(polygonBytes, 20, 8).ToArray(), 0);
            var b3 = BitConverter.ToDouble(new ArraySegment<byte>(polygonBytes, 28, 8).ToArray(), 0);
            //var arr0 = new ArraySegment<byte>(polygonBytes, 4, 8).ToArray();
            //var doub0 = BitConverter.ToDouble(arr0, 0);
            var numParts = BitConverter.ToInt32(new ArraySegment<byte>(polygonBytes, 36, 4).ToArray(), 0);
            var numPoints = BitConverter.ToInt32(new ArraySegment<byte>(polygonBytes, 40, 4).ToArray(), 0);
            var partIndexes = new int[numParts];
            for (int i = 0; i < numParts; i++)
            {
                //var partIndex= 
                var partIndex = BitConverter.ToInt32(new ArraySegment<byte>(polygonBytes, 44 + i * 4, 4).ToArray(), 0);
                partIndexes[i] = partIndex;
            }
            var polygonStartIndex = 44 + 4 * numParts;


            int startByteIndexOfAllPoints = 44 + numParts * 4;

            var bytesOfAllPoints = new ArraySegment<byte>(polygonBytes, startByteIndexOfAllPoints, polygonBytes.Length - startByteIndexOfAllPoints).ToArray();





            return null;
        }

    }



}
