using AoCli.AoActions;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ESRI.ArcGIS.AnalysisTools;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS;

namespace AoCli.Test
{
    [TestClass]
    public class MyTestClass
    {
        [TestMethod]
        public void MyTestMethod()
        {
            new ArcEngineLicense();
            new ArcEngineLicense();
            new ArcEngineLicense();
            new ArcEngineLicense();
            new PointClass();
            //AoCli.Program.BindLicense();

            //AoCli.Program.Main(null);
            //new ArcEngineLicense();
        }

        [TestMethod]
        public void TestReadGdeFeatureClass()
        {
            new ArcEngineLicense();
            //FileGDBWorkspaceFactory fileGDBWorkspaceFactory = new FileGDBWorkspaceFactoryClass();
            //var workspace = fileGDBWorkspaceFactory.OpenFromFile(@"C:\OCNwork\东莞\20181022\YDYZT (WGS84)\WGS84.gdb", 0);
            //var fc = ((IFeatureWorkspace)workspace).OpenFeatureClass("城际轨道站点");
            //Trace.WriteLine($"{ fc.FeatureCount(null)}");
            var fc = DataActions.GetFeatureClass(@"C:\OCNwork\东莞\20181022\YDYZT (WGS84)\WGS84.gdb", WorkspaceType.Gdb, "城际轨道站点");
            Trace.WriteLine(fc.FeatureCount(null));
        }

        [TestMethod]
        public void AdjustTest()
        {
            new ArcEngineLicense();
            SpatialAdjust.Adjust();
        }

        [TestMethod]
        public void ReadFeatureClassFromSdeFileTest()
        {
            new ArcEngineLicense();
            var fc = DataActions.GetFeatureClass(@"C:\OCNwork\东莞\dggisdata02.sde", DataSourceType.SdeFilePath, "GUIDAO_ZQ");
            LogActions.LogFeatureClass(fc);
        }

        [TestMethod]
        public void PropertySetToDictionaryTest()
        {
            new ArcEngineLicense();
            IPropertySet ps = new PropertySetClass();
            ps.SetProperty("a", "b");
            ps.SetProperty("as", "bs");
            var dic = ps.ToDictionary();
        }

        [STAThread]
        [TestMethod]
        public void RasterReadTest()
        {
            new ArcEngineLicense();
            //new SpatialAdjust().Georef();

            IRasterDataset rasterDataset = new RasterDatasetClass();

            rasterDataset.OpenFromFile(@"C:\test\v2\wsiearth.tif");
            //rasterDataset.Format;
            var format = rasterDataset.Format;
            Trace.WriteLine(format);
        }

        [TestMethod]
        public void ShapeInWhereTest()
        {
            new ArcEngineLicense();

            var sdeWs = DataActions.GetSdeWorkspace(new Dictionary<string, object>()
            {
                { "INSTANCE" , "sde:oracle11g:192.168.0.2/orcl"},
                { "USER" , "dzgisdata"},
                { "PASSWORD" , "dzgisdata"},
            });

            var feaWs = (IFeatureWorkspace)sdeWs;

            IFeatureClass xzqh = null;
            IFeatureClass ydhx = null;
            try
            {
                xzqh = feaWs.OpenFeatureClass("DZGISDATA.XZQH");
                ydhx = feaWs.OpenFeatureClass("DZGISDATA.YDHX_SP");
            }
            catch (Exception ex)
            {
                throw new Exception("尝试打开图层失败，名字或许不存在");
            }

            var chartMap = new Dictionary<string, int>();

            ydhx.Features().ToList().ForEach((feature) =>
            {
                Dictionary<string, double> interectAreasDic = new Dictionary<string, double>();
                xzqh.Features().ToList().ForEach((xz) =>
                {
                    var copy = xz.ShapeCopy;
                    copy.SpatialReference = feature.Shape.SpatialReference;
                    var intersect = ((ITopologicalOperator)(feature.Shape)).Intersect(copy, esriGeometryDimension.esriGeometry2Dimension);
                    var area = ((IArea)intersect).Area;
                    var row = (IRow)xz;
                    var fieldIndex = row.Fields.FindField("NAME");

                    string name = String.Empty;
                    try
                    {
                        name = (string)xz.Value[fieldIndex];
                    }
                    catch (Exception)
                    {
                    }
                    double storedArea;
                    if (interectAreasDic.TryGetValue(name, out storedArea))
                    {
                        if (area > storedArea)
                        {
                            interectAreasDic[name] = area;
                        }
                    }
                    else
                    {
                        interectAreasDic[name] = area;
                    }
                });
                var descendOrder = interectAreasDic.OrderByDescending(kvp => kvp.Value);
                var finalRegionName = descendOrder.First().Key;
                var preCount = 0;
                if (chartMap.TryGetValue(finalRegionName, out preCount))
                {
                    chartMap[finalRegionName] += 1;
                }
                else
                {
                    chartMap[finalRegionName] = 0;
                }
            });
        }

