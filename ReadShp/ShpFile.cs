using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ReadShp.Geometry;

namespace ReadShp
{
    public class ShpFile
    {
        public int[] RecordPositions { get; private set; } = new int[0];
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
            var recordPositions = GetRecordPositions();
            return recordPositions.Count();
        }
        private IEnumerable<int> GetRecordPositions()
        {
            using (var fileStream = new FileStream(Filepath, FileMode.Open))
            {
                var holeFileLength = fileStream.Length;
                int cursor = recordsStartIndex;
                fileStream.Position = 0;
                while (cursor < holeFileLength)
                {
                    fileStream.Position = cursor + 4;
                    byte[] bytesOfCurrentLenght = new byte[4];
                    fileStream.Read(bytesOfCurrentLenght, 0, 4);
                    var bytesOfLength = BitConverter.ToInt32(bytesOfCurrentLenght.Reverse().ToArray(), 0) * 2;

                    yield return cursor;
                    cursor += bytesOfLength + 8;
                }
            }
        }
        public string GetFeatureString(int index)
        {
            var featureType = GetFeatureType(index);


            var recordPositions = GetRecordPositions();
            var recordPosition = recordPositions.ToArray()[index];
            string contentString = String.Empty;
            byte[] bytesOfContent = new byte[0];
            using (var fs = new FileStream(this.Filepath, FileMode.Open))
            {
                var bytesOfContentLength = new byte[4];
                fs.Position = recordPosition + 4;
                fs.Read(bytesOfContentLength, 0, 4);
                var contentLength = BitConverter.ToInt32(bytesOfContentLength.Reverse().ToArray(), 0) * 2;
                //fs.Position += 4;
                bytesOfContent = new byte[contentLength];
                fs.Read(bytesOfContent, 0, contentLength);
                contentString = Encoding.UTF8.GetString(bytesOfContent);
            }

            switch (featureType)
            {
                case 5:
                    {

                        return Utils.PolygonBytesToFeatureString(bytesOfContent);
                        ;


                    }
                    break;
                default:
                    break;
            }

            return contentString;



            //return contentString;
            //return null;

            //throw new NotImplementedException();
        }

        public double[] GetBoxDoubleArr()
        {
            List<double> boxDoubles = new List<double>();

            using (var fileStream = new FileStream(Filepath, FileMode.Open))
            {
                fileStream.Position = 36;
                for (int i = 0; i < 4; i++)
                {

                    var bytesOfCurrentBoxDouble = new byte[8];
                    fileStream.Read(bytesOfCurrentBoxDouble, 0, 8);
                    var currentBoxDouble = BitConverter.ToDouble(bytesOfCurrentBoxDouble, 0);
                    boxDoubles.Add(currentBoxDouble);
                    //fileStream.Position += 8;
                }
            }

            var bytesOfWholeFile = File.ReadAllBytes(this.Filepath);

            return boxDoubles.ToArray();
            throw new NotImplementedException();
        }
        public int GetFeatureType(int index)
        {
            var recordPositions = GetRecordPositions();
            var recordPosition = recordPositions.ToArray()[index];
            var bytesOfFeatureType = new byte[4];
            int stringOfFeatureType;
            using (var fs = new FileStream(this.Filepath, FileMode.Open))
            {
                fs.Position = recordPosition + 8;
                fs.Read(bytesOfFeatureType, 0, 4);
                stringOfFeatureType = BitConverter.ToInt32(bytesOfFeatureType, 0);
            }
            return stringOfFeatureType;
            //throw new NotImplementedException();
        }

        public Point[] GetPoints()
        {
            throw new NotImplementedException();
        }


        public Feature[] GetFeatures()
        {
            var recordIndexes = GetRecordPositions();
            throw new NotImplementedException();





        }


    }



}
