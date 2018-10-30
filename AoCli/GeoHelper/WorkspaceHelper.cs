using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoCli.GeoHelper
{
    public class WorkspaceHelper
    {


    }

    class SdeWorkspaceHelper : WorkspaceHelper
    {

    }

    public class GdbWorkspaceHelper : WorkspaceHelper
    {

        private string filePath;

        public string FilePath { get => filePath; set => filePath = value; }
    }

}
