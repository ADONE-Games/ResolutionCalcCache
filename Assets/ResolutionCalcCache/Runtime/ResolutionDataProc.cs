using System;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

namespace ADONEGames.ResolutionCalcCache
{
    /// <summary>
    /// Setting Resolution data
    /// </summary>
    /// <remarks>
    /// 解像度データの設定
    /// </remarks>
    public partial class ResolutionDataProc : IDisposable
    {
        /// <summary>
        /// Valid instance
        /// </summary>
        /// <remarks>
        /// 有効なインスタンス
        /// </remarks>
        private static ResolutionDataProc _instance;
        /// <inheritdoc cref="_instance"/>
        public static ResolutionDataProc Instance { get => _instance ??= ResolutionDataProc.CreateInstance( new ResolutionData[] { ResolutionData.DefaultPortraitInstance, ResolutionData.DefaultLandscapeInstance } ); private set => _instance = value; }

        /// <summary>
        /// Instance locator by resolution
        /// </summary>
        /// <remarks>
        /// 解像度ごとのインスタンスロケーター
        /// </remarks>
        private static readonly Dictionary<int, ResolutionDataProc> ResolutionDataProcLocator = new();


        /// <summary>
        /// RenderTexture cache
        /// </summary>
        /// <remarks>
        /// RenderTextureのキャッシュ
        /// </remarks>
        private static readonly Dictionary<int, RenderTexture> RenderTextureCache = new();

        /// <summary>
        /// Platform screen width
        /// </summary>
        /// <remarks>
        /// プラットフォームの画面幅
        /// </remarks>
        internal static float PlatformScreenWidith { get; private set; }
        /// <summary>
        /// Platform screen height
        /// </summary>
        /// <remarks>
        /// プラットフォームの画面高さ
        /// </remarks>
        internal static float PlatformScreenHeight { get; private set; }

        /// <summary>
        /// Represents the orientation of the screen.
        /// </summary>
        /// <remarks>
        /// 画面の向き
        /// </remarks>
        internal static ScreenOrientation PlatformScreenOrientation => PlatformScreenWidith > PlatformScreenHeight ? ScreenOrientation.Landscape : ScreenOrientation.Portrait;

        /// <summary>
        /// Resolution data
        /// </summary>
        /// <remarks>
        /// 解像度データ
        /// </remarks>
        private readonly Dictionary<ScreenOrientation, ResolutionData> _resolutionDatas;


