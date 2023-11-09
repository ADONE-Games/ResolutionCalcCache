namespace ADONEGames.ResolutionCalcCache
{

    /// <summary>
    /// Interface for resolution data.
    /// </summary>
    /// <remarks>
    /// 解像度データ
    /// </remarks>
    public interface IResolutionData
    {
        /// <summary>
        /// An array of IResolutionData objects representing different screen resolutions.
        /// </summary>
        /// <remarks>
        /// 画面解像度を表すIResolutionDataオブジェクトの配列
        /// </remarks>
        IResolutionSizeData[] ResolutionSizeDatas { get; }

        /// <summary>
        /// Gets the fit direction of the display resolution.
        /// </summary>
        /// <remarks>
        /// ディスプレイの解像度のフィット方向
        /// </remarks>
        FitDirection FitDirection => FitDirection.Vertical;
    }

    /// <summary>
    /// Represents the display resolution size data.
    /// </summary>
    /// <remarks>
    /// ディスプレイの解像度サイズデータを表します。
    /// </remarks>
    public interface IResolutionSizeData
    {
        /// <summary>
        /// Gets the width of the display resolution.
        /// </summary>
        /// <remarks>
        /// ディスプレイの解像度の幅
        /// </remarks>
        int Width => 1080;

        /// <summary>
        /// Gets the height of the display resolution.
        /// </summary>
        /// <remarks>
        /// ディスプレイの解像度の高さ
        /// </remarks>
        int Height => 1920;

        /// <summary>
        /// Gets the aspect of the display resolution.
        /// </summary>
        /// <remarks>
        /// ディスプレイの解像度のアスペクト
        /// </remarks>
        float Aspect => 1.77777f;

        /// <summary>
        /// Gets the orientation of the display resolution.
        /// </summary>
        /// <remarks>
        /// ディスプレイの解像度の向き
        /// </remarks>
        ScreenOrientation Orientation => ScreenOrientation.Portrait;
    }
}
