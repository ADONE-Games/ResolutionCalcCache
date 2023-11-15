using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;

using UnityEditor;

using UnityEditorInternal;

using UnityEngine;


namespace ADONEGames.ResolutionCalcCache.Editor
{
    /// <summary>
    /// A class that manages a list of <see cref="ResolutionData"/> objects for editing in the Unity Editor.
    /// </summary>
    /// <remarks>
    /// このクラスは、Unityエディタで編集するための<see cref="ResolutionData"/>オブジェクトのリストを管理するクラスです。
    /// </remarks>
    internal class ResolutionDataEditList
    {
        private ReorderableList _reorderableList;
        private Vector2 _reorderableListScrollPosition;

        private GUIContent ErrorIconContent { get; set; } = null;
        private GUIContent GreenIconContent { set; get; } = null;

        public int ActiveIndex { get => _reorderableList == null ? -1 : _reorderableList.index; set { if( _reorderableList != null ) _reorderableList.index = value; } }
        public int Count => ResolutionDataList == null ? 0 : ResolutionDataList.Count;

        public ResolutionData this[int index] => ResolutionDataList.Count != 0 && index < ResolutionDataList.Count ? (ResolutionData)ResolutionDataList[index] : null;

        public UnityEngine.Object SeveObject { get; set; }
        public IList ResolutionDataList { get; set; }
        public Func<ResolutionData> AddItem { get; set; }

        private void ReorderableListInit()
        {
            ErrorIconContent ??= EditorGUIUtility.IconContent( "TestFailed" );
            GreenIconContent ??= EditorGUIUtility.IconContent( "TestPassed" );

            // リストの数が変わったら再生成
            if( _reorderableList == null || _reorderableList.count != ResolutionDataList.Count )
                _reorderableList = null;

            _reorderableList ??= new ReorderableList( ResolutionDataList, typeof( ResolutionData ), draggable: true, displayHeader: false, displayAddButton: false, displayRemoveButton: false ) {
                drawElementCallback = ( rect, index, isActive, isFocused ) => {
                    var iconRect = new Rect( rect.x, rect.y, ErrorIconContent.image.width, rect.height );
                    var labelRect = new Rect( rect.x + iconRect.width, rect.y, rect.width - iconRect.width - 30, rect.height );
                    var indexRect = new Rect( rect.x + iconRect.width + labelRect.width, rect.y, 30, rect.height );

                    using( var check = new EditorGUI.ChangeCheckScope() )
                    {
                        var levelName = EditorGUI.TextField( labelRect, GetItem( index ).LevelName );

                        using( new EditorGUI.DisabledScope( true ) )
                        {
                            EditorGUI.IntField( indexRect, index );
                        }

                        if( check.changed )
                        {

                            // 英字以外の文字が含まれている場合は、入力を無効にする
                            if( !string.IsNullOrEmpty( levelName ) && !Regex.IsMatch( levelName, "^[a-zA-Z]+$" ) )
                            {
                                levelName = GetItem( index ).LevelName;
                            }
                            else
                            {
                                Undo.RecordObject( SeveObject, "SetResolutionData" );

                                GetItem( index ).LevelName = levelName;
                                // 先頭が英字の場合は大文字にする
                                if( !string.IsNullOrEmpty( GetItem( index ).LevelName ) && new Regex( "^[a-zA-Z]+$" ).IsMatch( GetItem( index ).LevelName ) )
                                {
                                    var chars = GetItem( index ).LevelName.ToCharArray();
                                    chars[0] = char.ToUpper( chars[0] );
                                    GetItem( index ).LevelName = new string( chars );
                                }

                                EditorUtility.SetDirty( SeveObject );
                            }
                        }
                    }


                    var checkIcon = new GUIContent( string.Empty, IsLevelNameNullOrEmpty( index ) ? ErrorIconContent.image : null );
                    EditorGUI.LabelField( iconRect, checkIcon );

                    return;

                    bool IsLevelNameNullOrEmpty( int index )
                    {
                        if( index < 0 || index >= ResolutionDataList.Count ) return true;

                        if( string.IsNullOrEmpty( GetItem( index ).LevelName ) ) return true;

                        for( var i = 0; i < ResolutionDataList.Count; i++ )
                        {
                            if( i == index ) continue; // Skip checking the same index
                            if( this[i].LevelName == GetItem( index ).LevelName ) return true;
                        }

                        return false;
                    }
                },
            };
        }

        public void OnGUI()
        {
            ReorderableListInit();
            using( new EditorGUILayout.VerticalScope() )
            {
                using( new EditorGUILayout.HorizontalScope( "helpBox" ) )
                {
                    var title = "ResolutionLevel";
                    GUIContent titleContent;
                    if( IsNullOrAnyEmpty() )
                    {
                        titleContent = new GUIContent( title, ErrorIconContent.image, "ResolutionLevel is empty" );
                    }
                    else
                    {
                        titleContent = new GUIContent( title, GreenIconContent.image );
                    }
                    EditorGUILayout.LabelField( titleContent );

                    GUILayout.FlexibleSpace();

                    if( GUILayout.Button( "+", GUILayout.Width( 50 ) ) )
                    {
                        Undo.RecordObject( SeveObject, "AddResolutionData" );

                        ResolutionDataList.Add( AddItem() );

                        EditorUtility.SetDirty( SeveObject );
                    }
                    if( GUILayout.Button( "-", GUILayout.Width( 50 ) ) )
                    {
                        Undo.RecordObject( SeveObject, "RemoveResolutionData" );

                        ResolutionDataList.RemoveAt( ActiveIndex );
                        ActiveIndex = ActiveIndex >= ResolutionDataList.Count ? ResolutionDataList.Count - 1 : ActiveIndex;
                        EditorGUIUtility.keyboardControl = 0;
                        EditorGUIUtility.editingTextField = false;

                        EditorUtility.SetDirty( SeveObject );
                    }
                }

                using var scroll = new EditorGUILayout.ScrollViewScope( _reorderableListScrollPosition, GUILayout.Height( 100 ) );
                _reorderableListScrollPosition = scroll.scrollPosition;

                _reorderableList.DoLayoutList();
            }
        }

        private ResolutionData GetItem( int index ) => this[index];

        public bool IsNullOrAnyEmpty()
        {
            if( ResolutionDataList == null || ResolutionDataList.Count == 0 ) return true;

            foreach( var data in ResolutionDataList.Cast<ResolutionData>() )
            {
                if( string.IsNullOrEmpty( data.LevelName ) ) return true;
            }

            return false;
        }
    }
}
