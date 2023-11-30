using System;

using UnityEngine;
using UnityEngine.UI;


namespace ADONEGames.ResolutionCalcCache
{
    // [RequireComponent( typeof( Camera ) )]
    // public sealed class AutoResolutionInCameraRTexSetter : MonoBehaviour, IResolutionRTexSetter, IResolutionRTexViewObjectGetter, IResolutionLevelChangeObserver
    // {
    //     [SerializeField]
    //     private int _categoryIndex = 0;

    //     private int _resolutionSetObjectCacheId = -1;
    //     private int _renderTextureCacheId = -1;

    //     private Camera _cameraCache = null;
    //     private Camera CameraCache => _cameraCache != null ? _cameraCache : ( _cameraCache = GetComponent<Camera>() );

    //     private RawImage _viewImageCache = null;

    //     private void Awake()
    //     {
    //         // _resolutionSetObjectCacheId = ResolutionDataProc.SetObject( gameObject );

    //         SetTexture( ResolutionDataProc.GetRenderTexture( _categoryIndex ) );
    //     }

    //     private void SetTexture( (int renderTextureCacheId, RenderTexture renderTexture) resolutionRTex ) => SetTexture( resolutionRTex.renderTextureCacheId, resolutionRTex.renderTexture );
    //     private void SetTexture( int renderTextureCacheId, RenderTexture renderTexture )
    //     {
    //         _renderTextureCacheId = renderTextureCacheId;
    //         CameraCache.targetTexture = renderTexture;
    //     }

    //     private void OnLevelChangeNotification()
    //     {
    //         var renderTextureCacheId = _renderTextureCacheId;

    //         SetTexture( ResolutionDataProc.GetRenderTexture( _categoryIndex ) );

    //         ResolutionDataProc.ReleaseRenderTextureCache( renderTextureCacheId );
    //     }

    //     private GameObject GetViewImage( GameObject parent )
    //     {
    //         if( _viewImageCache != null )
    //             return _viewImageCache.gameObject;

    //         var viewObject = new GameObject( "ViewImage" );

    //         viewObject.transform.SetParent( parent.transform );
    //         viewObject.transform.SetLocalPositionAndRotation( new Vector3( 0f, 0f, CameraCache.depth ), Quaternion.identity );
    //         viewObject.transform.localScale = Vector3.one;

    //         _viewImageCache = viewObject.AddComponent<RawImage>();
    //         _viewImageCache.texture = CameraCache.targetTexture;

    //         return _viewImageCache.gameObject;
    //     }

    //     private void OnDestroy() => Dispose();

    //     private void Dispose()
    //     {
    //         if( _viewImageCache != null && _viewImageCache.gameObject != null )
    //         {
    //             _viewImageCache.texture = null;
    //             Destroy( _viewImageCache.gameObject );
    //         }

    //         CameraCache.targetTexture = null;

    //         ResolutionDataProc.ReleaseRenderTextureCache( _renderTextureCacheId );

    //         ResolutionDataProc.RemoveObject( _resolutionSetObjectCacheId );

    //         _cameraCache = null;
    //         _viewImageCache = null;

    //         _renderTextureCacheId = -1;
    //         _categoryIndex = 0;
    //     }

    //     // IResolutionInCameraRTexSetter
    //     int IResolutionRTexSetter.RenderTextureCacheId => _renderTextureCacheId;

    //     int IResolutionGuid.Guid => throw new NotImplementedException();

    //     void IResolutionRTexSetter.SetTexture( int renderTextureCacheId, RenderTexture renderTexture ) => SetTexture( renderTextureCacheId, renderTexture );

    //     // IResolutionInCameraRTexViewImageGetter
    //     TResult IResolutionRTexViewObjectGetter.GetViewObject<TParent, TResult>( TParent parent ) => GetViewImage( parent as GameObject ) as TResult;

    //     // IResolutionLevelChangeNotification
    //     void IResolutionLevelChangeObserver.OnLevelChange() => OnLevelChangeNotification();

    //     // IDisposable
    //     void IDisposable.Dispose() => Dispose();
    // }

    // public interface IResolutionRTexSetter : IDisposable
    // {
    //     int RenderTextureCacheId { get; }
    //     void SetTexture( (int renderTextureCacheId, RenderTexture renderTexture) resolutionRTex ) => SetTexture( resolutionRTex.renderTextureCacheId, resolutionRTex.renderTexture );
    //     void SetTexture( int renderTextureCacheId, RenderTexture renderTexture );
    // }

    // public interface IResolutionRTexViewObjectGetter : IDisposable
    // {
    //     TResult GetViewObject<TParent,TResult>( TParent parent ) where TParent : class where TResult : class;
    // }
}

