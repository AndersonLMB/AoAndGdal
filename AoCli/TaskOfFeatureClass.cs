using System;
using System.Threading.Tasks;
using ESRI.ArcGIS.Geodatabase;

namespace AoCli
{
    public class TaskOfFeatureClass
    {
        public IWorkspace Workspace { get; set; }
        public string FeatureClassName { get; set; }

        public Task CurrentStartedTask { set; get; }

        public Action Action()
        {
            return () =>
            {
                var fc = ((IFeatureWorkspace)Workspace).OpenFeatureClass(FeatureClassName);
                Console.WriteLine(String.Format("{1} {0} opened", ((IDataset)fc).Name, DateTime.Now.ToFullTimeSecondString()));
            };
        }
    }
}
