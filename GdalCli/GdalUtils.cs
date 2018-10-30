using OSGeo.OGR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections;
using System.Linq;

namespace GdalCli
{
    public static class GdalUtils
    {
        public static IEnumerable<Layer> Layers(this DataSource dataSource)
        {
            var count = dataSource.GetLayerCount();
            for (int i = 0; i < count; i++)
            {
                yield return dataSource.GetLayerByIndex(i);
            }
        }

        public static IEnumerable<OSGeo.OGR.Driver> OgrDrivers()
        {
            var count = Ogr.GetDriverCount();
            for (int i = 0; i < count; i++)
            {
                yield return Ogr.GetDriver(i);
            }
        }

        /// <summary>
        /// Layer里面的所有Feature
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public static IEnumerable<Feature> Features(this Layer layer)
        {
            //var count = layer.GetFeatureCount(0);
            //for (int i = 0; i < count; i++)
            //{
            //    yield return layer.GetFeature(i);
            //}
            return layer.Features(0);
        }

        /// <summary>
        /// Layer里面的所有Feature
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="offset">偏移量</param>
        /// <returns></returns>
        public static IEnumerable<Feature> Features(this Layer layer, int offset)
        {
            Feature feature = null;
            while ((feature = layer.GetNextFeature()) != null)
            {
                yield return feature;
            }
            //var count = layer.GetFeatureCount(0);
            //for (int i = 0; i < count; i++)
            //{
            //    yield return layer.GetFeature(i + offset);
            //}
        }

        public static IEnumerable<Geometry> Geometries(this Geometry geometry)
        {
            var count = geometry.GetGeometryCount();
            for (int i = 0; i < count; i++)
            {
                var item = geometry.GetGeometryRef(i);
                //Trace.WriteLine(item.GetGeometryType());
                yield return item;
            }
        }

        public static IEnumerable<double[]> Points(this Geometry geometry)
        {

            var count = geometry.GetPointCount();

            var dimension = geometry.GetCoordinateDimension();
            //Trace.WriteLine(geometry.GetGeometryType());
            for (int i = 0; i < count; i++)
            {
                double[] vs = new double[dimension];
                geometry.GetPoint(i, vs);
                yield return vs;
            }
        }

        public static IEnumerable<FieldDefn> Fields(this Layer layer)
        {
            var count = layer.GetLayerDefn().GetFieldCount();
            for (int i = 0; i < count; i++)
            {
                yield return layer.GetLayerDefn().GetFieldDefn(i);
            }
        }

        public static IEnumerable<FieldDefn> Fields(this Feature feature)
        {
            var count = feature.GetFieldCount();
            for (int i = 0; i < count; i++)
            {
                yield return feature.GetFieldDefnRef(i);
            }
        }


        public static IEnumerable<FieldDefn> FieldDefns(this FeatureDefn featureDefn)
        {
            var count = featureDefn.GetFieldCount();
            for (int i = 0; i < count; i++)
            {
                yield return featureDefn.GetFieldDefn(i);
            }
        }
        public static string LayerToCsvString(Layer layer, bool withGeometry)
        {
            string csv = "";

            var features = layer.Features();
            var layerFields = layer.Fields();
            var fieldCount = layerFields.Count();
            features.ToList().ForEach((feature) =>
            {
                List<string> vs = new List<string>();
                for (int i = 0; i < fieldCount; i++)
                {
                    var fieldVal = feature.GetFieldAsString(i);
                    fieldVal = Uri.EscapeDataString(fieldVal);
                    vs.Add(fieldVal);
                }
                if (withGeometry)
                {
                    var json = feature.GetGeometryRef().ExportToJson(null);
                    //json.ToArray()
                    //var newJson = String.Join("", json.ToList().Select(character => Uri.HexEscape(character)));
                    string str = "";
                    var wkt = feature.GetGeometryRef().ExportToWkt(argout: out str);

                    //Trace.WriteLine(newJson);

                    //json = Uri.EscapeDataString(json);
                    vs.Add(json);
                }
                var featureLine = String.Format("{0}\n     ", String.Join(",", vs));
                csv += featureLine;
            });
            return csv;
        }
    }

}
