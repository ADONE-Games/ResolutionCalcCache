using UnityEditor;

using UnityEngine;


namespace ADONEGames.ResolutionCalcCache.Editor
{
    /// <summary>
    /// PlayerSettingsに解像度を設定した構造体を生成する項目を追加する
    /// </summary>
    [FilePath( "ProjectSettings/ResolutionCalcCache.asset", FilePathAttribute.Location.ProjectFolder )]
    internal class ProjectSettingData : ScriptableSingleton<ProjectSettingData>, IResolutionData
    {
        [SerializeField]
        private DefaultAsset resolutionDataFolder;

        [SerializeField]
        private string @namespace;

        [SerializeField]
        private string @class;

        [SerializeField]
        private int width;

        [SerializeField]
        private int height;


        /// <summary>
        /// Folder Asset
        /// </summary>
        public DefaultAsset ResolutionDataFolder { get => resolutionDataFolder; set => SetResolutionDataFolder( value ); }

        /// <summary>
        /// コード生成時の名前空間
        /// </summary>
        public string Namespace { get => @namespace; set => @namespace = value; }

        /// <summary>
        /// コード生成時のクラス名
        /// </summary>
        public string Class { get => @class; set => @class = value; }

        /// <summary>
        /// コード生成時の幅
        /// </summary>
        public int Width { get => width; set => width = value; }

        /// <summary>
        /// コード生成時の高さ
        /// </summary>
        public int Height { get => height; set => height = value; }

        /// <summary>
        /// フォルダの設定
        /// </summary>
        /// <param name="folder">Folder Asset</param>
        private void SetResolutionDataFolder( DefaultAsset folder )
        {
            if( folder == null )
            {
                resolutionDataFolder = null;

                return;
            }

            var path = AssetDatabase.GetAssetPath( folder );

            var isDirectory = System.IO.Directory.Exists( path );

            resolutionDataFolder = isDirectory ? folder : null;
        }

        /// <summary>
        /// デフォルトのフォルダを設定する 
        /// </summary>
        public void SetDefaultDirectory()
        {
            if( ResolutionDataFolder != null ) return;

            SetFolderAsset( "Assets" );
        }

        /// <summary>
        /// フォルダを設定する
        /// </summary>
        /// <param name="path"></param>
        public void SetFolderAsset( string path )
        {
            var isDirectory = System.IO.Directory.Exists( path );
            ResolutionDataFolder = isDirectory ? AssetDatabase.LoadAssetAtPath<DefaultAsset>( path ) : null;
        }

        /// <summary>
        /// GUIを描画する
        /// </summary>
        public void OnGUI()
        {
            var folder = EditorGUILayout.ObjectField( "ResolutionDataFolder", ResolutionDataFolder, typeof( DefaultAsset ), false ) as DefaultAsset;

            using( new EditorGUI.DisabledScope( true ) )
                EditorGUILayout.TextField( AssetDatabase.GetAssetPath( folder ) );

            EditorGUILayout.Space();


            Namespace = EditorGUILayout.TextField( "Namespace", Namespace );
            Class     = EditorGUILayout.TextField( "Class",     Class );
            Width     = EditorGUILayout.IntField( "Width",  Width );
            Height    = EditorGUILayout.IntField( "Height", Height );

            if( ResolutionDataFolder != folder )
                ResolutionDataFolder = folder;
        }
    }
}
