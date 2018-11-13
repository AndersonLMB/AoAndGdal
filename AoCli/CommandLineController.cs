using AoCli.AoActions;
using ESRI.ArcGIS.Geodatabase;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AoCli
{
    public class CommandLineController
    {
        public Dictionary<ActionTypes, Action> map = new Dictionary<ActionTypes, Action>();

        /// <summary>
        /// 动作类型
        /// </summary>
        [Argument(0, Description = "The action you would like to execute. <Adjust|ImportData|LogLayer>")]
        public ActionTypes ActionArgument { get; set; }

        /// <summary>
        /// 控制点输入类型
        /// </summary>
        [Option(ShortName = "cpt", LongName = "controlpointstype", Description = "The way you pass control points. <File|Web>")]
        public ControlPointsInputType ControlPointsInputType { get; set; }

        /// <summary>
        /// 控制点输入文件或者地址
        /// </summary>
        [Option(ShortName = "cp", LongName = "controlpoints", Description = "The path or url to pass control points.")]
        public string ControlPoints { get; set; }

        // Affine, Conformal, EdgeSnap, Piecewise, Projective
        [Option(ShortName = "samt", Description = "Spatial adjustment method type. Default option is Affine. <Affine|Conformal|EdgeSnap|Piecewise|Projective>")]
        public SpatialAdjustMethodType SpatialAdjustMethodType { get; set; } = SpatialAdjustMethodType.Affine;

        /// <summary>
        /// 图层的数据源
        /// </summary>
        [Option(ShortName = "ds", Description = "Data source of Layer")]
        public string Datasource { get; set; }

        /// <summary>
        /// 数据源类型
        /// </summary>
        [Option(ShortName = "dst", Description = "Data source type. <SdeFilePath|SdeTxt|SdeJson|GdbFilePath|ShapefilePath>")]
        public DataSourceType DataSourceType { get; set; }

        /// <summary>
        /// 图层名称
        /// </summary>
        [Option(ShortName = "ln", Description = "Layer name")]
        public string LayerName { get; set; }

        [Option(ShortName = "ods", Description = "Output Data source of Layer")]
        public string OutDatasource { get; set; }

        [Option(ShortName = "odst", Description = "Output Data source type. <SdeFilePath|SdeTxt|SdeJson|GdbFilePath|ShapefilePath>")]
        public DataSourceType OutDatasourceType { get; set; }

        [Option(ShortName = "oln", Description = "Output Layer name")]
        public string OutLayerName { get; set; }

        [Option(ShortName = "bfn", Description = "Batch file name")]
        public string BatchFileName { get; set; }
        /// <summary>
        /// 数据源图层名
        /// </summary>
        public void OnExecute()
        {
            switch (ActionArgument)
            {
                case ActionTypes.Adjust:
                    {
                        new ArcEngineLicense();
                        SpatialAdjust.Adjust();
                        break;
                    }
                case ActionTypes.ImportData:
                    {
                        if (new string[] { Datasource, LayerName, OutDatasource, OutLayerName, ControlPoints }.Any(s => s == null))
                        {
                            Console.WriteLine("参数没有写全");
                            break;
                            //throw new Exception("参数没有写全");
                        }
                        new ArcEngineLicense();
                        SpatialAdjust sa = null;
                        try
                        {
                            sa = new SpatialAdjust(controlPointsFile: ControlPoints, controlPointsInputType: ControlPointsInputType, spatialAdjustMethodType: SpatialAdjustMethodType);
                        }
                        catch (Exception ex)
                        {
                            sa = null;
                        }
                        var inFc = DataActions.GetFeatureClass(Datasource, DataSourceType, LayerName);
                        ESRI.ArcGIS.Geodatabase.IFeatureClass outFc;
                        try
                        {
                            outFc = DataActions.GetFeatureClass(OutDatasource, OutDatasourceType, OutLayerName);
                        }
                        catch (Exception)
                        {
                            outFc = DataActions.CreateFeatureClass(DataActions.GetWorkspace(OutDatasource, OutDatasourceType), "", OutLayerName, inFc);
                        }
                        //Console.WriteLine($"将使用{ SpatialAdjust.transformMethodMap[sa.SpatialAdjustMethodType].Name} 偏移");
                        DataActions.CoverFeatureClassWithFeatureClass(inFc, outFc, sa);
                        break;
                    }
                case ActionTypes.LogLayer:
                    {
                        new ArcEngineLicense();
                        LogActions.LogFeatureClass(Datasource, DataSourceType, LayerName);
                    }
                    break;
                case ActionTypes.BatchImport:
                    {
                        new ArcEngineLicense();
                        var layerNames = LayerName.Split(',');
                        List<string> finalNameStringList = new List<string>();
                        var workspace = DataActions.GetWorkspace(Datasource, DataSourceType);
                        var featureWorkspace = (IFeatureWorkspace)workspace;
                        layerNames.ToList().ForEach((name) =>
                        {
                            try
                            {
                                featureWorkspace.OpenFeatureClass(name);
                                finalNameStringList.Add(name);
                            }
                            catch (Exception)
                            {
                                try
                                {
                                    var dataset = featureWorkspace.OpenFeatureDataset(name);
                                    var subs = dataset.FeatureDatasetsInFeatureDataset();
                                    subs.ToList().ForEach(sub => finalNameStringList.Add(sub.Name));
                                }
                                catch (Exception)
                                {
                                }
                            }
                        });
                        finalNameStringList.ForEach((finalName) =>
                        {
                            new CommandLineController()
                            {
                                ActionArgument = ActionTypes.ImportData,
                                Datasource = Datasource,
                                DataSourceType = DataSourceType,
                                LayerName = finalName,
                                OutDatasource = OutDatasource,
                                OutDatasourceType = OutDatasourceType,
                                OutLayerName = $"{finalName.Split('.').Last()}_adjusted",
                                ControlPoints = ControlPoints,
                                ControlPointsInputType = ControlPointsInputType,
                                SpatialAdjustMethodType = SpatialAdjustMethodType
                            }.OnExecute();
                        });
                    }
                    break;
                case ActionTypes.GenerateBatch:
                    {
                        new ArcEngineLicense();
                        var batchFileName = String.Empty;
                        //var dtn = DateTime.Now.ToFullTimeSecondString();
                        if (String.IsNullOrEmpty(BatchFileName))
                        {
                            var dtn = DateTime.Now;
                            var s = dtn.ToBinary();
                            batchFileName = $"bat{s}.bat";
                        }
                        else
                        {
                            batchFileName = BatchFileName;
                        }
                        var layerNames = LayerName.Split(',');
                        List<string> finalNameStringList = new List<string>();
                        var workspace = DataActions.GetWorkspace(Datasource, DataSourceType);
                        var featureWorkspace = (IFeatureWorkspace)workspace;
                        layerNames.ToList().ForEach((name) =>
                        {
                            try
                            {
                                featureWorkspace.OpenFeatureClass(name);
                                finalNameStringList.Add(name);
                            }
                            catch (Exception)
                            {
                                try
                                {
                                    var dataset = featureWorkspace.OpenFeatureDataset(name);
                                    var subs = dataset.FeatureDatasetsInFeatureDataset();
                                    subs.ToList().ForEach(sub => finalNameStringList.Add(sub.Name));
                                }
                                catch (Exception)
                                {
                                }
                            }
                        });


                        using (StreamWriter sw = new StreamWriter(batchFileName, true, Encoding.Default))
                        //using (StreamWriter sw = File.AppendText(batchFileName))
                        {
                            var text = String.Empty;

                            finalNameStringList.ToList().ForEach((layerName) =>
                            {
                                //System.Reflection.Assembly.GetExecutingAssembly().FullName;
                                //sw.WriteLine( $"AoCli.exe "  )
                                sw.WriteLine($@"AoCli.exe BatchImport -ln {layerName} -ds {Datasource} -dst {DataSourceType} -ods {OutDatasource} -odst {OutDatasourceType} -oln {OutLayerName} -cp {ControlPoints} -samt {SpatialAdjustMethodType}" + " \n");
                                //text += $@"AoCli.exe BatchImport -ln {layerName} -ds {Datasource} -dst {DataSourceType} -ods {OutDatasource} -oln {OutLayerName} -cp {ControlPoints} -samt {SpatialAdjustMethodType}";
                                //text += "\n";
                            });
                            sw.Write(text);


                            Console.WriteLine("Batch file generated");
                        }

                    }
                    break;
                default:
                    break;
            }
        }
    }

    public enum ControlPointsInputType
    {
        File, Web
    }

    public enum ActionTypes
    {
        Adjust, ImportData, LogLayer, BatchImport, GenerateBatch
    }
}