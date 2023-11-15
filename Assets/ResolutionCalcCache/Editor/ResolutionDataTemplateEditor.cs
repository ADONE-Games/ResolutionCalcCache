using System.Collections.Generic;
using System.Linq;

using UnityEditor;


namespace ADONEGames.ResolutionCalcCache.Editor
{
    /// <summary>
    /// ResolutionDataのテンプレートを生成するクラス
    /// </summary>
    internal readonly struct ResolutionDataTemplateEditor
    {
        private const string ResolutionCalcCacheKeyword              = "ResolutionCalcCache";
        private const string ResolutionDataTemplateKeyword           = "Template/ResolutionDataTemplate";
        private const string ResolutionSizeDataTemplateKeyword       = "Template/ResolutionSizeDataTemplate";
        private const string ResolutionLevelTemplateKeyword          = "Template/ResolutionLevelTemplate";
        private const string ResolutionCategoryTemplateKeyword       = "Template/ResolutionCategoryTemplate";
        private const string ResolutionDataProcAppendTemplateKeyword = "Template/ResolutionDataProcAppendTemplate";
        private const string ResolutionSetupTemplateKeyword          = "Template/ResolutionSetupTemplate";

        private const string NamespaceKeyword     = "#NAMESPACE#";
        private const string ClassKeyword         = "#CLASS#";
        private const string DataClassKeyword     = "#DATACLASS#";
        private const string SizeDataKeyword      = "#SIZEDATA#";
        private const string SizeDataClassKeyword = "#SIZEDATACLASS#";
        private const string WidthKeyword         = "#WIDTH#";
        private const string HeightKeyword        = "#HEIGHT#";
        private const string AspectKeyword        = "#ASPCT#";
        private const string DepthKeyword         = "#DEPTH#";
        private const string TextureFormatKeyword = "#TEXTUREFORMAT#";
        private const string OrientationKeyword   = "#ORIENTATION#";
        private const string FitDirectionKeyword  = "#FitDirection#";
        private const string LevelsKeyword        = "#LEVELS#";
        private const string CategoryKeyword      = "#CATEGORY#";
        private const string AppendKeyword        = "#APPEND#";
        private const string LevelKeyword         = "#LEVEL#";

        private readonly ResolutionProjectDataEditor _projectData;
        private readonly string _dataPath;
        private readonly string _resolutionDataTemplatePath;
        private readonly string _resolutionSizeDataTemplatePath;
        private readonly string _resolutionLevelTemplatePath;
        private readonly string _resolutionCategoryTemplatePath;
        private readonly string _resolutionDataProcAppendTemplatePath;
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
            _resolutionDataProcAppendTemplatePath = paths.FirstOrDefault( path => path.Contains( ResolutionDataProcAppendTemplateKeyword ) );
            _resolutionSetupTemplatePath = paths.FirstOrDefault( path => path.Contains( ResolutionSetupTemplateKeyword ) );
        }

        /// <summary>
        /// Generate ResolutionData
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
            var resolutionDataProcAppendtempText = System.IO.File.ReadAllText( _resolutionDataProcAppendTemplatePath );
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
                var dataClassTexts = new List<string>();

                levelNames.Add( $"        {levelName} = {i}," ); // Enumの定義

                // data
                resolutionData = _projectData.ResolutionDataList[i];
                classNames.Add( $"{_projectData.ClassName}{levelName}{resolutionData.ResolutionSizeDataList[0].Orientation}" ); // クラス名
                dataClassTexts.Add( DataText( _dataPath, _projectData.NamespaceName, classNames.Last(), resolutionData ) );

                // data
                resolutionData = new ResolutionData( levelName, resolutionData.FitDirection == FitDirection.Horizontal ? FitDirection.Vertical : FitDirection.Horizontal );
                for( var j = 0; j < _projectData.ResolutionCategoryList.Count; j++ )
                {
                    resolutionData.SetSize( _projectData.ResolutionCategoryList[j], _projectData.ResolutionDataList[i].ResolutionSizeDataList[j].Height, _projectData.ResolutionDataList[i].ResolutionSizeDataList[j].Width, _projectData.ResolutionDataList[i].ResolutionSizeDataList[j].Depth, _projectData.ResolutionDataList[i].ResolutionSizeDataList[j].Format );
                }
                classNames.Add( $"{_projectData.ClassName}{levelName}{resolutionData.ResolutionSizeDataList[0].Orientation}" ); // クラス名
                dataClassTexts.Add( DataText( _dataPath, _projectData.NamespaceName, classNames.Last(), resolutionData ) );

                // setup
                setupAppendText.Add( AppendText( dataClassTexts, levelName ) );
            }
            LevelWrite( _dataPath, _projectData.NamespaceName, levelNames );
            CategoryWrite( _dataPath, _projectData.NamespaceName, categoryNames );
            SetupWrite( _dataPath, _projectData.NamespaceName, setupAppendText );

            return (writePaths, true);

            string DataText( string dirPath, string namespaceName, string className, ResolutionData data )
            {
                var sizeClassTexts = new List<string>();
                foreach( var sizeData in data.ResolutionSizeDataList )
                {
                    sizeClassTexts.Add( SizeDataText( sizeData ) );
                }
                
                var dataText = resolutionDatatempText;
                dataText = dataText.Replace( SizeDataClassKeyword, string.Join( " ", sizeClassTexts ) );
                dataText = dataText.Replace( FitDirectionKeyword,  data.FitDirection.ToString() );

                return dataText;
            }
            string SizeDataText( ResolutionSizeData data )
            {
                var sizeText = resolutionSizeDatatempText;

                sizeText = sizeText.Replace( WidthKeyword,       data.Width.ToString() );
                sizeText = sizeText.Replace( HeightKeyword,      data.Height.ToString() );
                sizeText = sizeText.Replace( AspectKeyword,      data.Aspect.ToString("F8") );
                sizeText = sizeText.Replace( DepthKeyword,       data.Depth.ToString() );
                sizeText = sizeText.Replace( TextureFormatKeyword,      data.Format.ToString() );
                sizeText = sizeText.Replace( OrientationKeyword, data.Orientation.ToString() );

                return sizeText;
            }

            string AppendText( List<string> dataClassTexts, string levelName )
            {
                var appendText = resolutionDataProcAppendtempText;
                appendText = appendText.Replace( LevelKeyword, levelName );
                appendText = appendText.Replace( DataClassKeyword, string.Join( "\n", dataClassTexts ) );

                return appendText;
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
