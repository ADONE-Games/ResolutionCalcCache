using System;

using UnityEditor;

namespace ADONEGames.ResolutionCalcCache.AutoResolution
{
    [CustomEditor( typeof( AutoResolutionInCameraRTexSetter ) )]
    public class AutoResolutionInCameraRTexSetterEditor : UnityEditor.Editor
    {
        private Type ResolutionLevelType { get; set; }
        private Type ResolutionCategoryType { get; set; }


        private void OnEnable() 
        {
            ResolutionLevelType = null;
            ResolutionCategoryType = null;
            foreach( var assembly in AppDomain.CurrentDomain.GetAssemblies() )
            {
                foreach( var type in assembly.GetTypes() )
                {
                    if( ResolutionCategoryType == null && type.Name.Equals( "ResolutionCategory" ) )
                        ResolutionCategoryType = type;

                    if( ResolutionLevelType == null && type.Name.Equals( "ResolutionLevel" ) )
                        ResolutionLevelType = type;

                    if( ResolutionCategoryType != null && ResolutionLevelType != null )
                        break;
                }
                if( ResolutionCategoryType != null && ResolutionLevelType != null )
                    break;
            }
            
        }

        public override void OnInspectorGUI()
        {
            if( ResolutionLevelType == null || ResolutionCategoryType == null )
            {
                EditorGUILayout.HelpBox( "ResolutionLevelType or ResolutionCategoryType is null", MessageType.Error );
                return;
            }

            serializedObject.Update();

            var categoryIndex = serializedObject.FindProperty( "_categoryIndex" );
            categoryIndex.intValue = EditorGUILayout.Popup( categoryIndex.intValue, Enum.GetNames( ResolutionCategoryType ) );

            serializedObject.ApplyModifiedProperties();
        }
    }
}

