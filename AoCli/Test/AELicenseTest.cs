using AoCli.AoActions;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;

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
    }
}