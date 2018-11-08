using ESRI.ArcGIS.EditorExt;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
namespace AoCli
{
    public class SpatialAdjust
    {
        #region Constructors

        public SpatialAdjust(string controlPointsFile, ControlPointsInputType controlPointsInputType, SpatialAdjustMethodType spatialAdjustMethodType)
        {
            ControlPointsFile = controlPointsFile;
            ControlPointsInputType = controlPointsInputType;
            SpatialAdjustMethodType = spatialAdjustMethodType;
            string txt = string.Empty;
            switch (ControlPointsInputType)
            {
                case ControlPointsInputType.File:
                    try
                    {
                        txt = File.ReadAllText(ControlPointsFile);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("找不到控制点文件");
                        throw ex;
                    }
                    break;

                case ControlPointsInputType.Web:
                    try
                    {
                        txt = new WebClient().DownloadString(ControlPointsFile);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("找不到控制点文件");
                        throw ex;
                    }
                    break;

                default:
                    break;
            }
            if (String.IsNullOrEmpty(txt))
            {
                TransformationMethod = null;
            }
            else
            {
                var methodObject = transformMethodMap[SpatialAdjustMethodType]
                    .GetConstructors()
                    .Where(constructor => constructor.GetParameters().Length == 0)
                    .FirstOrDefault()
                    .Invoke(null);
                TransformationMethod = (ITransformationMethodGEN)methodObject;

                #region 定义控制点

                var lines = txt.Split('\n').ToList();
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
                TransformationMethod.DefineFromControlPoints(fromPoints.ToArray(), toPoints.ToArray(), null, null);

                #endregion 定义控制点
            }
        }

        public SpatialAdjust()
        {
        }

        #endregion Constructors

        #region Properties

        public string ControlPointsFile { get; set; } = @"C:\test\v2\cps.txt";
        public ControlPointsInputType ControlPointsInputType { get; set; }
        public SpatialAdjustMethodType SpatialAdjustMethodType { get; set; }
        public ITransformationMethodGEN TransformationMethod { get; set; }

        #endregion Properties

        #region Fields

        public static Dictionary<SpatialAdjustMethodType, Type> transformMethodMap = new Dictionary<SpatialAdjustMethodType, Type>()
        {
            { SpatialAdjustMethodType.Affine ,  typeof(AffineTransformationMethodClass) },
            { SpatialAdjustMethodType.Conformal ,  typeof(ConformalTransformationMethodClass) },
            { SpatialAdjustMethodType.EdgeSnap ,  typeof(EdgeSnapTransformationMethodClass) },
            { SpatialAdjustMethodType.Piecewise ,  typeof(PiecewiseTransformationClass) },
            { SpatialAdjustMethodType.Projective ,  typeof(ProjectiveTransformationMethodClass) }
        };

        #endregion Fields

        public void AdjustGeometry(IGeometry geometry)
        {
            //Adjust(
            //    controlPointsFile: ControlPointsFile,
            //    controlPointsInputType: ControlPointsInputType,
            //    geometry: geometry,
            //    spatialAdjustMethodType: SpatialAdjustMethodType
            //    );
            Adjust(geometry, TransformationMethod);
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

        /// <summary>
        ///
        /// </summary>
        /// <param name="controlPointsFile"></param>
        /// <param name="controlPointsInputType"></param>
        /// <param name="geometry"></param>
        /// <param name="spatialAdjustMethodType"></param>
        /// <remarks>
        ///     这个方法日后要被替换，
        /// </remarks>
        public static void Adjust(string controlPointsFile, ControlPointsInputType controlPointsInputType, IGeometry geometry, SpatialAdjustMethodType spatialAdjustMethodType)
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

                        var a = transformMethodMap[spatialAdjustMethodType];
                        var constructors = a.GetConstructors();
                        var constructorIWant = a.GetConstructors().Where(constructor => constructor.GetParameters().Length == 0).FirstOrDefault();
                        var b = constructorIWant.Invoke(null);
                        transformMethod = b as ITransformationMethodGEN;
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

        public void Adjust(IGeometry geometry, ITransformationMethodGEN transformation)
        {
            transformation.TransformShape(geometry);
        }

        public void Georef()
        {
            //IRasterGeometryProc3 rgpc = new RasterGeometryProcClass();
        }
    }

    public enum SpatialAdjustMethodType
    {
        Affine, Conformal, EdgeSnap, Piecewise, Projective
    }
}