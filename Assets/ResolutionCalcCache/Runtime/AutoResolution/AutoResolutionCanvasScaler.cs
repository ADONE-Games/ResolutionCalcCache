using System;

using UnityEngine;
using UnityEngine.UI;


namespace ADONEGames.ResolutionCalcCache.AutoResolution
{
    [RequireComponent( typeof( Canvas ) )]
    public class AutoResolutionCanvasScaler : MonoBehaviour, IResolutionLevelChangeObserver
    {
        [SerializeField]
        private int _categoryIndex = 0;

        private int _guid = -1;

        private CanvasScaler _canvasScalerCache = null;


        private void Awake()
        {
            ResolutionDataProc.SetObject( this );

            SetScaler();
        }

        private void SetScaler()
        {
            _canvasScalerCache = _canvasScalerCache != null ? _canvasScalerCache : GetComponent<CanvasScaler>();

            _canvasScalerCache.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            ResolutionDataProc.TryGetResolutionData( out var resolutionData );

            _canvasScalerCache.referenceResolution = new Vector2( resolutionData.ResolutionSizeDatas[_categoryIndex].Width, resolutionData.ResolutionSizeDatas[_categoryIndex].Height );
        }

        private void OnLevelChange()
        {
            SetScaler();
        }

        private void OnDestroy() => Dispose();

        private void Dispose()
        {
            ResolutionDataProc.RemoveObject( _guid );

            _canvasScalerCache = null;
        }

        // IResolutionGuid
        int IResolutionGuid.Guid => _guid == -1 ? _guid = Guid.NewGuid().GetHashCode() : _guid;

        // IResolutionLevelChangeObserver
        void IResolutionLevelChangeObserver.OnLevelChange() => OnLevelChange();

        // IDisposable
        void IDisposable.Dispose() => Dispose();
    }
}
