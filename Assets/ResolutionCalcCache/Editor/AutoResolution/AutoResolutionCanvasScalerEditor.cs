using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

namespace ADONEGames.ResolutionCalcCache.AutoResolution
{
    [CustomEditor( typeof( AutoResolutionCanvasScaler ) )]
    public class AutoResolutionCanvasScalerEditor : UnityEditor.Editor
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

            using( new EditorGUI.DisabledGroupScope( true ) )
            {
                var categoryIndex = serializedObject.FindProperty( "_categoryIndex" );
                categoryIndex.intValue = EditorGUILayout.Popup( categoryIndex.intValue, Enum.GetNames( ResolutionCategoryType ) );
            }

            serializedObject.ApplyModifiedProperties();

        }
    }
}

