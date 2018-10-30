using OSGeo.OGR;
using OSGeo.OSR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Text;

namespace GdalCli
{
    public class Program
    {
        //[STAThread]
        public static void Main(string[] args)
        {
            //new TestClass().Test1();
            //Test1();
            //Test2();
        }

        static void Test2()
        {
            Ogr.RegisterAll();
            OSGeo.OGR.Driver driver = Ogr.GetDriverByName("ESRI Shapefile");
            //var dataSource = driver.Open(@"C:\test\continents.shp", 0);

            OpenFileDialog openFileDialog = new OpenFileDialog();
            var result = openFileDialog.ShowDialog();
            string filePath = @"C:\test\continents.shp";
            if (result == DialogResult.Yes || result == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
            }
            var dataSource = driver.Open(filePath, 0);
        }

        public static void Test1()
        {
            Ogr.RegisterAll();
            OSGeo.OGR.Driver driver = Ogr.GetDriverByName("ESRI Shapefile");
            var ds = driver.Open(@"C:\test\continents.shp", 0);
            var drv = ds.GetDriver();
            var layer = ds.GetLayerByIndex(0);
            var count = layer.GetFeatureCount(0);
            var name = ds.GetName();
            var sr = layer.GetSpatialRef();
            sr.ExportToProj4(out var p4Str);
            var cgcs2000wkt = "GEOGCS[\"ChinaGeodeticCoordinateSystem2000\",DATUM[\"China_2000\",SPHEROID[\"CGCS2000\",6378137,298.257222101,AUTHORITY[\"EPSG\",\"1024\"]],AUTHORITY[\"EPSG\",\"1043\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.0174532925199433,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"4490\"]]";
            var srCgcs2000 = new SpatialReference(cgcs2000wkt);
            srCgcs2000.ExportToProj4(out var cgcs2000p4str);
            var webMctWkt = "PROJCS[\"WGS84/Pseudo-Mercator\",GEOGCS[\"WGS84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.0174532925199433,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"4326\"]],PROJECTION[\"Mercator_1SP\"],PARAMETER[\"central_meridian\",0],PARAMETER[\"scale_factor\",1],PARAMETER[\"false_easting\",0],PARAMETER[\"false_northing\",0],UNIT[\"metre\",1,AUTHORITY[\"EPSG\",\"9001\"]],AXIS[\"X\",EAST],AXIS[\"Y\",NORTH],EXTENSION[\"PROJ4\",\"+proj=merc+a=6378137+b=6378137+lat_ts=0.0+lon_0=0.0+x_0=0.0+y_0=0+k=1.0+units=m+nadgrids=@null+wktext+no_defs\"],AUTHORITY[\"EPSG\",\"3857\"]]";
            var srWebMct = new SpatialReference(webMctWkt);
            var projMethods = Osr.GetProjectionMethods();
            sr.ExportToProj4(out var o1);
            srWebMct.ExportToProj4(out var o2);
            Dictionary<string, int> isProjectedDictionary = new Dictionary<string, int>();
            isProjectedDictionary.Add(o1, sr.IsProjected());
            isProjectedDictionary.Add(o2, srWebMct.IsProjected());
            double[] vs = new double[3];
            //transform.TransformPoint(vs, 120, 30, 20);
            SpatialReference srr = new SpatialReference("");
            srr.SetWellKnownGeogCS("WGS84");
            //srr.SetProjCS("UTM 17 (WGS84) in northern hemisphere.");
            srr.SetProjCS("");
            //srr.SetUTM(17, 1);
            srr.ExportToProj4(out var srrstr);
            var defn = layer.GetLayerDefn();
            var defnCount = defn.GetFieldCount();
            layer.Fields().ToList().ForEach(field => Console.Write(String.Format("{0} {1} ", field.GetName(), field.GetFieldTypeName(field.GetFieldType()))));
            Console.Write("\n");
            var clist = layer.Features().Select(a => a.GetFieldAsString(1)).ToList();
            Console.WriteLine(String.Join(" , ", clist));
            Console.Write("\n");
            foreach (var feature in layer.Features())
            {
                Console.WriteLine(feature.GetFieldAsString(1));
                var geomType = feature.GetGeometryRef().GetGeometryType();
                Console.WriteLine(geomType);
                //var j = feature.GetGeometryRef().ExportToJson(null);
                foreach (var geometry in feature.GetGeometryRef().Geometries())
                {
                    //Console.WriteLine(geometry.GetGeometryType());
                    Envelope envelope = new Envelope();
                    geometry.GetEnvelope(env: envelope);
                    var pointCount = geometry.GetGeometryRef(0).GetPointCount();
                    foreach (var point in geometry.GetGeometryRef(0).Points())
                    {
                        //Console.WriteLine(String.Format("{0} {1}", point[0], point[1]));
                    }
                    var gml = geometry.ExportToGML();
                    var json = geometry.ExportToJson(null);
                    foreach (var point in geometry.Points())
                    {
                        ;
                    }
                }
            }
            layer.ResetReading();
            var gfr = layer.GetFeaturesRead();
            //Console.ReadLine();
        }


