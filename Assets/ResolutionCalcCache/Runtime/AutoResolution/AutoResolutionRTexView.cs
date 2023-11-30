using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ADONEGames.ResolutionCalcCache.AutoResolution
{
    public class AutoResolutionRTexView : MonoBehaviour, IDisposable
    {
        private void Awake()
        {
            gameObject.layer = gameObject.transform.parent.gameObject.layer;
            AutoResolutionObservablePresenter.AddRTexView( this );
        }


        private void OnDestroy() => Dispose();

        private void Dispose()
        {
            AutoResolutionObservablePresenter.RemoveRTexView( this );
        }

        void IDisposable.Dispose() => Dispose();


        public void SortChildren()
        {
            var children = new List<Transform>();
            foreach( Transform child in transform )
            {
                children.Add( child );
            }

            children.Sort( ( a, b ) => a.localPosition.z.CompareTo( b.localPosition.z ) );

            foreach( var child in children )
            {
                child.SetAsFirstSibling();
            }
        }
    }

}

