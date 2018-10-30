using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.DataSourcesGDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AoCli;
using ESRI.ArcGIS.Geodatabase;
using System.Diagnostics;
using AoCli.AoActions;

namespace AoCli.Test
{

    [TestClass]
    public class MyTestClass
    {
        [TestMethod]
        public void MyTestMethod()
        {

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

    }
}
