using System.Collections.Generic;

using UnityEditor;

using UnityEngine;


namespace ADONEGames.ResolutionCalcCache.Editor
{
    /// <summary>
    /// エディターで使用する解像度データの管理クラス
    /// </summary>
    /// <remarks>
    /// 解像度データのフォルダを設定し、解像度データを編集するためのUIを提供する。
    /// </remarks>
    internal class ResolutionProjectDataEditor : ScriptableObject
    {
        /// <summary>
        /// 解像度プロジェクトデータ
        /// </summary>
        /// <remarks>
        /// 解像度プロジェクトデータを保持する。
        /// </remarks>
        public ResolutionProjectData ProjectData { get; set; }


        
        [SerializeField]
        private string _resolutionDataFolderGUID; // 編集用のデータ（Undo対応）
        [SerializeField]
        private DefaultAsset _resolutionDataFolder; // 編集用のデータ（Undo対応）
        [SerializeField]
        private string _resolutionDataFolderPath; // 編集用のデータ（Undo対応）

        public List<ResolutionData> ResolutionDataList; // 編集用のデータ（Undo対応）
        public List<string> ResolutionCategoryList; // 編集用のデータ（Undo対応）

        private ResolutionDataEditList _resolutionDataEditList; // 編集用のデータ（Undo対応）
        private ResolutionCategoryEditList _resolutionCategoryEditList; // 編集用のデータ（Undo対応）

        [SerializeField]
        private string _namespaceName; // 編集用のデータ（Undo対応）
        [SerializeField]
        private string _className; // 編集用のデータ（Undo対応）


        // プレビュー用画像のインデックス
        [SerializeField]
        private int _aspectPreviewIndex = 0;
        private readonly string[] _aspectPreviewImageNames = new string[] { "3:4", "9:16", "9:21", };

        /// <summary>
        /// Folder Asset
        /// </summary>
        public DefaultAsset ResolutionDataFolder { get => _resolutionDataFolder; set => SetResolutionDataFolder( value ); }

        /// <summary>
        /// Folder Path
        /// </summary>
        public string ResolutionDataFolderPath => _resolutionDataFolderPath;

        /// <summary>
        /// コード生成時の名前空間
        /// </summary>
        public string NamespaceName { get => _namespaceName; set => _namespaceName = value; }

        /// <summary>
        /// コード生成時のクラス名
        /// </summary>
        public string ClassName { get => _className; set => _className = value; }

        /// <summary>
        /// フォルダの設定
        /// </summary>
        /// <param name="folder">Folder Asset</param>
        private void SetResolutionDataFolder( DefaultAsset folder )
        {
            if( folder == null )
            {
                SetResolutionDataFolderAtPath( "Assets" );
                return;
            }

            SetResolutionDataFolderAtPath( AssetDatabase.GetAssetPath( folder ), folder );
        }

        /// <summary>
        /// フォルダの設定
        /// </summary>
        /// <param name="folder">Folder Asset</param>
        public void SetDefaultDirectory()
        {
            if( ResolutionDataFolder != null ) return;

            SetResolutionDataFolderAtPath( "Assets" );
        }

        /// <summary>
        /// フォルダを設定する
        /// </summary>
        /// <param name="path"></param>
        public void SetResolutionDataFolderAtPath( string path, DefaultAsset folder = null )
        {
            // ResolutionDataFolder = System.IO.Directory.Exists( path ) ? AssetDatabase.LoadAssetAtPath<DefaultAsset>( path ) : null;
            if( string.IsNullOrEmpty( path ) || !System.IO.Directory.Exists( path ) )
            {
                SetResolutionDataFolderAtPath( "Assets" );
                return;
            }
            _resolutionDataFolderGUID = AssetDatabase.AssetPathToGUID( path );
            _resolutionDataFolderPath = path;
            _resolutionDataFolder = folder != null ? folder : AssetDatabase.LoadAssetAtPath<DefaultAsset>( path );
        }

