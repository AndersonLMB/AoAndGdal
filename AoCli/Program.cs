using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geodatabase;
using System.Threading;
using ESRI.ArcGIS.DataSourcesRaster;
using System;
using ESRI.ArcGIS;
using McMaster.Extensions.CommandLineUtils;

namespace AoCli
{
    public class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        [STAThread]//不加这个STAThread的Attribute会报错，原因不详，详见https://stackoverflow.com/questions/12426559/rpc-e-serverfault。
        public static void Main(string[] args)
        {
            //CommandLineApplication.Execute<Program>(args);
            CommandLineApplication.Execute<CommandLineController>(args);
            //new ArcEngineLicense();
            //Console.ReadLine();
        }

        public static void BindLicense()
        {
            RuntimeManager.BindLicense(ProductCode.EngineOrDesktop);
        }
    }

}
