using System;
using System.Collections;
using System.Collections.Generic;

using ADONEGames.ResolutionCalcCache;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.UI;

namespace TestSample
{
    [RequireComponent( typeof( Button ) )]
    public class ResolutionSwitch : MonoBehaviour
    {
        [SerializeField]
        private int _levelIndex = 0;

        private Button _buttonCache = null;
        private Button ButtonCache => _buttonCache != null ? _buttonCache : ( _buttonCache = GetComponent<Button>() );

        private void Awake()
        {
            ButtonCache.onClick.AddListener( () =>
            {
                ResolutionDataProc.SwitchResolution( _levelIndex );
            } );
        }
    }

#if UNITY_EDITOR
    [CustomEditor( typeof( ResolutionSwitch ) )]
    public class ResolutionSwitchEditor : Editor
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

            var levelIndex = serializedObject.FindProperty( "_levelIndex" );

            levelIndex.intValue = EditorGUILayout.Popup( levelIndex.intValue, Enum.GetNames( ResolutionLevelType ) );

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
