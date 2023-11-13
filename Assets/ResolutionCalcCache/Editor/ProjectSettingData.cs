using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using UnityEditor;

using UnityEditorInternal;

using UnityEngine;



namespace ADONEGames.ResolutionCalcCache.Editor
{
    [Serializable]
    internal class ResolutionProjectData
    {
        private static readonly string AssetPath = "../ProjectSettings/ResolutionCalcCache.asset";
        
        private static string _assetFilePathCache = string.Empty;
        private static string AssetFilePath => !string.IsNullOrEmpty( _assetFilePathCache ) ? _assetFilePathCache : (_assetFilePathCache = GetAssetFilePath());
        private static string GetAssetFilePath()
        {
            if( AssetPath.StartsWith( "/" ) )
                return Application.dataPath + AssetPath;
            return Application.dataPath + "/" + AssetPath;
        }

        private static string _assetFileFullPathCache = string.Empty;
        private static string AssetFileFullPath => !string.IsNullOrEmpty( _assetFileFullPathCache ) ? _assetFileFullPathCache : (_assetFileFullPathCache = Path.GetFullPath( AssetFilePath ));


        public string ResolutionDataFolderGUID;

        public List<ResolutionData> ResolutionDataList;
        public List<string> CategoryList = new() { "Base" };

        public string NamespaceName;

        public string ClassName;

        public static ResolutionProjectData CreateOrLoad()
        {
            var instance = new ResolutionProjectData();
            instance.Load();

            return instance;
        }

        public void Save()
        {
            var directoryName = Path.GetDirectoryName( AssetFileFullPath );
            if( !Directory.Exists( directoryName ) )
                Directory.CreateDirectory( directoryName );
            
            using var writer = new StreamWriter( AssetFileFullPath, false, System.Text.Encoding.UTF8 );
            writer.Write( JsonUtility.ToJson( this, true ) );
        }

        public void Load()
        {
            if( !File.Exists( AssetFileFullPath ) )
                return;

            using var reader = new StreamReader( AssetFileFullPath, System.Text.Encoding.UTF8 );
            
            JsonUtility.FromJsonOverwrite( reader.ReadToEnd(), this );
            ResolutionDataList.ForEach( data => data.SetSize() );
        }
    }

    // [assembly: Dependency(typeof(AbstractResolutionProjectData))]
    internal class ResolutionProjectDataEditor : ScriptableObject
    {
        public ResolutionProjectData ProjectData { get; set; }


        [SerializeField]
        private string _resolutionDataFolderGUID;
        [SerializeField]
        private DefaultAsset _resolutionDataFolder;
        [SerializeField]
        private string _resolutionDataFolderPath;

        public List<ResolutionData> ResolutionDataList;
        public List<string> ResolutionCategoryList;

        /// <summary>
        /// 
        /// </summary>
        private ResolutionDataEditList _resolutionDataEditList;
        private ResolutionCategoryEditList _resolutionCategoryEditList;

        [SerializeField]
        private string _namespaceName;
        [SerializeField]
        private string _className;


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

        public void OnGUI()
        {
            try
            {
                if( ProjectData == null ) Load();

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

                // data list
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
                        // GUILayout.FlexibleSpace();

                        GUILayout.Button( "Preview Image", GUILayout.Width( 200 ), GUILayout.Height( 200 ) ); // 仮でボタンを表示

                        // toggle
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

                        // GUILayout.FlexibleSpace();
                    }

                    using( new EditorGUILayout.VerticalScope() )
                    {
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
                                    using( new EditorGUILayout.HorizontalScope() )
                                    {
                                        GUILayout.FlexibleSpace();
                                        _resolutionCategoryEditList.ActiveIndex = EditorGUILayout.Popup( _resolutionCategoryEditList.ActiveIndex, ResolutionCategoryList.ToArray(), GUILayout.Width( 200 ), GUILayout.MinHeight( 18 ) );
                                    }
                                }
                            }

                            EditorGUILayout.Space( 20 );

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

        private ResolutionDataEditList GetResolutionDataEditList()
        {
            return new ResolutionDataEditList()
            {
                SeveObject = this,
                ResolutionDataList = ResolutionDataList,
                AddItem = () => new ResolutionData( ResolutionCategoryList ,1080, 1920, 0, RenderTextureFormat.ARGB32 )
            };
        }

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

        public static ResolutionProjectDataEditor CreateOrLoad()
        {
            var instance = CreateInstance<ResolutionProjectDataEditor>();
            instance.hideFlags = HideFlags.DontSave;

            instance.Load();

            return instance;
        }

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

