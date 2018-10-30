using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using OSGeo.OGR;
using OSGeo.OSR;
using System.IO;
using System.Diagnostics;

namespace GdalCli.GdalImaging
{
    public static class ImageUtils
    {
        public static string ExportBmp(this Feature feature)
        {

            return "";
        }


    }


    public static class Drawer
    {
        public static void DrawLayer(Layer layer)
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

        public static string DrawGeometry(Geometry geometry)
        {

            var geomType = geometry.GetGeometryType();
            //Trace.WriteLine(geometry.GetGeometryType());
            switch (geomType)
            {
                case wkbGeometryType.wkbUnknown:
                    break;
                case wkbGeometryType.wkbPoint:
                    break;
                case wkbGeometryType.wkbLineString:
                    break;
                case wkbGeometryType.wkbPolygon:
                    break;
                case wkbGeometryType.wkbMultiPoint:
                    break;
                case wkbGeometryType.wkbMultiLineString:
                    break;
                case wkbGeometryType.wkbMultiPolygon:
                    DrawMultiPolygon(geometry);
                    break;
                case wkbGeometryType.wkbGeometryCollection:
                    break;
                case wkbGeometryType.wkbNone:
                    break;
                case wkbGeometryType.wkbLinearRing:
                    break;
                case wkbGeometryType.wkbPoint25D:
                    break;
                case wkbGeometryType.wkbLineString25D:
                    break;
                case wkbGeometryType.wkbPolygon25D:
                    break;
                case wkbGeometryType.wkbMultiPoint25D:
                    break;
                case wkbGeometryType.wkbMultiLineString25D:
                    break;
                case wkbGeometryType.wkbMultiPolygon25D:
                    break;
                case wkbGeometryType.wkbGeometryCollection25D:
                    break;
                default:
                    break;
            }


            return null;

        }

        public static string DrawMultiPolygon(Geometry geometry)
        {
            Trace.WriteLine(geometry.Geometries().ToList().Count);
            Envelope envelope = new Envelope();
            geometry.GetEnvelope(envelope);
            var width = envelope.MaxX - envelope.MinX;
            var height = envelope.MaxY - envelope.MinY;
            var left = envelope.MinX;
            var bottom = envelope.MinY;
            var longerSide = Math.Max(width, height);
            // actual / imagesize
            var resolution = longerSide / 512.0;
            geometry.GetEnvelope(envelope);
            var ms = new MemoryStream();
            Bitmap bitmap = new Bitmap(512, 512);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                geometry.Geometries().ToList().ForEach((polygon) =>
                {
                    var points = polygon.Geometries().First().Points();
                    var pointsList = points.ToList();
                    var drawPoints = pointsList.Select((point) =>
                    {
                        var x = point[0];
                        var y = point[1];
                        var actualWidth = x - left;
                        var drawX = actualWidth / resolution;
                        var actualHeight = y - bottom;
                        var drawY = 512 - actualHeight / resolution;
                        return new int[2] {
                            Convert.ToInt32(drawX),
                            Convert.ToInt32(drawY)
                        };
                    }).ToArray();
                    var count = drawPoints.Count();
                    for (int i = 0; i < count - 1; i++)
                    {
                        graphics.DrawLine(new Pen(Color.Blue), drawPoints[i][0], drawPoints[i][1], drawPoints[i + 1][0], drawPoints[i + 1][1]);
                    }
                });
            }
            bitmap.Save(ms, ImageFormat.Png);
            var b64s = Convert.ToBase64String(ms.ToArray());
            Trace.WriteLine(String.Format(@"data:image/png;base64,{0}", b64s));

            


            return null;
        }

    }
}
