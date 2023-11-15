using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using UnityEditor;

using UnityEditorInternal;

using UnityEngine;


namespace ADONEGames.ResolutionCalcCache.Editor
{
    /// <summary>
    /// Manages a list of resolution categories for editing in the Unity Editor.
    /// </summary>
    /// <remarks>
    /// Unityエディタで編集する解像度カテゴリのリストを管理します。
    /// </remarks>
    internal class ResolutionCategoryEditList
    {
        private ReorderableList _reorderableList;
        private Vector2 _reorderableListScrollPosition;

        private GUIContent ErrorIconContent { get; set; } = null;
        private GUIContent GreenIconContent { set; get; } = null;

        public int ActiveIndex { get => _reorderableList == null ? -1 : _reorderableList.index; set { if( _reorderableList != null ) _reorderableList.index = value; } }
        public int Count => ResolutionCategoryList == null ? 0 : ResolutionCategoryList.Count;

        public string this[int index] { get => ResolutionCategoryList.Count != 0 && index < ResolutionCategoryList.Count ? ResolutionCategoryList[index] : null; set => ResolutionCategoryList[index] = value; }

        public UnityEngine.Object SeveObject { get; set; }
        public List<string> ResolutionCategoryList { get; set; }

        public Action<List<string>> AddItem { get; set; }
        public Action<int> RemoveItem { get; set; }
        public Action<(int index, string name)> ChangeCategoryName { get; set; }

        private void ReorderableListInit()
        {
            ErrorIconContent ??= EditorGUIUtility.IconContent( "TestFailed" );
            GreenIconContent ??= EditorGUIUtility.IconContent( "TestPassed" );

            // リストの数が変わったら再生成
            if( _reorderableList == null || _reorderableList.count != ResolutionCategoryList.Count )
                _reorderableList = null;

            _reorderableList ??= new ReorderableList( ResolutionCategoryList, typeof( string ), draggable: false, displayHeader: false, displayAddButton: false, displayRemoveButton: false ) {
                drawElementCallback = ( rect, index, isActive, isFocused ) => {
                    var iconRect = new Rect( rect.x, rect.y, ErrorIconContent.image.width, rect.height );
                    var labelRect = new Rect( rect.x + iconRect.width, rect.y, rect.width - iconRect.width, rect.height );

                    using( new EditorGUI.DisabledGroupScope( index == 0 && ResolutionCategoryList[ 0 ].Equals( "Base" ) ) )
                    using( var check = new EditorGUI.ChangeCheckScope() )
                    {
                        var categoryName = EditorGUI.TextField( labelRect, this[ index ] );

                        // 更新処理
                        if( check.changed )
                        {

                            // 英字以外の文字が含まれている場合は、入力を無効にする
                            if( !string.IsNullOrEmpty( categoryName ) && !Regex.IsMatch( categoryName, "^[a-zA-Z]+$" ) )
                            {
                                categoryName = this[ index ];
                            }
                            else
                            {
                                Undo.RecordObject( SeveObject, "SetResolutionData" );

                                this[index] = categoryName;
                                // 先頭が英字の場合は大文字にする
                                if( !string.IsNullOrEmpty( this[index] ) && new Regex( "^[a-zA-Z]+$" ).IsMatch( this[index] ) )
                                {
                                    var chars = this[index].ToCharArray();
                                    chars[0] = char.ToUpper( chars[0] );
                                    this[index] = new string( chars );
                                }

                                ChangeCategoryName?.Invoke( (index, categoryName) );

                                EditorUtility.SetDirty( SeveObject );
                            }
                        }
                    }


                    var checkIcon = new GUIContent( string.Empty, IsLevelNameNullOrEmpty( index ) ? ErrorIconContent.image : null );
                    EditorGUI.LabelField( iconRect, checkIcon );

                    return;

                    /// <summary>
                    /// Checks if the level name is null or empty.
                    /// </summary>
                    /// <remarks>
                    /// レベル名がnullまたは空かどうかをチェックします。
                    /// </remarks>
                    /// <param name="index">Index of the level name to check.</param>
                    /// <returns>True if the level name is null or empty.</returns>
                    bool IsLevelNameNullOrEmpty( int index )
                    {
                        if( index < 0 || index >= ResolutionCategoryList.Count ) return true;

                        if( string.IsNullOrEmpty( ResolutionCategoryList[index] ) ) return true;

                        for( var i = 0; i < ResolutionCategoryList.Count; i++ )
                        {
                            if( i == index ) continue; // Skip checking the same index
                            if( this[i] == this[index] ) return true;
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
                using( new EditorGUILayout.HorizontalScope("helpBox") )
                {
                    var title = "Category";
                    GUIContent titleContent;
                    if( IsNullOrAnyEmpty() )
                    {
                        titleContent = new GUIContent( title, ErrorIconContent.image, "Category is empty" );
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

                        // ResolutionDataCategory.Add( AddItem() );
                        AddItem?.Invoke( ResolutionCategoryList );

                        EditorUtility.SetDirty( SeveObject );
                    }

                    using( new EditorGUI.DisabledScope( ActiveIndex == 0 && ResolutionCategoryList[ 0 ].Equals( "Base" ) ) )
                    {
                        if( GUILayout.Button( "-", GUILayout.Width( 50 ) ) )
                        {
                            if( ActiveIndex == 0 ) return;

                            Undo.RecordObject( SeveObject, "RemoveResolutionData" );

                            // ResolutionDataCategory.RemoveAt( ActiveIndex );
                            RemoveItem?.Invoke( ActiveIndex );

                            ActiveIndex = ActiveIndex >= ResolutionCategoryList.Count ? ResolutionCategoryList.Count - 1 : ActiveIndex;
                            // ActiveIndex = ResolutionDataCategory.Count - 1;

                            EditorGUIUtility.keyboardControl = 0;
                            EditorGUIUtility.editingTextField = false;

                            EditorUtility.SetDirty( SeveObject );
                        }
                    }
                }

                using var scroll = new EditorGUILayout.ScrollViewScope( _reorderableListScrollPosition, GUILayout.Height( 100 ) );
                _reorderableListScrollPosition = scroll.scrollPosition;

                // var rect = GUILayoutUtility.GetRect( 100f, 150f, GUILayout.ExpandWidth( true ) );
                // _reorderableList?.DoList( rect );
                _reorderableList.DoLayoutList();
            }
        }
        public bool IsNullOrAnyEmpty()
        {
            if( ResolutionCategoryList == null || ResolutionCategoryList.Count == 0 ) return true;

            foreach( var categoryName in ResolutionCategoryList )
            {
                if( string.IsNullOrEmpty( categoryName ) ) return true;
            }

            return false;
        }
    }
}
