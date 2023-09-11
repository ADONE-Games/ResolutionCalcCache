using UnityEditor;

namespace ADONEGames.ResolutionCalcCache.Editor
{
    /// <summary>
    /// ResolutionDataのテンプレートを生成するクラス
    /// </summary>
    internal readonly struct ResolutionDataTemplateEditor
    {
        private const string ResolutionDataTemplatePathKeyword = "/ResolutionCalcCache/Editor/ResolutionDataTemplate";

        private const string NamespaceKeyword = "#NAMESPACE#";
        private const string ClassKeyword     = "#CLASS#";
        private const string WidthKeyword     = "#WIDTH#";
        private const string HeightKeyword    = "#HEIGHT#";

        private readonly ProjectSettingData _data;
        private readonly string             _dataPath;
        private readonly string             _filePath;
        private readonly string             _templatePath;

        /// <summary>
        /// Constructor
        /// </summary>
        public ResolutionDataTemplateEditor( ProjectSettingData data )
        {
            _data         = data;
            _dataPath     = AssetDatabase.GetAssetPath( _data.ResolutionDataFolder );
            _filePath     = $"{_data.Class}.cs";
            _templatePath = SearchTemplatePath();

            return;

            // Local Functions
            string SearchTemplatePath()
            {
                var paths = AssetDatabase.GetAllAssetPaths();

                foreach( var path in paths )
                {
                    if( !path.Contains( ResolutionDataTemplatePathKeyword ) ) continue;

                    return path;
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Generate ResolutionData
        /// </summary>
        public (string writePath, bool result) Generate()
        {
            // テンプレートが見つからない場合は生成しない
            if( string.IsNullOrEmpty( _templatePath ) || string.IsNullOrEmpty( _dataPath ) )
                return (string.Empty, false);

            var text = System.IO.File.ReadAllText( _templatePath );

            text = text.Replace( NamespaceKeyword, _data.Namespace );
            text = text.Replace( ClassKeyword,     _data.Class );
            text = text.Replace( WidthKeyword,     _data.Width.ToString() );
            text = text.Replace( HeightKeyword,    _data.Height.ToString() );

            var writePath = $"{_dataPath}/{_filePath}";
            System.IO.File.WriteAllText( writePath, text );

            return (writePath, true);
        }
    }
}