        public static void TdtApiToShapefile(string keyword, string shapefilePath, string apiToken)
        {

            Ogr.RegisterAll();
            OSGeo.OGR.Driver driver = Ogr.GetDriverByName("ESRI Shapefile");
            var ds = driver.Open(@"C:\test\dz.shp", 1);
            var layer = ds.GetLayerByIndex(0);
            //var features = ds.GetLayerByIndex(0).Features().ToList();
            //Trace.WriteLine(features.Count);
            //Console.WriteLine(features.Count);
            var postStrObj = new
            {
                searchWord = keyword,
                searchType = "1",
                needSubInfo = true.ToString(),
                needAll = false.ToString(),
                needPolygon = true.ToString(),
                needPre = true.ToString()
            };
            var token = apiToken;
            var postStr = JsonConvert.SerializeObject(postStrObj, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
            Trace.WriteLine(postStr);
            var url = String.Format(
                "http://api.tianditu.gov.cn/administrative?postStr={0}&tk={1}",
                postStr, token
                );


            WebClient client = new WebClient();
            client.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.100 Safari/537.36");
            var task = client.DownloadStringTaskAsync(new Uri(url));
            task.Wait();
            var responseString = task.Result;

            //var responseString = File.OpenText(@"Test\testResponseText.txt").ReadToEnd();


            var adm = FromResponseString(responseString);
            var layerDefn = layer.GetLayerDefn();

            //each county
            adm.data[0].child.ForEach((county) =>
            {
                //var defn = layer.GetLayerDefn();
                //var geom = new Geometry(layer.GetGeomType());
                //Feature feature = new Feature(defn);
                //feature.SetField("cn", county.name);
                //feature.SetField("en", county.englist);
                //var nums = county.points[0].region.Split(',').ToList();
                //var numsLength = nums.Count;
                //nums.ForEach((coordinateStr) =>
                //{
                //    var split = coordinateStr.Split(' ');
                //    var x = Convert.ToDouble(split[0]);
                //    var y = Convert.ToDouble(split[1]);
                //});



                var feature = new Feature(layerDefn);
                //layerDefn.FieldDefns().ToList().ForEach((fieldDefn) =>
                //{


                //    //layer.CreateFeature(feature);
                //    //feature = null;
                //    //Ogr.CreateGeometryFromWkt()

                //    //layer.getgeo
                //    //Trace.WriteLine(fieldDefn.GetNameRef());

                //});

                //Trace.WriteLine(county.name);
                //feature.SetField("cn", county.name);
                feature.SetField("en", county.english);
                //var wgs84 = new SpatialReference("GEOGCS["WGS 84",DATUM["WGS_1984",SPHEROID["WGS 84",6378137,298.257223563,AUTHORITY["EPSG","7030"]],AUTHORITY["EPSG","6326"]],PRIMEM["Greenwich",0,AUTHORITY["EPSG","8901"]],UNIT["degree",0.0174532925199433,AUTHORITY["EPSG","9122"]],AUTHORITY["EPSG","4326"]]")
                var wgs84 = new SpatialReference("GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.0174532925199433,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"4326\"]]");
                var wkt = String.Format("POLYGON(({0}))", county.points[0].region);
                var geom = Ogr.CreateGeometryFromWkt(ref wkt, wgs84);
                feature.SetGeometry(geom);


                layer.CreateFeature(feature);


                //geom.AddPoint_2D(    )

                //feature.SetGeometry()
                //layer.CreateFeature(feature);

                //feature.Fields().ToList().ForEach((field) => { Trace.WriteLine(field.GetName()); });
                //var points = feature.GetGeometryRef().Points();
                //Trace.WriteLine(feature.GetGeometryRef().GetGeometryType());

                //Feature feature = new Feature()


                //layer.CreateFeature()

            });




            Trace.WriteLine(url);

        }


        public static AdministrativeResponseObject FromResponseString(string responseString)
        {
            var obj = JsonConvert.DeserializeObject<AdministrativeResponseObject>(responseString, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
            return obj;
        }

    }

    public class AdministrativeResponseObject
    {
        public string msg { get; set; }

        public List<AdminModel> data { get; set; }

    }

    public class AdminModel
    {
        public string adminType { get; set; }
        public string bound { get; set; }

        public string english { get; set; }

        //public string name { get; set; }

        public List<PointsPart> points { get; set; }

        public List<AdminModel> child { get; set; }

    }


    public class PointsPart
    {
        public string region { get; set; }
    }

    public class TestClass
    {
        public void Test1()
        {



        }
    }

}
