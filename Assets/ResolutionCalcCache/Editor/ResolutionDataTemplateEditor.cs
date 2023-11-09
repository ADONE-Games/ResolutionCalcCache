using System.Collections.Generic;
using System.Linq;

using UnityEditor;

using UnityEngine;

namespace ADONEGames.ResolutionCalcCache.Editor
{
    /// <summary>
    /// ResolutionDataのテンプレートを生成するクラス
    /// </summary>
    internal readonly struct ResolutionDataTemplateEditor
    {
        private const string ResolutionCalcCacheKeyword = "ResolutionCalcCache";
        private const string ResolutionDataTemplateKeyword = "ResolutionDataTemplate";
        private const string ResolutionSizeDataTemplateKeyword = "ResolutionSizeDataTemplate";
        private const string ResolutionLevelTemplateKeyword = "ResolutionLevelTemplate";
        private const string ResolutionCategoryTemplateKeyword = "ResolutionCategoryTemplate";
        private const string ResolutionSetupTemplateKeyword = "ResolutionSetupTemplate";

        private const string NamespaceKeyword     = "#NAMESPACE#";
        private const string ClassKeyword         = "#CLASS#";
        private const string SizeDataKeyword      = "#SIZEDATA#";
        private const string SizeDataClassKeyword = "#SIZEDATACLASS#";
        private const string WidthKeyword         = "#WIDTH#";
        private const string HeightKeyword        = "#HEIGHT#";
        private const string AspectKeyword        = "#ASPCT#";
        private const string OrientationKeyword   = "#Orientation#";
        private const string FitDirectionKeyword  = "#FitDirection#";
        private const string LevelsKeyword        = "#LEVELS#";
        private const string CategoryKeyword      = "#CATEGORY#";
        private const string AppendKeyword        = "#APPEND#";

        private readonly ResolutionProjectDataEditor _projectData;
        private readonly string _dataPath;
        private readonly string _resolutionDataTemplatePath;
        private readonly string _resolutionSizeDataTemplatePath;
        private readonly string _resolutionLevelTemplatePath;
        private readonly string _resolutionCategoryTemplatePath;
        private readonly string _resolutionSetupTemplatePath;

        /// <summary>
        /// Constructor
        /// </summary>

        public ResolutionDataTemplateEditor(ResolutionProjectDataEditor data)
        {
            _projectData = data;
            _dataPath = AssetDatabase.GUIDToAssetPath(_projectData.ProjectData.ResolutionDataFolderGUID);

            var paths = AssetDatabase.GetAllAssetPaths().Where( path => path.Contains( ResolutionCalcCacheKeyword ) );
            _resolutionDataTemplatePath = paths.FirstOrDefault( path => path.Contains( ResolutionDataTemplateKeyword ) );
            _resolutionSizeDataTemplatePath = paths.FirstOrDefault( path => path.Contains( ResolutionSizeDataTemplateKeyword ) );
            _resolutionLevelTemplatePath = paths.FirstOrDefault( path => path.Contains( ResolutionLevelTemplateKeyword ) );
            _resolutionCategoryTemplatePath = paths.FirstOrDefault( path => path.Contains( ResolutionCategoryTemplateKeyword ) );
            _resolutionSetupTemplatePath = paths.FirstOrDefault( path => path.Contains( ResolutionSetupTemplateKeyword ) );
        }

        /// <summary>
        ///  Generate ResolutionData
        /// </summary>
        public (List<string> writePaths, bool result) Generate()
        {
            // テンプレートが見つからない場合は生成しない
            if( string.IsNullOrEmpty( _resolutionDataTemplatePath ) || string.IsNullOrEmpty( _resolutionLevelTemplatePath ) || string.IsNullOrEmpty( _resolutionCategoryTemplatePath ) || string.IsNullOrEmpty( _dataPath ) )
                return (null, false);

            var resolutionDatatempText = System.IO.File.ReadAllText( _resolutionDataTemplatePath );
            var resolutionSizeDatatempText = System.IO.File.ReadAllText( _resolutionSizeDataTemplatePath );
            var resolutionLeveltempText = System.IO.File.ReadAllText( _resolutionLevelTemplatePath );
            var resolutionCategorytempText = System.IO.File.ReadAllText( _resolutionCategoryTemplatePath );
            var resolutionSetuptempText = System.IO.File.ReadAllText( _resolutionSetupTemplatePath );

            var writePaths = new List<string>();
            var levelNames = new List<string>();
            var categoryNames = new List<string>();
            var setupAppendText = new List<string>();
            for( var i = 0; i < _projectData.ResolutionCategoryList.Count; i++ )
            {
                categoryNames.Add( $"        {_projectData.ResolutionCategoryList[i]} = {i}," ); // Enumの定義
            }
            for( var i = 0; i < _projectData.ResolutionDataList.Count; i++ )
            {
                ResolutionData resolutionData;
                var levelName = _projectData.ResolutionDataList[i].LevelName;
                var classNames = new List<string>();

                levelNames.Add( $"        {levelName} = {i}," ); // Enumの定義

                resolutionData = _projectData.ResolutionDataList[i];
                classNames.Add( $"{_projectData.ClassName}{levelName}{resolutionData.ResolutionSizeDataList[0].Orientation}" ); // クラス名
                DataWrite( _dataPath, _projectData.NamespaceName, classNames.Last(), resolutionData );

                resolutionData = new ResolutionData( levelName, resolutionData.FitDirection == FitDirection.Horizontal ? FitDirection.Vertical : FitDirection.Horizontal );
                for( var j = 0; j < _projectData.ResolutionCategoryList.Count; j++ )
                {
                    resolutionData.SetSize( _projectData.ResolutionCategoryList[j], _projectData.ResolutionDataList[i].ResolutionSizeDataList[j].Height, _projectData.ResolutionDataList[i].ResolutionSizeDataList[j].Width );
                }
                classNames.Add( $"{_projectData.ClassName}{levelName}{resolutionData.ResolutionSizeDataList[0].Orientation}" ); // クラス名
                DataWrite( _dataPath, _projectData.NamespaceName, classNames.Last(), resolutionData );

                // Setupの引数
                setupAppendText.Add( $"            ResolutionDataProc.Append( (int)ResolutionLevel.{resolutionData.LevelName}, new IResolutionData[] {{ new {classNames[0]}(), new {classNames[1]}(), }} );" );
            }
            LevelWrite( _dataPath, _projectData.NamespaceName, levelNames );
            CategoryWrite( _dataPath, _projectData.NamespaceName, categoryNames );
            SetupWrite( _dataPath, _projectData.NamespaceName, setupAppendText );

            return (writePaths, true);

            void DataWrite( string dirPath, string namespaceName, string className, ResolutionData data )
            {
                // var sizeText = resolutionSizeDatatempText;
                var dataText = resolutionDatatempText;

                var newInstanceText = new List<string>();
                var sizeClassTexts = new List<string>();
                foreach( var sizeData in data.ResolutionSizeDataList )
                {
                    var sizeClassText = resolutionSizeDatatempText;

                    sizeClassText = sizeClassText.Replace( ClassKeyword,       sizeData.CategoryName );
                    sizeClassText = sizeClassText.Replace( WidthKeyword,       sizeData.Width.ToString() );
                    sizeClassText = sizeClassText.Replace( HeightKeyword,      sizeData.Height.ToString() );
                    sizeClassText = sizeClassText.Replace( AspectKeyword,      sizeData.Aspect.ToString("F8") );
                    sizeClassText = sizeClassText.Replace( OrientationKeyword, sizeData.Orientation.ToString() );

                    sizeClassTexts.Add( sizeClassText );

                    newInstanceText.Add( $"new {sizeData.CategoryName}()," );
                }

                dataText = dataText.Replace( NamespaceKeyword,    namespaceName );
                dataText = dataText.Replace( ClassKeyword,        className );
                dataText = dataText.Replace( SizeDataKeyword,     string.Join( " ", newInstanceText ) );
                dataText = dataText.Replace( SizeDataClassKeyword,string.Join( "\n", sizeClassTexts ) );
                dataText = dataText.Replace( FitDirectionKeyword, data.FitDirection.ToString() );


                // var text = resolutionDatatempText;

                // text = text.Replace( NamespaceKeyword, namespaceName );
                // text = text.Replace( ClassKeyword,     className );
                // // text = text.Replace( WidthKeyword,     data.CanvasWidth.ToString() );
                // // text = text.Replace( HeightKeyword,    data.CanvasHeight.ToString() );
                // // text = text.Replace( AspectKeyword,    $"{data.Aspect}f" );
                // // text = text.Replace( OrientationKeyword, data.Orientation.ToString() );
                // text = text.Replace( FitDirectionKeyword, data.FitDirection.ToString() );

                var writePath = $"{dirPath}/{className}.cs";
                System.IO.File.WriteAllText( writePath, dataText );

                writePaths.Add( writePath );
            }
            void LevelWrite( string dirPath, string namespaceName, List<string> levelNames )
            {
                var text = resolutionLeveltempText;

                text = text.Replace( NamespaceKeyword, namespaceName );
                text = text.Replace( LevelsKeyword,    string.Join( "\n", levelNames ) );

                var writePath = $"{dirPath}/ResolutionLevel.cs";
                System.IO.File.WriteAllText( writePath, text );

                writePaths.Add( writePath );
            }
            void CategoryWrite( string dirPath, string namespaceName, List<string> categoryNames )
            {
                var text = resolutionCategorytempText;

                text = text.Replace( NamespaceKeyword, namespaceName );
                text = text.Replace( CategoryKeyword,  string.Join( "\n", categoryNames ) );

                var writePath = $"{dirPath}/ResolutionCategory.cs";
                System.IO.File.WriteAllText( writePath, text );

                writePaths.Add( writePath );
            }
            void SetupWrite( string dirPath, string namespaceName, List<string> setupAppendText )
            {
                var text = resolutionSetuptempText;

                text = text.Replace( NamespaceKeyword, namespaceName );
                text = text.Replace( AppendKeyword,    string.Join( "\n", setupAppendText ) );

                var writePath = $"{dirPath}/ResolutionSetup.cs";
                System.IO.File.WriteAllText( writePath, text );

                writePaths.Add( writePath );
            }
        }
    }
}
