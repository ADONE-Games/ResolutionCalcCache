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
        public static ResolutionDataProc Instance { get => _instance/*  ??= CreateInstance( new IResolutionData[] { new ResolutionData() { Width = 1080, Height = 1920, Aspect = 1080f / 1920f, Orientation = ScreenOrientation.Portrait }, new ResolutionData() { Width = 1920, Height = 1080, Aspect = 1920f / 1080, Orientation = ScreenOrientation.Landscape } } ) */; private set => _instance = value; }

        /// <summary>
        /// Instance locator by resolution
        /// </summary>
        /// <remarks>
        /// 解像度ごとのインスタンスロケーター
        /// </remarks>
        private static readonly Dictionary<int, ResolutionDataProc> ResolutionDataProcLocator = new();

        public static float PlatformScreenWidith { get; private set; }
        public static float PlatformScreenHeight { get; private set; }

        /// <summary>
        /// Resolution data
        /// </summary>
        /// <remarks>
        /// 解像度データ
        /// </remarks>
        private readonly Dictionary<ScreenOrientation, ResolutionData> _resolutionDatas;


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
        /// Constructor
        /// </summary>
        /// <remarks>
        /// コンストラクタ
        /// </remarks>
        internal ResolutionDataProc( ResolutionData[] resolutions )
        {
            _resolutionDatas ??= new Dictionary<ScreenOrientation, ResolutionData>();

            foreach( var resolution in resolutions )
            {
                SetResolutionData( resolution.ResolutionSizeDatas[0].Orientation, resolution );
            }
        }

        private void SetResolutionData( ScreenOrientation orientation, ResolutionData resolutionData )
        {
            if( !_resolutionDatas.ContainsKey( orientation ) )
                _resolutionDatas.Add( orientation, resolutionData );
            else
                _resolutionDatas[orientation] = resolutionData;
        }


        private void Initialize()
        {
            // 解像度の計算や縦持ち横持ちの計算などを行う
            // staticな領域に計算結果をキャッシュもする
        }

        public void Dispose()
        {
            _resolutionDatas.Clear();
        }



    }
}
