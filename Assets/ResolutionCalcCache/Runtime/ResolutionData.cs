using System;

using UnityEngine;

namespace ADONEGames.ResolutionCalcCache
{
    /// <inheritdoc cref="IResolutionData"/>
    internal struct ResolutionData : IResolutionData
    {
        // /// <inheritdoc cref="IResolutionData.Width"/>
        // public int Width { get; set; }

        // /// <inheritdoc cref="IResolutionData.Height"/>
        // public int Height { get; set; }

        // /// <inheritdoc cref="IResolutionData.Aspect"/>
        // public float Aspect { get; set; }

        // /// <inheritdoc cref="IResolutionData.Orientation"/>
        // public ScreenOrientation Orientation { get; set; }

        // public FitDirection FitDirection { get; set; }

        // public void Initialize()
        // {
        //     var platformAspect = (float)ResolutionDataProc.PlatformScreenWidith / ResolutionDataProc.PlatformScreenHeight;
        //     if (platformAspect > Aspect)
        //     {
        //         Height = Mathf.RoundToInt(Width / platformAspect);
        //     }
        //     else
        //     {
        //         Width = Mathf.RoundToInt(Height * platformAspect);
        //     }
        // }

        // /// <inheritdoc/>
        // readonly int IResolutionData.Width => Width;

        // /// <inheritdoc/>
        // readonly int IResolutionData.Height => Height;

        // /// <inheritdoc/>
        // readonly float IResolutionData.Aspect => Aspect;

        // /// <inheritdoc/>
        // readonly ScreenOrientation IResolutionData.Orientation => Orientation;

        // readonly FitDirection IResolutionData.FitDirection => FitDirection;

        public ResolutionSizeData[] ResolutionSizeDatas { get; set; }

        readonly IResolutionSizeData[] IResolutionData.ResolutionSizeDatas => Array.ConvertAll(ResolutionSizeDatas, x => (IResolutionSizeData)x);
    }

    /// <inheritdoc cref="IResolutionSizeData"/>
    internal struct ResolutionSizeData : IResolutionSizeData
    {
        /// <inheritdoc cref="IResolutionSizeData.Width"/>
        public int Width { get; set; }
        /// <inheritdoc cref="IResolutionSizeData.Height"/>
        public int Height { get; set; }
        /// <inheritdoc cref="IResolutionSizeData.Aspect"/>
        public float Aspect { get; set; }
        /// <inheritdoc cref="IResolutionSizeData.Orientation"/>
        public ScreenOrientation Orientation { get; set; }

        public void Initialize()
        {
            // 仮処理

            var platformAspect = (float)ResolutionDataProc.PlatformScreenWidith / ResolutionDataProc.PlatformScreenHeight;
            if (platformAspect > Aspect)
            {
                Height = Mathf.RoundToInt(Width / platformAspect);
            }
            else
            {
                Width = Mathf.RoundToInt(Height * platformAspect);
            }
        }

        /// <inheritdoc/>
        readonly int IResolutionSizeData.Width => Width;
        /// <inheritdoc/>
        readonly int IResolutionSizeData.Height => Height;
        /// <inheritdoc/>
        readonly float IResolutionSizeData.Aspect => Aspect;
        /// <inheritdoc/>
        readonly ScreenOrientation IResolutionSizeData.Orientation => Orientation;
    }
}
