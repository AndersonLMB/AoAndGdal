using System;
using System.Collections.Generic;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace AoCli
{
    public static class Util
    {
        public static string ToFullTimeSecondString(this DateTime dateTime)
        {
            return String.Format("{0:D4}/{1:D2}/{2:D2}-{3:D2}:{4:D2}:{5:D2}.{6:D3}", dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond);
        }
    }

    public static class AoUtil
    {
        public static IPropertySet ToPropertySet(this Dictionary<string, object> keyValuePairs)
        {
            var ps = new PropertySetClass();
            foreach (var item in keyValuePairs)
            {
                ps.SetProperty(item.Key, item.Value);
            }
            return ps;
        }

        public static IEnumerable<IDataset> FeatureDatasetsInWorkspace(this IWorkspace workspace, string userFilter)
        {
            var datasets = workspace.Datasets[esriDatasetType.esriDTFeatureDataset];
            IDataset dataset = null;
            while ((dataset = datasets.Next()) != null)
            {
                yield return dataset;
            }
        }
        public static IEnumerable<IDataset> FeatureDatasetsInWorkspace(this IWorkspace workspace, string userFilter, esriDatasetType esriDatasetType)
        {
            var datasets = workspace.Datasets[esriDatasetType];
            IDataset dataset = null;
            while ((dataset = datasets.Next()) != null)
            {
                yield return dataset;
            }
        }

        public static IEnumerable<IPixelBlock> PixelBlocks(this IRaster raster)
        {
            var cursor = raster.CreateCursor();

            while (cursor.Next())
            {
                yield return cursor.PixelBlock;
            }
        }

        public static IEnumerable<IFeatureClass> FeatureClassedInWorkspace(this IWorkspace workspace)
        {
            var featureDatasetsOnRoot = workspace.FeatureDatasetsInWorkspace("", esriDatasetType.esriDTFeatureDataset);
            var featureClassOnRoot = workspace.FeatureDatasetsInWorkspace("", esriDatasetType.esriDTFeatureClass);
            throw new NotImplementedException();
        }

        public static IEnumerable<IDataset> FeatureDatasetsInFeatureDataset(this IDataset dataset)
        {
            var subsets = dataset.Subsets;
            IDataset subDataset = null;
            while ((subDataset = subsets.Next()) != null)
            {
                yield return subDataset;
            }
        }

        public static IEnumerable<IFeature> Features(this IFeatureClass featureClass)
        {
            var featureCursor = featureClass.Update(null, false);
            IFeature feature = null;
            while ((feature = featureCursor.NextFeature()) != null)
            {
                yield return feature;
            }
            //var count = featureClass.FeatureCount(null);
            //for (int i = 0; i < count; i++)
            //{
            //    var f = featureClass.GetFeature(i);
            //    yield return f;
            //}
            //featureClass.Indexes.Index[0];         

            //throw new NotImplementedException();

        }

        public static IEnumerable<IField> Fields(this IFields fields, bool filterEditable)
        {
            var count = fields.FieldCount;
            for (int i = 0; i < count; i++)
            {
                var field = fields.Field[i];
                if (filterEditable)
                {
                    if (field.Editable)
                    {
                        yield return field;
                    }
                }
                else
                {
                    yield return field;
                }

            }

        }

        


    }



}
