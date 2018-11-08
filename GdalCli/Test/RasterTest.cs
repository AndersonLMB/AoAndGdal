using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSGeo.GDAL;
using OSGeo.OGR;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;

namespace GdalCli.Test
{
    [TestClass]
    public class RasterTest
    {
        [TestMethod]
        public void GCPTest()
        {
            Ogr.RegisterAll();
            Gdal.AllRegister();
            var dataset = Gdal.Open(@"C:\test\v2\wsiearth.tif", Access.GA_Update);
            var prj = dataset.GetProjection();
            var gcps = dataset.GetGCPs();
            var gcpSr = dataset.GetGCPProjection();

            var task = Task.Factory.StartNew(() => { });
            var aw = task.GetAwaiter();
            task.ContinueWith((a) => { });

            var filelist = dataset.GetFileList();
            Trace.WriteLine(filelist);
            //Trace.WriteLine(prj);
            //Console.WriteLine(gcpSr);
            //Trace.WriteLine(gcpSr);
        }

        public void DatasetTest()
        {
            System.Data.DataSet dataSet = new DataSet();

            DataTable dataTable = new DataTable();

            dynamic a = null;
            a.OutPut();
        }
    }
}