using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace AoData
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            CommandLineApplication.Execute<CommandLineController>(args);
        }
    }
    public class CommandLineController
    {
        public void OnExecute() { }
    }
}
