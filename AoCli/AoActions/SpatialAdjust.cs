using ESRI.ArcGIS.EditorExt;
using ESRI.ArcGIS.Geometry;
using System;
using System.Linq;
using System.Collections;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AoCli
{
    public class SpatialAdjust
    {
        public SpatialAdjust(string controlPointsFile, ControlPointsInputType controlPointsInputType)
        {
            if (!File.Exists(controlPointsFile))
            {
                throw new FileNotFoundException("控制点文件没有找到");
            }
            this.ControlPointsFile = controlPointsFile;
            ControlPointsInputType = controlPointsInputType;
        }

        public string ControlPointsFile { get; set; } = @"C:\test\v2\cps.txt";
        public ControlPointsInputType ControlPointsInputType { get; set; }

        public void AdjustGeometry(IGeometry geometry)
        {
            Adjust(
                controlPointsFile: ControlPointsFile,
                controlPointsInputType: ControlPointsInputType,
                geometry: geometry
                );
        }

        public static void Adjust()
        {
            var txtStr = File.ReadAllText(@"C:\test\v2\cps.txt");
            var lines = txtStr.Split('\n').ToList();
            if (String.IsNullOrWhiteSpace(lines.Last()) || String.IsNullOrEmpty(lines.Last()))
            {
                lines.RemoveAt(lines.Count - 1);
            }
            List<IPoint> fromPoints = new List<IPoint>();
            List<IPoint> toPoints = new List<IPoint>();
            lines.ForEach((line) =>
            {
                var nums = line.Split('\t').ToList().Select(numString => Convert.ToDouble(numString)).ToList();
                IPoint fromPoint = new PointClass() { X = nums[0], Y = nums[1] };
                IPoint toPoint = new PointClass() { X = nums[2], Y = nums[3] };
                fromPoints.Add(fromPoint);
                toPoints.Add(toPoint);
            });
            ITransformationMethodGEN transformMethod = new AffineTransformationMethodClass();
            transformMethod.DefineFromControlPoints(fromPoints.ToArray(), toPoints.ToArray(), null, null);
            IPoint testPoint = new PointClass() { X = 14000000, Y = -5300000 };
            Console.WriteLine($"{testPoint.X} , {testPoint.Y}");
            //Task.Delay(2000).Wait();
            transformMethod.TransformShape(testPoint);
            Task.Delay(1000).Wait();


            Console.WriteLine($"{testPoint.X} , {testPoint.Y}");
            Task.Delay(1000).Wait();
        }


        public static void Adjust(string controlPointsFile, ControlPointsInputType controlPointsInputType, IGeometry geometry)
        {
            switch (controlPointsInputType)
            {
                case ControlPointsInputType.File:
                    {
                        var txtStr = File.ReadAllText(controlPointsFile);
                        var lines = txtStr.Split('\n').ToList();
                        if (String.IsNullOrWhiteSpace(lines.Last()) || String.IsNullOrEmpty(lines.Last()))
                        {
                            lines.RemoveAt(lines.Count - 1);
                        }
                        List<IPoint> fromPoints = new List<IPoint>();
                        List<IPoint> toPoints = new List<IPoint>();
                        lines.ForEach((line) =>
                        {
                            var nums = line.Split('\t').ToList().Select(numString => Convert.ToDouble(numString)).ToList();
                            IPoint fromPoint = new PointClass() { X = nums[0], Y = nums[1] };
                            IPoint toPoint = new PointClass() { X = nums[2], Y = nums[3] };
                            fromPoints.Add(fromPoint);
                            toPoints.Add(toPoint);
                        });

                        ITransformationMethodGEN transformMethod = new AffineTransformationMethodClass();
                        transformMethod.DefineFromControlPoints(fromPoints.ToArray(), toPoints.ToArray(), null, null);
                        transformMethod.TransformShape(geometry);
                    }
                    break;
                case ControlPointsInputType.Web:

                    throw new NotImplementedException();
                    break;
                default:
                    throw new NotImplementedException();
                    break;
            }
        }

    }



}
