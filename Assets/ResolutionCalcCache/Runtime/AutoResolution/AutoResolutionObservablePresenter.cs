using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace ADONEGames.ResolutionCalcCache.AutoResolution
{
    internal class AutoResolutionObservablePresenter : IResolutionLevelChangeObserver
    {
        private static AutoResolutionObservablePresenter _instance;
        public static AutoResolutionObservablePresenter Instance => _instance ??= new AutoResolutionObservablePresenter();

        private static ObjectPool<RawImage> _imagePool = null;

        private int _guid = -1;

        private readonly Dictionary<int, AutoResolutionInCameraRTexSetter> _autoResolutionRTexSetters = new();

        private AutoResolutionRTexView _autoResolutionRTexView = null;

        public AutoResolutionObservablePresenter()
        {
            ResolutionDataProc.SetObject( this );
        }

        public static void AddRTexSetter( AutoResolutionInCameraRTexSetter autoResolutionRTexSetter )
        {
            Instance._autoResolutionRTexSetters.Add( autoResolutionRTexSetter.Guid, autoResolutionRTexSetter );
            autoResolutionRTexSetter.SetRenderTexture();

            if( Instance._autoResolutionRTexView != null )
            {
                autoResolutionRTexSetter.GetViewImage( Instance._autoResolutionRTexView.transform );
            }
            Instance._autoResolutionRTexView.SortChildren();
        }

        public static void RemoveRTexSetter( AutoResolutionInCameraRTexSetter autoResolutionRTexSetter )
        {
            Instance._autoResolutionRTexSetters.Remove( autoResolutionRTexSetter.Guid );

            if( Instance.ReleaseChecker() )
                Instance.Dispose();
        }

        public static void AddRTexView( AutoResolutionRTexView autoResolutionRTexView )
        {
            Instance._autoResolutionRTexView = autoResolutionRTexView;

            foreach ( var autoResolutionRTexSetter in Instance._autoResolutionRTexSetters.Values )
            {
                autoResolutionRTexSetter.GetViewImage( autoResolutionRTexView.transform );
            }
            Instance._autoResolutionRTexView.SortChildren();
        }
        public static void RemoveRTexView( AutoResolutionRTexView autoResolutionRTexView )
        {
            if( Instance._autoResolutionRTexView != autoResolutionRTexView )
                return;

            Instance._autoResolutionRTexView = null;

            foreach ( var autoResolutionRTexSetter in Instance._autoResolutionRTexSetters.Values )
            {
                autoResolutionRTexSetter.ReleaseViewImage();
            }

            if( Instance.ReleaseChecker() )
                Instance.Dispose();
        }

        public static RawImage GetRawImage()
        {
            _imagePool ??= new ObjectPool<RawImage>( createFunc: Create, actionOnGet: OnGet, actionOnRelease: OnRelease, actionOnDestroy: OnDestroy, true, 0, 10 );

            return _imagePool.Get();

            // Local Functions
            static RawImage Create()
            {
                var viewObject = new GameObject();

                var transform = viewObject.transform;

                transform.SetParent( null );
                transform.SetLocalPositionAndRotation( Vector3.zero, Quaternion.identity );
                transform.localScale = Vector3.one;

                var viewImage = viewObject.AddComponent<RawImage>();

                return viewImage;
            }

            static void OnGet( RawImage viewImage )
            {
                viewImage.gameObject.name = $"AutoResolution_ViewImage";
                viewImage.gameObject.SetActive( true );
            }

            static void OnRelease( RawImage viewImage )
            {
                viewImage.gameObject.name = $"Relese_AutoResolution_ViewImage";

                viewImage.texture = null;
                viewImage.material = null;
                // viewImage.gameObject.transform.SetParent( null );
                viewImage.gameObject.transform.SetLocalPositionAndRotation( Vector3.zero, Quaternion.identity );
                viewImage.gameObject.SetActive( false );
            }

            static void OnDestroy( RawImage viewImage )
            {
                if( viewImage == null )
                    return;
                
                viewImage.texture = null;
                viewImage.material = null;
                if( viewImage.gameObject != null )
                    UnityEngine.Object.Destroy( viewImage.gameObject );
            }
        }

        public static void ReleaseRawImage( RawImage viewImage )
        {
            _imagePool?.Release( viewImage );
        }

        private void OnLevelChange() 
        {
            foreach ( var autoResolutionRTexSetter in _autoResolutionRTexSetters.Values )
            {
                autoResolutionRTexSetter.SetRenderTexture();
            }
        }

        private bool ReleaseChecker()
        {
            if( _autoResolutionRTexSetters.Count > 0 )
                return false;

            if( _autoResolutionRTexView != null )
                return false;

            return true;
        }

        private void Dispose()
        {
            ResolutionDataProc.RemoveObject( _guid );

            _imagePool?.Dispose();
            _imagePool = null;

            _autoResolutionRTexSetters.Clear();
            _autoResolutionRTexView = null;

            _instance = null;
        }

        // IResolutionGuid
        int IResolutionGuid.Guid => _guid == -1 ? _guid = Guid.NewGuid().GetHashCode() : _guid;

        // IResolutionLevelChangeObserver
        void IResolutionLevelChangeObserver.OnLevelChange() => OnLevelChange();

        // IDisposable
        void IDisposable.Dispose() => Dispose();
    }
}