        [TestMethod]
        public void ShapeInWhereTest2()
        {
            new ArcEngineLicense();
            var sdeWs = DataActions.GetSdeWorkspace(new Dictionary<string, object>()
            {
                { "INSTANCE" , "sde:oracle11g:192.168.0.2/orcl"},
                { "USER" , "dzgisdata"},
                { "PASSWORD" , "dzgisdata"},
            });




            var feaWs = (IFeatureWorkspace)sdeWs;
            var xzqh = feaWs.OpenFeatureClass("DZGISDATA.XZQH");
            var ydhx = feaWs.OpenFeatureClass("DZGISDATA.YDHX_SP");

            IGpValueTableObject gpvt = new GpValueTableObjectClass();
            gpvt.AddRow(xzqh);
            gpvt.AddRow(ydhx);


            var outFcname = $@"C:/test/i{DateTime.Now.ToBinary()}.shp";
            Intersect intersect = new Intersect()
            {
                //in_features = @"C:\test\dzgisdata02.sde\DZGISDATA.XZQH\DZGISDATA.XZQH;C:\test\dzgisdata02.sde\DZGISDATA.YWHX\DZGISDATA.YDHX_SP",
                in_features = gpvt,
                out_feature_class = outFcname,
                join_attributes = "ONLY_FID",
                output_type = "INPUT"
            };

            //gpfeatureclass

            var result = new Geoprocessor().Execute(intersect, null);

            var gpResult = (IGeoProcessorResult)result;

            var gpms = gpResult.GetResultMessages();



            ;
        }

        [TestMethod]
        public void ShapeInWhereTest3()
        {
            //new ArcEngineLicense();
            RuntimeManager.BindLicense(ProductCode.EngineOrDesktop);
            //ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.EngineOrDesktop);
            var sdeWs = DataActions.GetSdeWorkspace(new Dictionary<string, object>()
            {
                { "INSTANCE" , "sde:oracle11g:192.168.0.2/orcl"},
                { "USER" , "dzgisdata"},
                { "PASSWORD" , "dzgisdata"},
            });




            var outFcname = $@"C:/test/i{DateTime.Now.ToBinary()}.shp";
            var feaWs = (IFeatureWorkspace)sdeWs;
            var xzqh = feaWs.OpenFeatureClass("DZGISDATA.XZQH");
            //var ydhx = feaWs.OpenFeatureClass("DZGISDATA.YDHX_SP");

            IVariantArray arr = new VarArrayClass();
            //arr.Add(@"C:\test\dzgisdata02.sde\DZGISDATA.XZQH\DZGISDATA.XZQH");
            arr.Add(outFcname);

            //IGeoProcessor gp2 = new GeoProcessorClass();
            //IGeoProcessorResult result = gp2.Execute("Select", arr, null);
            var gp = new Geoprocessor();


            var result = gp.Execute(new Select()
            {
                in_features = @"C:\test\continents.shp",
                out_feature_class = outFcname,
            }, null);

            ;

            //Select sel = new Select()
            //{
            //    in_features = xzqh,
            //    out_feature_class = outFcname

            //};
            //var result = new Geoprocessor().Execute(sel, null);

            //var gpResult = (IGeoProcessorResult)result;

            //var gpms = gpResult.GetResultMessages();

        }

    }
}