        /// <summary>
        /// Displays the editor GUI for the Resolution Project Data.
        /// </summary>
        /// <remarks>
        /// 解像度プロジェクトデータのエディタGUIの表示
        /// </remarks>
        public void OnGUI()
        {
            try
            {
                // データのロード
                if( ProjectData == null ) Load();

                // データの初期化
                _resolutionDataEditList ??= GetResolutionDataEditList();
                _resolutionCategoryEditList ??= GetResolutionCategoryEditList();

                // folder setting
                using( var check = new EditorGUI.ChangeCheckScope() )
                using( new EditorGUILayout.VerticalScope( "box" ) )
                {
                    EditorGUILayout.Space();

                    // foldar
                    var folder = EditorGUILayout.ObjectField( "ResolutionDataFolder", ResolutionDataFolder, typeof( DefaultAsset ), false ) as DefaultAsset;
                    if( check.changed )
                    {
                        Undo.RecordObject( this, "SetResolutionDataFolder" );

                        ResolutionDataFolder = folder;
                        EditorUtility.SetDirty( this );
                    }

                    // forder path
                    using( new EditorGUI.DisabledScope( true ) )
                        EditorGUILayout.TextField( ResolutionDataFolderPath );

                    if( ResolutionDataFolder == null )
                    {
                        EditorGUILayout.HelpBox( "ResolutionDataFolder is null", MessageType.Error );
                        return;
                    }
                }

                // level list / category list
                using( var check = new EditorGUI.ChangeCheckScope() )
                using( new EditorGUILayout.VerticalScope( "box" ) )
                {
                    using( new EditorGUILayout.HorizontalScope() )
                    {
                        _resolutionDataEditList.OnGUI();
                        EditorGUILayout.Space(20);
                        _resolutionCategoryEditList.OnGUI();
                    }
                    if( check.changed )
                    {
                        EditorUtility.SetDirty( this );
                    }

                    EditorGUILayout.Space();

                    if( _resolutionDataEditList.IsNullOrAnyEmpty())
                    {
                        EditorGUILayout.HelpBox( "ResolutionLevel is empty", MessageType.Error );
                        return;
                    }
                }

                // code setting
                using( new EditorGUILayout.HorizontalScope( "box" ) )
                {
                    // preview
                    using( new EditorGUILayout.VerticalScope("box") )
                    {
                        EditorGUILayout.Space(100);

                        GUILayout.Button( "Preview Image", GUILayout.Width( 200 ), GUILayout.Height( 200 ) ); // 仮でボタンを表示

                        // preview toggle
                        using( new EditorGUILayout.HorizontalScope() )
                        {
                            for( var i = 0; i < _aspectPreviewImageNames.Length; i++ )
                            {
                                var onToggle = GUILayout.Toggle( _aspectPreviewIndex == i, _aspectPreviewImageNames[i], "Radio", GUILayout.MinWidth( 50 ) );
                                if (onToggle != (_aspectPreviewIndex == i))
                                {
                                    _aspectPreviewIndex = i;
                                    EditorGUIUtility.keyboardControl = 0;
                                    EditorGUIUtility.editingTextField = false;
                                }
                            }
                        }
                    }

                    // code
                    using( new EditorGUILayout.VerticalScope() )
                    {
                        // namespace / class
                        using( new EditorGUILayout.HorizontalScope() )
                        {
                            EditorGUILayout.Space();

                            using( new EditorGUILayout.VerticalScope() )
                            {
                                using var check = new EditorGUI.ChangeCheckScope();
                                var namespaceName = EditorGUILayout.TextField( "Namespace", NamespaceName );
                                var className     = EditorGUILayout.TextField( "Class",     ClassName );

                                if( check.changed )
                                {
                                    Undo.RecordObject( this, "SetResolutionData" );

                                    NamespaceName = namespaceName;
                                    ClassName     = className;

                                    EditorUtility.SetDirty( this );
                                }
                            }
                        }

                        EditorGUILayout.Space();

                        
                        using( new EditorGUILayout.VerticalScope( "helpBox" ) )
                        {
                            using( new EditorGUILayout.HorizontalScope( "helpBox" ) )
                            {
                                using( new EditorGUILayout.VerticalScope() )
                                {
                                    // level select
                                    using( new EditorGUILayout.HorizontalScope() )
                                    {
                                        GUILayout.FlexibleSpace();
                                        for( var i = 0; i < _resolutionDataEditList.Count; i++ )
                                        {
                                            var onToggle = GUILayout.Toggle( _resolutionDataEditList.ActiveIndex == i, ResolutionDataList[i].LevelName, EditorStyles.miniButton, GUILayout.MinWidth( 100 ), GUILayout.MinHeight( 18 ) );
                                            if( onToggle != (_resolutionDataEditList.ActiveIndex == i) )
                                            {
                                                _resolutionDataEditList.ActiveIndex = i;
                                                EditorGUIUtility.keyboardControl = 0;
                                                EditorGUIUtility.editingTextField = false;
                                            }
                                        }
                                    }
                                    // category select
                                    using( new EditorGUILayout.HorizontalScope() )
                                    {
                                        GUILayout.FlexibleSpace();
                                        _resolutionCategoryEditList.ActiveIndex = EditorGUILayout.Popup( _resolutionCategoryEditList.ActiveIndex, ResolutionCategoryList.ToArray(), GUILayout.Width( 200 ), GUILayout.MinHeight( 18 ) );
                                    }
                                }
                            }

                            EditorGUILayout.Space( 20 );

                            // resolution setting
                            using( var check = new EditorGUI.ChangeCheckScope() )
                            using( new EditorGUILayout.VerticalScope() )
                            {
                                var activeData = ResolutionDataList[_resolutionDataEditList.ActiveIndex];
                                var activeCategory = ResolutionCategoryList[_resolutionCategoryEditList.ActiveIndex];
                                var (width, height, aspect, depth, format, orientation, fitDirection) = ViewGUILayout( activeData.ResolutionSizeDataList[_resolutionCategoryEditList.ActiveIndex].Width, activeData.ResolutionSizeDataList[_resolutionCategoryEditList.ActiveIndex].Height, activeData.ResolutionSizeDataList[_resolutionCategoryEditList.ActiveIndex].Depth, activeData.ResolutionSizeDataList[_resolutionCategoryEditList.ActiveIndex].Format, activeData.FitDirection );

                                if( check.changed )
                                {
                                    Undo.RecordObject( this, "SetResolutionData" );

                                    activeData.SetSize( activeCategory, width, height, depth, format );
                                    activeData.SetFitDirection( fitDirection );
                                    EditorUtility.SetDirty( this );
                                }

                                EditorGUILayout.Space( 10 );
                                using( new EditorGUI.DisabledGroupScope( true ) )
                                using( new EditorGUILayout.VerticalScope( "helpBox" ) )
                                {
                                    EditorGUILayout.Space( 30 );
                                    ViewGUILayout( activeData.ResolutionSizeDataList[_resolutionCategoryEditList.ActiveIndex].Height, activeData.ResolutionSizeDataList[_resolutionCategoryEditList.ActiveIndex].Width, activeData.ResolutionSizeDataList[_resolutionCategoryEditList.ActiveIndex].Depth, activeData.ResolutionSizeDataList[_resolutionCategoryEditList.ActiveIndex].Format, activeData.FitDirection == FitDirection.Horizontal ? FitDirection.Vertical : FitDirection.Horizontal );
                                    EditorGUILayout.Space( 10 );
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                Save();
            }
        }


        /// <summary>
        /// Provides a GUI layout for displaying and editing resolution data.
        /// </summary>
        /// <remarks>
        /// 解像度データを表示および編集するためのGUIレイアウトを提供します。
        /// </remarks>
        /// <param name="width">The width of the resolution.</param>
        /// <param name="height">The height of the resolution.</param>
        /// <param name="depth">The depth of the resolution.</param>
        /// <param name="format">The format of the resolution.</param>
        /// <param name="fitDirection">The fit direction of the resolution.</param>
        /// <returns>A tuple containing the width, height, aspect ratio, depth, format, orientation, and fit direction of the resolution.</returns>
        private ( int width, int height, float aspect, int depth, RenderTextureFormat format, ScreenOrientation orientation, FitDirection fitDirection ) ViewGUILayout( int width, int height, int depth, RenderTextureFormat format, FitDirection fitDirection )
        {
            var result = Editor.ResolutionSizeData.SizeCalculation( width, height );

            using( new EditorGUI.DisabledGroupScope( true ) )
            using( new EditorGUILayout.HorizontalScope() )
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.EnumPopup( "Orientation", result.orientation, GUILayout.Width( 300 ) );
            }

            using( new EditorGUILayout.HorizontalScope() )
            {
                GUILayout.FlexibleSpace();
                fitDirection = (FitDirection)EditorGUILayout.EnumPopup( "FitDirection", fitDirection, GUILayout.Width( 300 ) );
            }

            using( new EditorGUILayout.HorizontalScope() )
            {
                GUILayout.FlexibleSpace();
                result.width = EditorGUILayout.IntField( "Width", result.width, GUILayout.Width( 500 ) );
            }

            using( new EditorGUILayout.HorizontalScope() )
            {
                GUILayout.FlexibleSpace();
                result.height = EditorGUILayout.IntField( "Height", result.height, GUILayout.Width( 500 ) );
            }
            using( new EditorGUILayout.HorizontalScope() )
            {
                GUILayout.FlexibleSpace();
                depth = EditorGUILayout.IntPopup( depth, new[] { "0", "16", "24" }, new[] { 0, 16, 24 }, GUILayout.Width( 50 ) );
            }
            using( new EditorGUILayout.HorizontalScope() )
            {
                GUILayout.FlexibleSpace();
                format = (RenderTextureFormat)EditorGUILayout.EnumPopup( format, GUILayout.Width( 100 ) );
            }

            using( new EditorGUI.DisabledGroupScope( true ) )
            using( new EditorGUILayout.HorizontalScope() )
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField( "Aspect:", GUILayout.Width( 50 ) );
                EditorGUILayout.LabelField( result.aspect.ToString("0.####"), GUILayout.Width( 50 ) );

                EditorGUILayout.Space();
                EditorGUILayout.LabelField( " | ", GUILayout.Width( 20 ) );
                EditorGUILayout.Space();

                EditorGUILayout.LabelField( result.widthAspectProportional.ToString(), new GUIStyle( GUI.skin.label ) { alignment = TextAnchor.MiddleRight }, GUILayout.Width( 20 ) );
                EditorGUILayout.LabelField( " : ", new GUIStyle( GUI.skin.label ) { alignment = TextAnchor.MiddleCenter }, GUILayout.Width( 20 ) );
                EditorGUILayout.LabelField( result.heightAspectProportional.ToString(), new GUIStyle( GUI.skin.label ) { alignment = TextAnchor.MiddleLeft }, GUILayout.Width( 20 ) );
            }


            return ( result.width, result.height, result.aspect, depth, format, result.orientation, fitDirection );
        }

        /// <summary>
        /// Represents a list of ResolutionData objects that can be edited in the Unity editor.
        /// </summary>
        /// <remarks>
        /// エディタで編集可能なResolutionDataオブジェクトのリストを表します。
        /// </remarks>
        private ResolutionDataEditList GetResolutionDataEditList()
        {
            return new ResolutionDataEditList()
            {
                SeveObject = this,
                ResolutionDataList = ResolutionDataList,
                AddItem = () => new ResolutionData( ResolutionCategoryList ,1080, 1920, 0, RenderTextureFormat.ARGB32 )
            };
        }

        /// <summary>
        /// Represents a list of resolution categories that can be edited in the editor.
        /// </summary>
        /// <remarks>
        /// エディタで編集可能な解像度カテゴリのリストを表します。
        /// </remarks>
        private ResolutionCategoryEditList GetResolutionCategoryEditList()
        {
            return new ResolutionCategoryEditList()
            {
                SeveObject = this,
                ResolutionCategoryList = ResolutionCategoryList,
                AddItem = categoryList => {
                    categoryList.Add( string.Empty );
                    foreach( var data in ResolutionDataList )
                    {
                        data.SetSize( string.Empty, 1080, 1020, 0, RenderTextureFormat.ARGB32 );
                    }
                },
                RemoveItem = activeIndex => {
                    var categoryName = ResolutionCategoryList[ activeIndex ];
                    ResolutionCategoryList.RemoveAt( activeIndex );
                    foreach( var data in ResolutionDataList )
                    {
                        data.RemoveSize( categoryName );
                    }
                },
                ChangeCategoryName = category => {
                    foreach( var data in ResolutionDataList )
                    {
                        data.SetCategoryName( category.index, category.name );
                    }
                }

            };
        }

        /// <summary>
        /// Creates or loads the ResolutionProjectDataEditor instance.
        /// </summary>
        /// <remarks>
        /// リゾリューションプロジェクトデータエディターのインスタンスを作成またはロードします。
        /// </remarks>
        public static ResolutionProjectDataEditor CreateOrLoad()
        {
            var instance = CreateInstance<ResolutionProjectDataEditor>();
            instance.hideFlags = HideFlags.DontSave;

            instance.Load();

            return instance;
        }
        /// <summary>
        /// Saves the changes made to the ResolutionProjectData object.
        /// </summary>
        /// <remarks>
        /// ResolutionProjectDataオブジェクトに加えられた変更を保存します。
        /// </remarks>
        public void Save()
        {
            if( !EditorUtility.IsDirty( this ) )
                return;

            ProjectData.ResolutionDataFolderGUID = _resolutionDataFolderGUID;
            ProjectData.ResolutionDataList = ResolutionDataList;
            ProjectData.CategoryList = ResolutionCategoryList;
            ProjectData.NamespaceName = _namespaceName;
            ProjectData.ClassName = _className;
            ProjectData.Save();

            EditorUtility.ClearDirty( this );
        }
        /// <summary>
        /// Loads the ResolutionProjectData and sets the necessary variables.
        /// </summary>
        /// <remarks>
        /// ResolutionProjectDataをロードし、必要な変数を設定します。
        /// </remarks>
        public void Load()
        {
            ProjectData ??= ResolutionProjectData.CreateOrLoad();
            
            _resolutionDataFolderGUID = ProjectData.ResolutionDataFolderGUID;
            ResolutionDataList = ProjectData.ResolutionDataList;
            ResolutionCategoryList = ProjectData.CategoryList;
            _namespaceName = ProjectData.NamespaceName;
            _className = ProjectData.ClassName;
            SetResolutionDataFolderAtPath( AssetDatabase.GUIDToAssetPath( _resolutionDataFolderGUID ) );
        }
    }
}
