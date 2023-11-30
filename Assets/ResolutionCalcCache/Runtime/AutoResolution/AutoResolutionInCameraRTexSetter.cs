using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace ADONEGames.ResolutionCalcCache.AutoResolution
{
    [RequireComponent( typeof( Camera ) )]
    public sealed class AutoResolutionInCameraRTexSetter : MonoBehaviour, IAutoResolutionRTexSetter
    {
        [SerializeField]
        private int _categoryIndex = 0;

        private int _guid = -1;
        public int Guid => _guid == -1 ? _guid = System.Guid.NewGuid().GetHashCode() : _guid;
        
        private int _resolutionSetObjectCacheId = -1;

        private Camera _cameraCache = null;
        private Camera CameraCache => _cameraCache != null ? _cameraCache : ( _cameraCache = GetComponent<Camera>() );

        private RawImage _viewImageCache = null;

        private void Awake()
        {
            AutoResolutionObservablePresenter.AddRTexSetter( this );
        }

        public void SetRenderTexture()
        {
            var tempRTexId = _resolutionSetObjectCacheId;

            var (cacheId, renderTexture) = ResolutionDataProc.GetRenderTexture( _categoryIndex );

            _resolutionSetObjectCacheId = cacheId;

            CameraCache.targetTexture = renderTexture;

            if( _viewImageCache != null )
            {
                _viewImageCache.texture = renderTexture;

                if( _viewImageCache.transform is RectTransform transform )
                {
                    transform.SetLocalPositionAndRotation( new Vector3( 0f, 0f, -CameraCache.depth ), Quaternion.identity );

                    ResolutionDataProc.TryGetResolutionData( out var resolutionData );
                    transform.sizeDelta = new Vector2( resolutionData.ResolutionSizeDatas[0].Width, resolutionData.ResolutionSizeDatas[0].Height );
                }
            }

            ResolutionDataProc.ReleaseRenderTextureCache( tempRTexId );
        }

        public GameObject GetViewImage( Transform parent )
        {
            _viewImageCache = AutoResolutionObservablePresenter.GetRawImage();

            _viewImageCache.texture = CameraCache.targetTexture;
            
            
            if( _viewImageCache.transform is RectTransform transform )
            {
                parent.SetLocalPositionAndRotation( new Vector3( 0f, 0f, Mathf.Max( parent.localPosition.z, CameraCache.depth ) ), Quaternion.identity );

                transform.SetParent( parent );
                transform.SetLocalPositionAndRotation( new Vector3( 0f, 0f, -CameraCache.depth ), Quaternion.identity );
                transform.localScale = Vector3.one;

                transform.gameObject.layer = parent.gameObject.layer;
                
                ResolutionDataProc.TryGetResolutionData( out var resolutionData );

                transform.anchorMin = Vector2.zero;
                transform.anchorMax = Vector2.one;
                transform.pivot = new Vector2( 0.5f, 0.5f );
                transform.sizeDelta = new Vector2( resolutionData.ResolutionSizeDatas[0].Width, resolutionData.ResolutionSizeDatas[0].Height );
            }

            return _viewImageCache.gameObject;
        }

        public void ReleaseViewImage()
        {
            if( _viewImageCache == null ) return;

            AutoResolutionObservablePresenter.ReleaseRawImage( _viewImageCache );
            _viewImageCache = null;
        }

        private void OnDestroy() => Dispose();

        private void Dispose()
        {
            AutoResolutionObservablePresenter.RemoveRTexSetter( this );

            ReleaseViewImage();

            _cameraCache.targetTexture = null;
            ResolutionDataProc.ReleaseRenderTextureCache( _resolutionSetObjectCacheId );

            _viewImageCache = null;

            _cameraCache = null;

            _resolutionSetObjectCacheId = -1;

            _guid = -1;
        }

        // IAutoResolutionRTexSetter
        // void IAutoResolutionRTexSetter.SetRenderTexture() => SetRenderTexture();
        // GameObject IAutoResolutionRTexSetter.GetViewImage( Transform parent ) => GetViewImage( parent );
        // void IAutoResolutionRTexSetter.ReleaseViewImage() => ReleaseViewImage();

        // IResolutionGuid
        // int IResolutionGuid.Guid => _guid == -1 ? _guid = Guid.NewGuid().GetHashCode() : _guid;

        // IDisposable
        void IDisposable.Dispose() => Dispose();

    }
}
