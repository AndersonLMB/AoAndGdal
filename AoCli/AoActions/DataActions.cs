using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
            var count = fromFeatureClass.FeatureCount(null);
            var breakpoints = new List<double>() { 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9 };
            var progressDictionary = breakpoints.ToDictionary(d => d, d => false);


            var finishedCount = 0;
            Console.WriteLine($"{fromFeatureClass.FeatureCount(null)} 个要素待转换 在{((IDataset)fromFeatureClass).Name}");
            if (spatialAdjust != null) Console.WriteLine($"使用 {spatialAdjust.TransformationMethod.Name} 方法偏移");
            else Console.WriteLine("不偏移");
            Action<IFeature> action = (feature) =>
               {

                   //esriFlowDirection.

                   //UInt32.MaxValue
                   //4294967295
                   var createdFeature = toFeatureClass.CreateFeature();
                   var copy = feature.ShapeCopy;
                   if (spatialAdjust != null && feature.Shape != null)
                   {
                       //Console.WriteLine($"使用{ SpatialAdjust.transformMethodMap[spatialAdjust.SpatialAdjustMethodType].Name}来Spatial Adjustment");
                       var targetSr = createdFeature.Shape.SpatialReference;
                       try
                       {
                           copy.Project(targetSr);
                       }
                       catch (Exception ex)
                       {
                           Console.WriteLine(ex.Message);
                           //throw;
                       }
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
                           if (index > -1 && fieldName != "Shape_Length" && fieldName != "Shape_Area" && field.Editable)
                               createdFeature.set_Value(index, feature.get_Value(i));
                       }
                   }
                   createdFeature.Store();
                   finishedCount++;
                   double progress = ((double)finishedCount) / ((double)count);
                   progressDictionary.ToList().ForEach((kvp) =>
                   {
                       if (progress > kvp.Key && kvp.Value == false)
                       {
                           progressDictionary[kvp.Key] = true;
                           Console.Write($"{kvp.Key} ");
                       }

                   });
               };

            IFeatureCursor featureCursor = fromFeatureClass.Update(null, false);
            IFeature targetFeature;
            while ((targetFeature = featureCursor.NextFeature()) != null)
            {
                action.Invoke(targetFeature);
            }

            //fromFeatureClass.
            //for (int i = 0; i < count; i++)
            //{
            //    fromFeatureClass.Update()
            //    action.Invoke(fromFeatureClass.GetFeature(i));
            //}


            //fromFeatureClass.Features().ToList().ForEach(action);
            Console.Write("\n");
            Console.WriteLine($"{toFeatureClass.FeatureCount(null)} 个要素在目标图层 在{((IDataset)toFeatureClass).Name}");
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
                    {
                        FileGDBWorkspaceFactory fileGDBWorkspaceFactory = new FileGDBWorkspaceFactoryClass();
                        workspace = fileGDBWorkspaceFactory.OpenFromFile(datasource, 1);
                    }
                    //throw new NotImplementedException();
                    break;

                case DataSourceType.ShapefilePath:
                    throw new NotImplementedException();
                    break;

                default:
                    break;
            }

            var featureWorkspace = (IFeatureWorkspace)workspace;


            IFeatureClass tryOpenFeatureClass;
            try
            {
                tryOpenFeatureClass = featureWorkspace.OpenFeatureClass(featureClassName);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //((IFeatureWorkspace)workspace).CreateFeatureClass()

            return featureWorkspace.OpenFeatureClass(featureClassName);
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

        public static IFeatureClass CreateFeatureClass(IWorkspace workspace, string datasetname, string name, IFeatureClass refFeatureClass)
        {
            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)workspace;
            string username = null;
            string checkname = null;
            try
            {
                username = workspace.ConnectionProperties.ToDictionary()["USERNAME"].ToString();
                checkname = $"{username}.{name}";
            }
            catch (Exception)
            {
                checkname = name;
                //throw;
            }

            var exist = CheckFeatureClassExist(checkname, workspace);
            IFeatureClass createdFeatureClass;
            if (!exist)
            {
                createdFeatureClass = featureWorkspace.CreateFeatureClass($"{name}", refFeatureClass.Fields, null, null, refFeatureClass.FeatureType, refFeatureClass.ShapeFieldName, "");
                return createdFeatureClass;
            }

            throw new NotImplementedException();
        }

        public static IWorkspace GetWorkspace(string datasource, DataSourceType datasourceType)
        {
            switch (datasourceType)
            {
                case DataSourceType.SdeFilePath:
                    {
                        SdeWorkspaceFactory sdeWf = new SdeWorkspaceFactoryClass();
                        return sdeWf.OpenFromFile(datasource, 0);
                    }
                case DataSourceType.SdeTxt: throw new NotImplementedException();
                case DataSourceType.SdeJson: throw new NotImplementedException();
                case DataSourceType.GdbFilePath:
                    {
                        FileGDBWorkspaceFactory fileWf = new FileGDBWorkspaceFactoryClass();
                        return fileWf.OpenFromFile(datasource, 0);
                    }
                case DataSourceType.ShapefilePath: throw new NotImplementedException();
                default:
                    return null;
                    break;
            }
        }

        public static IWorkspace GetSdeWorkspace(object psObj)
        {
            var dic = (Dictionary<string, object>)psObj;
            var ps = dic.ToPropertySet();
            SdeWorkspaceFactory wf = new SdeWorkspaceFactoryClass();
            var ws = wf.Open(ps, 0);
            return ws;
        }
        /// <summary>
        /// 检查FeatureClass是否存在
        /// </summary>
        /// <param name="checkname"></param>
        /// <param name="workspace"></param>
        /// <returns></returns>
        public static bool CheckFeatureClassExist(string checkname, IWorkspace workspace)
        {
            try
            {
                ((IFeatureWorkspace)workspace).OpenFeatureClass(checkname);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public enum WorkspaceType
    {
        Gdb, Sde
    }
}