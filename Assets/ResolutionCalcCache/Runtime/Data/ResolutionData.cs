using System;

namespace ADONEGames.ResolutionCalcCache
{
    /// <summary>
    /// Interface for resolution data.
    /// </summary>
    /// <remarks>
    /// 解像度データ
    /// </remarks>
    public readonly struct ResolutionData
    {
        /// <summary>
        /// An array of IResolutionData objects representing different screen resolutions.
        /// </summary>
        /// <remarks>
        /// 画面解像度を表すIResolutionDataオブジェクトの配列
        /// </remarks>
        public readonly ResolutionSizeData[] ResolutionSizeDatas { get; }

        /// <summary>
        /// Gets the fit direction of the display resolution.
        /// </summary>
        /// <remarks>
        /// ディスプレイの解像度のフィット方向
        /// </remarks>
        public readonly FitDirection FitDirection { get; }

        /// <summary>
        /// Represents a set of resolution size data and the fit direction to be used for resolution calculations.
        /// </summary>
        /// <remarks>
        /// 解像度計算に使用する解像度サイズデータとフィット方向のセット
        /// </remarks>
        /// <param name="resolutionSizeDatas">An array of IResolutionData objects representing different screen resolutions.</param>
        /// <param name="fitDirection">The fit direction of the display resolution.</param>
        public ResolutionData( ResolutionSizeData[] resolutionSizeDatas, FitDirection fitDirection )
        {
            ResolutionSizeDatas = resolutionSizeDatas;
            FitDirection = fitDirection;
        }

        /// <summary>
        /// Represents a resolution data that is recalculated based on the current data and the platform's resolution.
        /// </summary>
        /// <remarks>
        /// 現在のデータとプラットフォームの解像度を基準に再計算された解像度データ
        /// </remarks>
        public ResolutionData Recalculate()
        {
            var resolutionSizeDatas = new ResolutionSizeData[ResolutionSizeDatas.Length];
            for (var i = 0; i < ResolutionSizeDatas.Length; i++)
            {
                resolutionSizeDatas[i] = ResolutionSizeDatas[i].Recalculate( FitDirection );
            }

            return new ResolutionData(resolutionSizeDatas, FitDirection);
        }

        /// <summary>
        /// Keep FullHD data as default
        /// </summary>
        /// <remarks>
        /// FullHDなデータをデフォルトとして持っておく
        /// </remarks>
        public static ResolutionData DefaultPortraitInstance => new( new[] { ResolutionSizeData.DefaultPortraitInstance, }, FitDirection.Horizontal );
        /// <inheritdoc cref="DefaultPortraitInstance"/>
        public static ResolutionData DefaultLandscapeInstance => new( new[] { ResolutionSizeData.DefaultLandscapeInstance, }, FitDirection.Vertical );
    }
}
