using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSGeo.OGR;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace GdalCli.Test
{
    [TestClass]
    public class ConnectTests
    {
        [TestMethod]
        public void MsTm()
        {
            //new TestClass().Test1();
            Program.Test1();
        }

        [TestMethod]
        public void TestMethod1()
        {
            Ogr.RegisterAll();
            var shpDriver = Ogr.GetDriverByName("ESRI Shapefile");
            var gdbDriver = Ogr.GetDriverByName("OpenFileGDB");
            var gdbDatasource = gdbDriver.Open(@"C:\test\temp.gdb", 0);
            Trace.WriteLine(gdbDatasource.GetLayerCount());
            var layerList = gdbDatasource.Layers().ToList();
            layerList.ForEach((layer) =>
            {
                Trace.WriteLine(String.Format(
                    "{0:D3} {1} {2} {3}",
                    layerList.IndexOf(layer),
                    layer.GetName(),
                    layer.GetGeomType(),
                    layer.GetFeatureCount(20)
                    ));
            });
            GdalUtils.OgrDrivers().ToList().ForEach((driver) =>
            {
                Trace.WriteLine(driver.name);
            });
        }

        [TestMethod]
        public void ShapefileTestMethod()
        {
            Ogr.RegisterAll();
            var shpDriver = Ogr.GetDriverByName("ESRI Shapefile");
            var shpDs = shpDriver.Open(@"C:\test\continents.shp", 0);
            var layer = shpDs.Layers().ToList()[0];
            var fields = layer.Fields();
            Trace.WriteLine(String.Join(" ", fields.Select(field => field.GetName())));
            layer.Features().ToList().ForEach((feature) =>
            {
                Trace.WriteLine(String.Join(" ", feature.Fields().Select(field => feature.GetFieldAsString(field.GetName()))));
            });
        }

        [TestMethod]
        public void SqliteTestMethod()
        {
            Ogr.RegisterAll();
            var sqliteDriver = Ogr.GetDriverByName("SQLite");
            var sqliteDs = sqliteDriver.Open(@"C:\Users\lmb\spltest.db", 0);
            sqliteDs.Layers().ToList().ForEach((layer) =>
            {
                Trace.WriteLine(layer.GetName());
                var fields = layer.Fields();
                Trace.WriteLine(String.Join(" ", fields.Select(field => field.GetName())));
                Trace.WriteLine(layer.GetFeatureCount(0));
                layer.GetSpatialRef().ExportToProj4(out var layerProj4Str);
                var sr = layer.GetSpatialRef();
                Trace.WriteLine(layerProj4Str);
                layer.Features(2).ToList().ForEach((feature) =>
                {
                    Trace.WriteLine(String.Join(" ", feature.Fields().Select(field => feature.GetFieldAsString(field.GetName()))));
                    double[] vs = new double[3];
                    var geom = feature.GetGeomFieldRef(0);
                    var geom1 = feature.GetGeometryRef();
                    var geomJson = geom.ExportToJson(null);
                    Trace.WriteLine(geomJson);
                    feature.GetGeometryRef().GetGeometryType();
                    feature.GetGeometryRef().GetPoint(0, vs);
                });
            });
        }

        [TestMethod]
        public void PostgresqlTestMethod()
        {
            Ogr.RegisterAll();
            var pgDriver = Ogr.GetDriverByName("PostgreSQL");
            var pgDs = pgDriver.Open(@"PG:dbname=postgis_24_sample host=localhost port=5432 user=postgres password=123", 0);
            var layerName = "continents";
            try
            {
                var layer = pgDs.GetLayerByName(layerName);
                var fields = layer.Fields();
                Trace.WriteLine(String.Join(" ", fields.Select(field => field.GetName())));
                var sr = layer.GetSpatialRef();
                sr.ExportToProj4(out var srStr);
                Trace.WriteLine(srStr);
                layer.Features(2).ToList().ForEach((feature) =>
                {
                    Trace.WriteLine(String.Join(" ", feature.Fields().Select(field => feature.GetFieldAsString(field.GetName()))));
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [TestMethod]
        public void OracleTestMetodh()
        {
            string balabala(string a)
            {
                return a;
            }
            balabala("sd");
            //var file = File.Open("hello", FileMode.Open);
            //Ogr.RegisterAll();
            //var pgDriver = Ogr.GetDriverByName("PostgreSQL");
            //var pgDs = pgDriver.Open(@"OCI:sde/sde@192.168.0.2/orcl", 0);

            ;
            //pgDs.GetLayerCount();

            //pgDs.Layers().ToList().ForEach(layer => Trace.WriteLine(layer.GetName()));
            //var layerName = "continents";
            //try
            //{
            //    var layer = pgDs.GetLayerByName(layerName);
            //    var fields = layer.Fields();
            //    Trace.WriteLine(String.Join(" ", fields.Select(field => field.GetName())));
            //    var sr = layer.GetSpatialRef();
            //    sr.ExportToProj4(out var srStr);
            //    Trace.WriteLine(srStr);
            //    layer.Features(2).ToList().ForEach((feature) =>
            //    {
            //        Trace.WriteLine(String.Join(" ", feature.Fields().Select(field => feature.GetFieldAsString(field.GetName()))));
            //    });
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }
    }

    [TestClass]
    public class GdalTest
    {
        [TestMethod]
        public void LayerCsvTest()
        {
            Trace.WriteLine(Uri.HexEscape(','));
            var urus = Uri.EscapeDataString("ad,asd");
            Trace.WriteLine(urus);
            Ogr.RegisterAll();
            var shpDriver = Ogr.GetDriverByName("ESRI Shapefile");
            var shpDs = shpDriver.Open(@"C:\test\continents.shp", 0);
            var layer = shpDs.GetLayerByIndex(0);

            var csv = GdalUtils.LayerToCsvString(layer, true);
            Trace.WriteLine(csv);
        }

        [TestMethod]
        public void CreateShpTest()
        {
            Ogr.RegisterAll();
            var shpDriver = Ogr.GetDriverByName("ESRI Shapefile");

            var shpDatasource = shpDriver.CreateDataSource(@"C:\test\shpCreatedByGdal.shp", null);

            //shpDatasource.GetLayerByIndex(0).feat
        }

        [TestMethod]
        public void TdtToShapefile()
        {
            Program.TdtApiToShapefile("佛山市", @"C:\test\dz.shp", "6c13494a3e9ed596149030705b37fd6c");
        }

        [TestMethod]
        public void FromResponseStringTest()
        {
            //var stream =  new FileStream(@"Test\testResponseText.txt", FileMode.Open);

            var stream = File.OpenText(@"Test\testResponseText.txt");
            var responseString = stream.ReadToEnd();

            //var obj = JsonConvert.DeserializeObject<AdministrativeResponseObject>(responseString);
            //obj.data[0].child.for

            //Trace.WriteLine(responseString);
            //var responseText =       new FileStream(   )

            //Program.FromResponseString(                 )
        }
    }
}