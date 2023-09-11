using UnityEditor;

using UnityEngine;

namespace ADONEGames.ResolutionCalcCache.Editor
{
    /// <summary>
    /// PlayerSettingsに解像度を設定した構造体を生成する項目を追加する
    /// </summary>
    internal class ProjectSetting : SettingsProvider
    {
        private const string ProjectSettingPath = "Project/ADONEGames-Tools/Generator/ResolutionCalcCache-ResolutionData";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path"></param>
        /// <param name="scopes"></param>
        private ProjectSetting( string path, SettingsScope scopes = SettingsScope.Project ) : base( path, scopes )
        {
            ProjectSettingData.instance.SetDefaultDirectory();
        }

        /// <summary>
        /// GUI
        /// </summary>
        /// <param name="searchContext"></param>
        public override void OnGUI( string searchContext )
        {
            ProjectSettingData.instance.OnGUI();

            using( new EditorGUI.DisabledScope( ProjectSettingData.instance.ResolutionDataFolder == null ) )
            using( new EditorGUILayout.HorizontalScope() )
            {
                GUILayout.FlexibleSpace();

                // 生成ボタン
                if( !GUILayout.Button( "Generate" ) )
                    return;

                // 生成
                var result = new ResolutionDataTemplateEditor( ProjectSettingData.instance ).Generate();

                {
                    // 通知
                    var assembly = typeof( EditorWindow ).Assembly;
                    var type     = assembly.GetType( "UnityEditor.ProjectSettingsWindow" );

                    EditorWindow.GetWindow( type ).ShowNotification( new GUIContent( result.result ? $"{result.writePath}\n\nSuccessful file generation" : $"{result.writePath}\n\nFile generation failure" ) );
                }
                

                // 更新
                AssetDatabase.Refresh();
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField( "" );
        }

        /// <summary>
        /// SettingsProviderを生成する
        /// </summary>
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            return new ProjectSetting( ProjectSettingPath );
        }
    }
}
