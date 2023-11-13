using UnityEngine;

namespace ADONEGames.ResolutionCalcCache
{
    /// <summary>
    /// Represents a resolution size data object that contains width, height, aspect ratio, and screen orientation information.
    /// </summary>
    /// <remarks>
    /// 幅、高さ、アスペクト比、および画面の向き情報を含む解像度サイズデータオブジェクト
    /// </remarks>
    public readonly struct ResolutionSizeData
    {
        /// <summary>
        /// Gets the width of the display resolution.
        /// </summary>
        /// <remarks>
        /// ディスプレイの解像度の幅
        /// </remarks>
        public readonly int Width { get; }
        /// <summary>
        /// Gets the height of the display resolution.
        /// </summary>
        /// <remarks>
        /// ディスプレイの解像度の高さ
        /// </remarks>
        public readonly int Height { get; }
        /// <summary>
        /// Gets the aspect of the display resolution.
        /// </summary>
        /// <remarks>
        /// ディスプレイの解像度のアスペクト
        /// </remarks>
        public readonly float Aspect { get; }

        /// <summary>
        /// Gets the depth of the RenderTexture.
        /// </summary>
        /// <remarks>
        /// RenderTextureの深度
        /// </remarks>
        public readonly int Depth { get; }
        /// <summary>
        /// Gets the format of the RenderTexture.
        /// </summary>
        /// <remarks>
        /// RenderTextureのフォーマットです。
        /// </remarks>
        public readonly RenderTextureFormat TextureFormat { get; }

        /// <summary>
        /// Gets the orientation of the display resolution.
        /// </summary>
        /// <remarks>
        /// ディスプレイの解像度の向き
        /// </remarks>
        public readonly ScreenOrientation Orientation { get; }

        /// <summary>
        /// Represents a resolution size data object that contains width, height, aspect ratio, and screen orientation information.
        /// </summary>
        /// <remarks>
        /// 解像度の幅、高さ、アスペクト比、および画面の向き情報を含む解像度サイズデータオブジェクト
        /// </remarks>
        /// <param name="width">The width of the display resolution.</param>
        /// <param name="height">The height of the display resolution.</param>
        /// <param name="aspect">The aspect of the display resolution.</param>
        /// <param name="orientation">The orientation of the display resolution.</param>
        public ResolutionSizeData( int width, int height, float aspect, int depth, RenderTextureFormat format, ScreenOrientation orientation )
        {
            Width = width;
            Height = height;
            Aspect = aspect;
            Depth = depth;
            TextureFormat = format;
            Orientation = orientation;
        }

        /// <summary>
        /// Represents a resolution size data that is recalculated based on the current data and the platform's resolution.
        /// </summary>
        /// <remarks>
        /// 現在のデータとプラットフォームの解像度を基準に再計算された解像度サイズデータ
        /// </remarks>
        /// <param name="fitDirection">The fit direction of the display resolution.</param>
        /// <returns>A new instance of ResolutionSizeData recalculated based on the current data and the platform's resolution.</returns>
        public ResolutionSizeData Recalculate( FitDirection fitDirection )
        {
            if( ResolutionDataProc.PlatformScreenOrientation == ScreenOrientation.Portrait )
            {
                // 縦持ち
                var platformAspect = (float)ResolutionDataProc.PlatformScreenWidith / ResolutionDataProc.PlatformScreenHeight;
                if( fitDirection == FitDirection.Horizontal )
                {
                    // 横フィット
                    return new ResolutionSizeData( Width, Mathf.RoundToInt( Width / platformAspect ), Aspect, Depth, TextureFormat, Orientation ); // 1080x2340
                }
                else
                {
                    // 縦フィット
                    return new ResolutionSizeData( Mathf.RoundToInt( Height * platformAspect ), Height, Aspect, Depth, TextureFormat, Orientation ); // 886x1920
                }
            }
            else
            {
                // 横持ち
                var platformAspect = (float)ResolutionDataProc.PlatformScreenWidith / ResolutionDataProc.PlatformScreenHeight;
                if( fitDirection == FitDirection.Horizontal )
                {
                    // 横フィット
                    return new ResolutionSizeData( Mathf.RoundToInt( Height * platformAspect ), Height, Aspect, Depth, TextureFormat, Orientation ); // 2340x1080
                }
                else
                {
                    // 縦フィット
                    return new ResolutionSizeData( Width, Mathf.RoundToInt( Width / platformAspect ), Aspect, Depth, TextureFormat, Orientation ); // 1920x886
                }
            }
        }

        /// <summary>
        /// Keep FullHD data as default
        /// </summary>
        /// <remarks>
        /// FullHDなデータをデフォルトとして持っておく
        /// </remarks>
        public static ResolutionSizeData DefaultPortraitInstance => new( 1080, 1920, 1080f / 1920f, 0, RenderTextureFormat.ARGB32, ScreenOrientation.Portrait );
        /// <inheritdoc cref="DefaultPortraitInstance"/>
        public static ResolutionSizeData DefaultLandscapeInstance => new( 1920, 1080, 1920f / 1080f, 0, RenderTextureFormat.ARGB32, ScreenOrientation.Landscape );
    }
}
