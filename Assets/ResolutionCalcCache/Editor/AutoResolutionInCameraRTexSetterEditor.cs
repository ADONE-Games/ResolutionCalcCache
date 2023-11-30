using System;

using UnityEditor;


namespace ADONEGames.ResolutionCalcCache
{
    // [CustomEditor( typeof( AutoResolutionInCameraRTexSetter ) )]
    // public class AutoResolutionInCameraRTexSetterEditor : UnityEditor.Editor
    // {
    //     private Type ResolutionLevelType { get; set; }
    //     private Type ResolutionCategoryType { get; set; }

    //     // private int _previewLevelIndex = 0;
    //     // private int _previewCategoryIndex = 0;

    //     private void OnEnable()
    //     {
    //         ResolutionLevelType = null;
    //         ResolutionCategoryType = null;
    //         foreach( var assembly in AppDomain.CurrentDomain.GetAssemblies() )
    //         {
    //             foreach( var type in assembly.GetTypes() )
    //             {
    //                 if( ResolutionCategoryType == null && type.Name.Equals( "ResolutionCategory" ) )
    //                     ResolutionCategoryType = type;
    //                 if( ResolutionLevelType == null && type.Name.Equals( "ResolutionLevel" ) )
    //                     ResolutionLevelType = type;

    //                 if( ResolutionCategoryType != null && ResolutionLevelType != null )
    //                     break;
    //             }
    //             if( ResolutionCategoryType != null && ResolutionLevelType != null )
    //                 break;
    //         }
    //     }

    //     public override void OnInspectorGUI()
    //     {
    //         if( ResolutionLevelType == null || ResolutionCategoryType == null )
    //         {
    //             EditorGUILayout.HelpBox( "ResolutionLevelType or ResolutionCategoryType is null", MessageType.Error );
    //             return;
    //         }

    //         serializedObject.Update();

    //         var categoryIndex = serializedObject.FindProperty( "_categoryIndex" );

    //         categoryIndex.intValue = EditorGUILayout.Popup( categoryIndex.intValue, Enum.GetNames( ResolutionCategoryType ) );

            
    //         serializedObject.ApplyModifiedProperties();

    //         // using( new EditorGUILayout.VerticalScope( "helpbox" ) )
    //         // {
    //         //     using( new EditorGUI.IndentLevelScope() )
    //         //     {
    //         //         using( new EditorGUILayout.HorizontalScope() )
    //         //         {
    //         //             GUILayout.FlexibleSpace();
    //         //             EditorGUILayout.LabelField( "Preview Data" );
    //         //         }
    //         //         _previewLevelIndex = EditorGUILayout.Popup( "Level", _previewLevelIndex, Enum.GetNames( ResolutionLevelType ) );
    //         //         _previewCategoryIndex = EditorGUILayout.Popup( "Category", _previewCategoryIndex, Enum.GetNames( ResolutionCategoryType ) );
    //         //     }
    //         // }
    //     }
    // }
}

