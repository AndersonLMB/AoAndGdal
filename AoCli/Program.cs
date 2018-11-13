using ESRI.ArcGIS;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Threading.Tasks;

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
            Console.WriteLine("5秒后结束");
            Task.Delay(5000).Wait();

        }

        public static void BindLicense()
        {
            RuntimeManager.BindLicense(ProductCode.EngineOrDesktop);
        }
    }
}