    [Serializable]
    internal class ResolutionData
    {
        public string LevelName;

        public List<ResolutionSizeData> ResolutionSizeDataList;

        public FitDirection FitDirection;

        public ResolutionData( string categoryName, int width, int height, int depth, RenderTextureFormat format )
        {
            SetSize( categoryName, width, height, depth, format );
        }
        public ResolutionData( List<string> categoryNames, int width, int height, int depth, RenderTextureFormat format )
        {
            foreach( var categoryName in categoryNames )
            {
                SetSize( categoryName, width, height, depth, format );
            }
        }

        public ResolutionData( string levelName, FitDirection fitDirection )
        {
            LevelName = levelName;
            SetFitDirection( fitDirection );
        }
        public void SetCategoryName( int index, string categoryName )
        {
            ResolutionSizeDataList[ index ].SetCategoryName( categoryName );
        }

        public void SetSize( string categoryName, int width, int height, int depth, RenderTextureFormat format )
        {
            ResolutionSizeDataList ??= new List<ResolutionSizeData>();

            for( var i = 0; i < ResolutionSizeDataList.Count; i++ )
            {
                if( !ResolutionSizeDataList[ i ].CategoryName.Equals( categoryName ) )
                    continue;

                ResolutionSizeDataList[ i ].SetSize( width, height, depth, format );
                return;
            }

            if( !ResolutionSizeDataList.Any( data => data.CategoryName == categoryName ) )
            {
                ResolutionSizeDataList.Add( new ResolutionSizeData( categoryName, width, height, depth, format ) );
            }
        }

        public void RemoveSize( string categoryName )
        {
            for( var i = 0; i < ResolutionSizeDataList.Count; i++ )
            {
                if( !ResolutionSizeDataList[ i ].CategoryName.Equals(categoryName ) )
                    continue;

                ResolutionSizeDataList.RemoveAt( i );
            }
        }

        public void SetSize()
        {
            foreach( var data in ResolutionSizeDataList )
            {
                data.SetSize();
            }
        }

        public void SetFitDirection( FitDirection fitDirection )
        {
            FitDirection = fitDirection;
        }
    }

    [Serializable]
    public class ResolutionSizeData
    {
        public string CategoryName;

        public int Width;
        public int Height;

        [NonSerialized]
        public float Aspect;

        public int Depth;

        public RenderTextureFormat Format;

        [NonSerialized]
        public float WidthAspectProportional;
        [NonSerialized]
        public float HeightAspectProportional;

        [NonSerialized]
        public ScreenOrientation Orientation;


        public ResolutionSizeData( string categoryName )
        {
            CategoryName = categoryName;
        }

        public ResolutionSizeData( string categoryName, int width, int height, int depth, RenderTextureFormat format )
        {
            SetSize( categoryName, width, height, depth, format );
        }

        public void SetCategoryName( string categoryName )
        {
            CategoryName = categoryName;
        }
        public void SetRenderTextureFormat( RenderTextureFormat format )
        {
            Format = format;
        }

        public void SetSize( string categoryName, int width, int height, int depth, RenderTextureFormat format )
        {
            SetCategoryName( categoryName );
            SetSize( width, height, depth, format );
        }
        public void SetSize( int width, int height, int depth, RenderTextureFormat format )
        {
            var result = SizeCalculation( width, height );

            Width = result.width;
            Height = result.height;
            Depth = depth;
            SetRenderTextureFormat( format );

            WidthAspectProportional = result.widthAspectProportional;
            HeightAspectProportional = result.heightAspectProportional;

            Aspect = result.aspect;

            Orientation = result.orientation;
        }

        public void SetSize()
        {
            SetSize( Width, Height, Depth, Format );
        }

        public static ( int width, int height, float aspect, float widthAspectProportional, float heightAspectProportional, ScreenOrientation orientation ) SizeCalculation( int width, int height )
        {
            var aspect = (float)width / (float)height;
            if( float.IsNaN( aspect ) ) aspect = 0f; // 0除算対策

            var gcd = System.Numerics.BigInteger.GreatestCommonDivisor( width, height );
            var widthAspectProportional = width / (float)gcd;
            var heightAspectProportional = height / (float)gcd;

            var orientation = aspect >= 1f ? ScreenOrientation.Landscape : ScreenOrientation.Portrait;

            return ( width, height, aspect, widthAspectProportional, heightAspectProportional, orientation );
        }
    }

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
