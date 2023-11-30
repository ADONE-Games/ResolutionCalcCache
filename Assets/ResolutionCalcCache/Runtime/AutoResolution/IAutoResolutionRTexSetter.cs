using System;

using UnityEngine;

namespace ADONEGames.ResolutionCalcCache.AutoResolution
{
    internal interface IAutoResolutionRTexSetter : IResolutionGuid, IDisposable
    {
        void SetRenderTexture();

        GameObject GetViewImage( Transform parent );
        void ReleaseViewImage();
    }
}
