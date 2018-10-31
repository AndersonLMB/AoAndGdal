using System.Collections.Generic;
using System;
using McMaster.Extensions.CommandLineUtils;
using AoCli.AoActions;
using System.Linq;
using System.IO;

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

        [Option(ShortName = "f", Description = "The file to operate")]
        public string File { get; set; }

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
                            if (ex is FileNotFoundException)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            sa = null;
                        }
                        var inFc = DataActions.GetFeatureClass(Datasource, DataSourceType, LayerName);
                        var outFc = DataActions.GetFeatureClass(OutDatasource, OutDatasourceType, OutLayerName);
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
        Adjust, ImportData, LogLayer
    }
}
