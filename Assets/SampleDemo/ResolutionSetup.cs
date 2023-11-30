///////////////////////////////////////////////////////////////////////////////////////////
// This file is automatically generated.
// Modifying it is not recommended.
///////////////////////////////////////////////////////////////////////////////////////////
using ADONEGames.ResolutionCalcCache;

using UnityEngine;


using ScreenOrientation = ADONEGames.ResolutionCalcCache.ScreenOrientation;

namespace ProjectOrigin
{
    /// <summary>
    /// This static class provides a method to set up the resolution data for the game
    /// </summary>
    /// <remarks>
    /// ゲームの解像度データをセットアップするためのメソッド
    /// </remarks>
    public static class ResolutionSetup
    {
        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
        public static void Setup()
        {
            ResolutionDataProc.Append(
                (int)ResolutionLevel.Lowest,
                new[] {
                    new ResolutionData( new[] { new ResolutionSizeData( 1080, 1920, 0.56250000f, 0, RenderTextureFormat.ARGB32, ScreenOrientation.Portrait ), new ResolutionSizeData( 360, 640, 0.56250000f, 16, RenderTextureFormat.ARGB32, ScreenOrientation.Portrait ), new ResolutionSizeData( 720, 1280, 0.56250000f, 0, RenderTextureFormat.ARGB32, ScreenOrientation.Portrait ), },  FitDirection.Horizontal ),
                    new ResolutionData( new[] { new ResolutionSizeData( 1920, 1080, 1.77777800f, 0, RenderTextureFormat.ARGB32, ScreenOrientation.Landscape ), new ResolutionSizeData( 640, 360, 1.77777800f, 16, RenderTextureFormat.ARGB32, ScreenOrientation.Landscape ), new ResolutionSizeData( 1280, 720, 1.77777800f, 0, RenderTextureFormat.ARGB32, ScreenOrientation.Landscape ), },  FitDirection.Vertical ),
                } );
            ResolutionDataProc.Append(
                (int)ResolutionLevel.Normal,
                new[] {
                    new ResolutionData( new[] { new ResolutionSizeData( 1080, 1920, 0.56250000f, 0, RenderTextureFormat.ARGB32, ScreenOrientation.Portrait ), new ResolutionSizeData( 540, 960, 0.56250000f, 16, RenderTextureFormat.ARGB32, ScreenOrientation.Portrait ), new ResolutionSizeData( 900, 1600, 0.56250000f, 0, RenderTextureFormat.ARGB32, ScreenOrientation.Portrait ), },  FitDirection.Horizontal ),
                    new ResolutionData( new[] { new ResolutionSizeData( 1920, 1080, 1.77777800f, 0, RenderTextureFormat.ARGB32, ScreenOrientation.Landscape ), new ResolutionSizeData( 960, 540, 1.77777800f, 16, RenderTextureFormat.ARGB32, ScreenOrientation.Landscape ), new ResolutionSizeData( 1600, 900, 1.77777800f, 0, RenderTextureFormat.ARGB32, ScreenOrientation.Landscape ), },  FitDirection.Vertical ),
                } );
            ResolutionDataProc.Append(
                (int)ResolutionLevel.High,
                new[] {
                    new ResolutionData( new[] { new ResolutionSizeData( 1080, 1920, 0.56250000f, 0, RenderTextureFormat.ARGB32, ScreenOrientation.Portrait ), new ResolutionSizeData( 720, 1280, 0.56250000f, 16, RenderTextureFormat.ARGB32, ScreenOrientation.Portrait ), new ResolutionSizeData( 1080, 1920, 0.56250000f, 0, RenderTextureFormat.ARGB32, ScreenOrientation.Portrait ), },  FitDirection.Horizontal ),
                    new ResolutionData( new[] { new ResolutionSizeData( 1920, 1080, 1.77777800f, 0, RenderTextureFormat.ARGB32, ScreenOrientation.Landscape ), new ResolutionSizeData( 1280, 720, 1.77777800f, 16, RenderTextureFormat.ARGB32, ScreenOrientation.Landscape ), new ResolutionSizeData( 1920, 1080, 1.77777800f, 0, RenderTextureFormat.ARGB32, ScreenOrientation.Landscape ), },  FitDirection.Vertical ),
                } );
            ResolutionDataProc.Append(
                (int)ResolutionLevel.Highest,
                new[] {
                    new ResolutionData( new[] { new ResolutionSizeData( 1080, 1920, 0.56250000f, 0, RenderTextureFormat.ARGB32, ScreenOrientation.Portrait ), new ResolutionSizeData( 1080, 1920, 0.56250000f, 16, RenderTextureFormat.ARGB32, ScreenOrientation.Portrait ), new ResolutionSizeData( 1080, 1920, 0.56250000f, 0, RenderTextureFormat.ARGB32, ScreenOrientation.Portrait ), },  FitDirection.Horizontal ),
                    new ResolutionData( new[] { new ResolutionSizeData( 1920, 1080, 1.77777800f, 0, RenderTextureFormat.ARGB32, ScreenOrientation.Landscape ), new ResolutionSizeData( 1920, 1080, 1.77777800f, 16, RenderTextureFormat.ARGB32, ScreenOrientation.Landscape ), new ResolutionSizeData( 1920, 1080, 1.77777800f, 0, RenderTextureFormat.ARGB32, ScreenOrientation.Landscape ), },  FitDirection.Vertical ),
                } );
        }
    }
}