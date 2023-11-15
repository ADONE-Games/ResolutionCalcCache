using System.Text;

using UnityEditor;

using UnityEngine;


namespace ADONEGames.ResolutionCalcCache.Editor
{
    /// <summary>
    /// Adds an item to generate a structure that sets the resolution in PlayerSettings.
    /// </summary>
    /// <remarks>
    /// PlayerSettingsに解像度を設定した構造体を生成する項目を追加する。
    /// </remarks>
    internal class ProjectSetting : SettingsProvider
    {
        private const string ProjectSettingPath = "Project/ADONEGames-Tools/Generator/ResolutionCalcCache-ResolutionData";
        private readonly string[] _projectSettingKeywords = { "ResolutionCalcCache", "ResolutionData" };

        private ResolutionProjectDataEditor _resolutionProjectDataEditor;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <remarks>
        /// このコンストラクタは、指定されたパスと設定スコープで ProjectSetting クラスの新しいインスタンスを初期化します。
        /// </remarks>
        /// <param name="path">設定ファイルのパス</param>
        /// <param name="scopes">設定のスコープ</param>
        private ProjectSetting( string path, SettingsScope scopes = SettingsScope.Project ) : base( path, scopes )
        {
            _resolutionProjectDataEditor = _resolutionProjectDataEditor != null ? _resolutionProjectDataEditor : ResolutionProjectDataEditor.CreateOrLoad();
            _resolutionProjectDataEditor.SetDefaultDirectory();
        }

        /// <summary>
        /// GUI
        /// </summary>
        /// <param name="searchContext"></param>
        public override void OnGUI( string searchContext )
        {
            EditorGUILayout.Space();

            _resolutionProjectDataEditor.OnGUI();
            using( new EditorGUI.DisabledScope( _resolutionProjectDataEditor.ResolutionDataFolder == null ) )
            using( new EditorGUILayout.HorizontalScope() )
            {
                GUILayout.FlexibleSpace();

                // 生成ボタン
                if( GUILayout.Button( "Generate" ) )
                {
                    // 生成
                    var result = new ResolutionDataTemplateEditor( _resolutionProjectDataEditor ).Generate();

                    {
                        // 通知
                        var assembly = typeof( EditorWindow ).Assembly;
                        var type     = assembly.GetType( "UnityEditor.ProjectSettingsWindow" );

                        var sb = new StringBuilder();
                        if (result.writePaths != null)
                        {
                            foreach (var path in result.writePaths)
                            {
                                sb.AppendLine(path);
                            }
                            var message = $"Successful file generation\n\n{sb}";
                            EditorWindow.GetWindow( type ).ShowNotification( new GUIContent( message ) );
                        }
                        else
                        {
                            EditorWindow.GetWindow( type ).ShowNotification( new GUIContent( $"File Generation Failure" ) );
                        }

                        EditorWindow.GetWindow( type ).ShowNotification( new GUIContent( result.result ? $"Successful file generation\n\n{sb}" : $"File generation failure" ) );
                    }

                    // 更新
                    AssetDatabase.Refresh();
                }
                EditorGUILayout.Space(10);
            }

            EditorGUILayout.Space();

            // EditorGUILayout.LabelField( "aaa" );
            // var dw = Display.main.colorBuffer;
            // var dh = Display.main.systemHeight;
            // EditorGUILayout.LabelField( $"Display.main.systemWidth: {Display.main.systemWidth}, Display.main.systemHeight: {Display.main.systemHeight}" );
            // EditorGUILayout.LabelField( $"Screen.width: {Screen.width}, Screen.height: {Screen.height}" );
            // EditorGUILayout.LabelField( $"Screen.mainWindowDisplayInfo.width: {Screen.mainWindowDisplayInfo.width}, Screen.mainWindowDisplayInfo.height: {Screen.mainWindowDisplayInfo.height}" );

            // var typeResult = false;
            // foreach( var assembly in AppDomain.CurrentDomain.GetAssemblies() )
            // {
            //     foreach( var type in assembly.GetTypes() )
            //     {
            //         if( !type.Name.Equals( "ResolutionCategory" ) )
            //             continue;

            //         typeResult = true;
            //         break;
            //     }
            //     if( typeResult )
            //         break;
            // }
            // if( typeResult )
            // {
            //     EditorGUILayout.LabelField( $"ResolutionCategory: {typeResult}" );
            // }
            // else
            // {
            //     EditorGUILayout.LabelField( $"ResolutionCategory: null" );
            // }

        }

        /// <summary>
        /// Generates a SettingsProvider.
        /// </summary>
        /// <remarks>
        /// SettingsProviderを生成する
        /// </remarks>
        /// <returns>SettingsProvider</returns>
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            var provider = new ProjectSetting( ProjectSettingPath, SettingsScope.Project );
            provider.keywords = provider._projectSettingKeywords;

            return provider;
        }
    }
}