        /// <summary>
        /// Initializes the platform screen size based on the current platform.
        /// </summary>
        /// <remarks>
        /// 現在のプラットフォームを基準にプラットフォームの画面サイズを初期化する
        /// </remarks>
        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.AfterAssembliesLoaded )]
        private static void InitializePlatformScreenSize()
        {
#if UNITY_EDITOR
            PlatformScreenWidith = Application.isPlaying ? Screen.width : int.Parse( UnityStats.screenRes.Split( 'x' )[0] );
            PlatformScreenHeight = Application.isPlaying ? Screen.height : int.Parse( UnityStats.screenRes.Split( 'x' )[1] );
#else
            PlatformScreenWidith = Screen.width;
            PlatformScreenHeight = Screen.height;
#endif
        }

        /// <summary>
        /// Class responsible for processing resolution data.
        /// </summary>
        /// <remarks>
        /// 解像度データを処理するクラス
        /// </remarks>
        /// <param name="resolutions">The resolution data to process.</param>
        internal ResolutionDataProc( ResolutionData[] resolutions )
        {
            _resolutionDatas ??= new Dictionary<ScreenOrientation, ResolutionData>();

            foreach( var resolution in resolutions )
            {
                SetResolutionData( resolution.ResolutionSizeDatas[0].Orientation, resolution );
            }
        }

        /// <summary>
        /// Sets the resolution data for a given screen orientation.
        /// </summary>
        /// <remarks>
        /// 指定された画面の向きに対する解像度データを設定する
        /// </remarks>
        /// <param name="orientation">The screen orientation.</param>
        /// <param name="resolutionData">The resolution data to set.</param>
        private void SetResolutionData( ScreenOrientation orientation, ResolutionData resolutionData )
        {
            if( !_resolutionDatas.ContainsKey( orientation ) )
                _resolutionDatas.Add( orientation, resolutionData );
            else
                _resolutionDatas[orientation] = resolutionData;
        }

        /// <summary>
        /// Initializes the ResolutionDataProc by performing calculations for resolution and orientation and caching the results in a static area.
        /// </summary>
        /// <remarks>
        /// 解像度と向きの計算を行い、結果をstaticな領域にキャッシュすることでResolutionDataProcを初期化する
        /// </remarks>
        private void Initialize()
        {
            // 解像度の計算や縦持ち横持ちの計算などを行う
            // staticな領域に計算結果をキャッシュもする
        }


        /// <inheritdoc/>
        public void Dispose()
        {
            _resolutionDatas.Clear();
        }

        /// <summary>
        /// Switches the instance.
        /// </summary>
        /// <remarks>
        /// Instanceの切り替え
        /// </remarks>
        /// <param name="levelIndex">The level index.</param>
        public static void SwitchResolution( int levelIndex )
        {
            if( !ResolutionDataProcLocator.TryGetValue( levelIndex, out var instance ) ) return;

            Instance = instance;
        }

        /// <inheritdoc cref="SwitchResolution(int)"/>
        /// <param name="level">The level.</param>
        public static void SwitchResolution( Enum level )
        {
            SwitchResolution( Convert.ToInt32( level ) );
        }

        /// <summary>
        /// Generates a unique ID for a RenderTexture cache.
        /// The ID is generated using a GUID and is checked against existing IDs in the RenderTextureCache dictionary to ensure uniqueness.
        /// </summary>
        /// <remarks>
        /// RenderTexture キャッシュ用の一意の ID を生成します。
        /// ID は GUID を使用して生成され、RenderTextureCache ディクショナリ内の既存の ID と照合されて一意性が確保されます。
        /// </remarks>
        /// <returns>Unique CacheID</returns>
        private static int GetRenderTextureCacheId()
        {
            var id = Guid.NewGuid().GetHashCode();
            while( RenderTextureCache.ContainsKey( id ) )
            {
                id = GetRenderTextureCacheId();
            }

            return id;
        }

        /// <summary>
        /// Gets the cached RenderTexture.
        /// </summary>
        /// <remarks>
        /// キャッシュされたRenderTextureを取得する
        /// </remarks>
        /// <param name="cacheId">The cache ID.</param>
        public static RenderTexture GetRenderTextureCache( int cacheId )
        {
            RenderTextureCache.TryGetValue( cacheId, out var renderTexture );
            return renderTexture;
        }

        /// <summary>
        /// Releases the cached RenderTexture.
        /// </summary>
        /// <remarks>
        /// キャッシュされたRenderTextureを解放する
        /// </remarks>
        /// <param name="cacheId">The cache ID.</param>
        public static void ReleaseRenderTextureCache( int cacheId )
        {
            if( !RenderTextureCache.TryGetValue( cacheId, out var renderTexture ) )
                return;

            RenderTextureCache.Remove( cacheId );
            RenderTexture.ReleaseTemporary( renderTexture );
        }


        /// <summary>
        /// Gets the RenderTexture corresponding to the category.
        /// </summary>
        /// <remarks>
        /// カテゴリーに対応するRenderTextureを取得する
        /// </remarks>
        /// <param name="categoryIndex">The category index.</param>
        /// <returns>RenderTexture</returns>
        public static RenderTexture NewRenderTexture( int categoryIndex )
        {
            Instance._resolutionDatas.TryGetValue( PlatformScreenOrientation, out var resolutionData );

            var resolutionSizeData = resolutionData.ResolutionSizeDatas[categoryIndex];

            return RenderTexture.GetTemporary( resolutionSizeData.Width, resolutionSizeData.Height );
        }

        /// <summary>
        /// Gets the RenderTexture corresponding to the category.
        /// </summary>
        /// <remarks>
        /// カテゴリーに対応するRenderTextureを取得する
        /// </remarks>
        /// <param name="categoryIndex">The category index.</param>
        /// <returns>cacheId, RenderTexture</returns>
        public static (int cacheId, RenderTexture renderTexture) GetRenderTexture( int categoryIndex )
        {
            var renderTexture = NewRenderTexture( categoryIndex );
            var cacheId = GetRenderTextureCacheId();
            RenderTextureCache.Add( cacheId, renderTexture );

            return (cacheId, renderTexture);
        }

        /// <inheritdoc cref="GetRenderTexture(int)"/>
        /// <param name="category">The category.</param>
        public static (int cacheId, RenderTexture renderTexture) GetRenderTexture( Enum category )
        {
            return GetRenderTexture( Convert.ToInt32( category ) );
        }
    }
}
