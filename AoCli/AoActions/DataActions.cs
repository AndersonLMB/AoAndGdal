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
    public static class DataActions
    {
        public static void Hello()
        {
        }

        /// <summary>
        /// 覆盖featureclass
        /// </summary>
        /// <param name="fromFeatureClass"></param>
        /// <param name="toFeatureClass"></param>
        public static void CoverFeatureClassWithFeatureClass(IFeatureClass fromFeatureClass, IFeatureClass toFeatureClass)
        {

            CoverFeatureClassWithFeatureClass(fromFeatureClass, toFeatureClass, null);
        }

        public static void CoverFeatureClassWithFeatureClass(IFeatureClass fromFeatureClass, IFeatureClass toFeatureClass, SpatialAdjust spatialAdjust)
        {
            //https://github.com/xinying180/ConvertingData
            ((ITable)toFeatureClass).DeleteSearchedRows(null);

            Console.WriteLine($"{fromFeatureClass.FeatureCount(null)} 个要素待转换");
            Console.WriteLine($"使用 {spatialAdjust.TransformationMethod.Name} 方法偏移");
            fromFeatureClass.Features().ToList().ForEach((feature) =>
            {
                var createdFeature = toFeatureClass.CreateFeature();
                var copy = feature.ShapeCopy;

                if (spatialAdjust != null)
                {
                    //Console.WriteLine($"使用{ SpatialAdjust.transformMethodMap[spatialAdjust.SpatialAdjustMethodType].Name}来Spatial Adjustment");
                    var targetSr = createdFeature.Shape.SpatialReference;
                    copy.Project(targetSr);
                    var fromSr = feature.Shape.SpatialReference;
                    Trace.WriteLine($"{fromSr.Name} -> {targetSr.Name}");
                    spatialAdjust.AdjustGeometry(copy);
                    //SpatialActions.Adjust(@"C:\OCNwork\东莞\dgcps.txt", ControlPointsInputType.File, copy);
                }
                else
                {
                    //Console.WriteLine($"不做Spatial Adjustmnent");
                }
                createdFeature.Shape = copy;
                for (int i = 0; i < feature.Fields.FieldCount; i++)
                {
                    IField field = feature.Fields.get_Field(i);
                    if (field.Type != esriFieldType.esriFieldTypeOID && field.Type != esriFieldType.esriFieldTypeGeometry && field.Type != esriFieldType.esriFieldTypeGlobalID && field.Type != esriFieldType.esriFieldTypeGUID)
                    {
                        string fieldName = field.Name;
                        int index = createdFeature.Fields.FindField(fieldName);
                        if (index > -1 && fieldName != "Shape_Length" && fieldName != "Shape_Area")
                            createdFeature.set_Value(index, feature.get_Value(i));
                    }
                }
                createdFeature.Store();
            });
            Console.WriteLine($"{toFeatureClass.FeatureCount(null)} 个要素在目标图层现在");
        }

        /// <summary>
        /// 获取FeatureClass
        /// </summary>
        /// <param name="datasource"></param>
        /// <param name="workspaceType"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IFeatureClass GetFeatureClass(object datasource, WorkspaceType workspaceType, string name)
        {
            switch (workspaceType)
            {
                case WorkspaceType.Gdb:
                    {
                        var fc = GetFeatureClass(datasource.ToString(), name);
                        return fc;
                    }
                    break;
                case WorkspaceType.Sde:
                    {
                        var fc = GetFeatureInSde(((Dictionary<string, object>)datasource).ToPropertySet(), name);
                        return fc;
                    }
                    break;
                default:
                    break;
            }
            return null;
        }
        public static IFeatureClass GetFeatureClass(string gdbPath, string featureClassName)
        {
            FileGDBWorkspaceFactory fileGDBWorkspaceFactory = new FileGDBWorkspaceFactoryClass();
            var workspace = fileGDBWorkspaceFactory.OpenFromFile(gdbPath, 0);
            var fc = ((IFeatureWorkspace)workspace).OpenFeatureClass(featureClassName);
            return fc;
        }

        public static IFeatureClass GetFeatureClass(string datasource, DataSourceType dataSourceType, string featureClassName)
        {
            IWorkspace workspace = null;
            switch (dataSourceType)
            {
                case DataSourceType.SdeFilePath:
                    {
                        SdeWorkspaceFactory sdeWorkspaceFactory = new SdeWorkspaceFactoryClass();
                        workspace = sdeWorkspaceFactory.OpenFromFile(datasource, 1);
                    }
                    break;
                case DataSourceType.SdeTxt:
                    throw new NotImplementedException();
                    break;
                case DataSourceType.SdeJson:
                    throw new NotImplementedException();
                    break;
                case DataSourceType.GdbFilePath:
                    throw new NotImplementedException();
                    break;
                case DataSourceType.ShapefilePath:
                    throw new NotImplementedException();
                    break;
                default:
                    break;
            }
            return ((IFeatureWorkspace)workspace).OpenFeatureClass(featureClassName);
        }

        public static IFeatureClass GetFeatureInSde(IPropertySet properties, string featureClassName)
        {
            SdeWorkspaceFactory sdeWorkspaceFactory = new SdeWorkspaceFactoryClass();
            var workspace = sdeWorkspaceFactory.Open(properties, 0);
            var fc = ((IFeatureWorkspace)workspace).OpenFeatureClass(featureClassName);
            return fc;
        }


        public static IFeatureClass CreateFeatueClass(object datasource, WorkspaceType workspace, string name)
        {
            throw new NotImplementedException();
        }




        public static IFeatureClass CreateFeatureClass(IWorkspace workspace, string datasetname, string name)
        {
            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)workspace;

            var dataset = featureWorkspace.OpenFeatureDataset(datasetname);
            //dataset.CreateFeatureClass(name, )
            throw new NotImplementedException();


        }



    }



    public enum WorkspaceType
    {
        Gdb, Sde
    }
}
