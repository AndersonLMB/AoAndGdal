using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.EditorExt;
using ESRI.ArcGIS.Geometry;
using System;
using System.Linq;
using System.Collections;

using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.ConversionTools;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ESRI.ArcGIS.DataSourcesGDB;
using System.Diagnostics;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.ADF;

namespace AoCli.AoActions
{
    public static class LogActions
    {


        //public static void LogFeatureClass(IFeatureClass featureClass)
        //{

        //}


        public static void LogFeatureClass(string datasourceFile, DataSourceType dataSourceType, string featureClassName)
        {
            //aocli loglayer -ds C:\OCNwork\东莞\20181022\WGS84.gdb -dst gdbfilepath -ln 城际轨道站点
            IFeatureClass featureClass = null;

            switch (dataSourceType)
            {
                case DataSourceType.SdeFilePath:
                    throw new NotImplementedException();
                case DataSourceType.SdeTxt:
                    throw new NotImplementedException();
                case DataSourceType.SdeJson:
                    throw new NotImplementedException();
                case DataSourceType.GdbFilePath:
                    featureClass = DataActions.GetFeatureClass(datasourceFile, featureClassName);
                    break;
                case DataSourceType.ShapefilePath:
                    throw new NotImplementedException();
                default:
                    break;
            }

            LogFeatureClass(featureClass);
        }

        public static void LogFeatureClass(IFeatureClass featureClass)
        {
            Console.WriteLine(featureClass.FeatureCount(null));
        }
    }

    public enum DataSourceType
    {
        SdeFilePath, SdeTxt, SdeJson, GdbFilePath, ShapefilePath
    }
}
