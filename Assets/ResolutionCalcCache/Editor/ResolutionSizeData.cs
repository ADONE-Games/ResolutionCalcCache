using System;

using UnityEngine;


namespace ADONEGames.ResolutionCalcCache.Editor
{
    /// <summary>
    /// Represents a resolution size data object that contains information about the width, height, aspect ratio, and other properties of a resolution size.
    /// </summary>
    /// <remarks>
    /// 解像度の幅、高さ、アスペクト比、その他のプロパティに関する情報を含む解像度サイズデータオブジェクトを表します。
    /// </remarks>
    [Serializable]
    public class ResolutionSizeData
    {
        /// <summary>
        /// The name of the category that this resolution size belongs to.
        /// </summary>
        /// <remarks>
        /// この解像度サイズが属するカテゴリの名前です。
        /// </remarks>
        public string CategoryName;

        /// <summary>
        /// The width of the resolution size data.
        /// </summary>
        /// <remarks>
        /// 解像度サイズデータの幅
        /// </remarks>
        public int Width;
        /// <summary>
        /// Represents the height of a resolution size data.
        /// </summary>
        /// <remarks>
        /// 解像度サイズデータの高さ
        /// </remarks>
        public int Height;

        /// <summary>
        /// The aspect ratio of the resolution size data.
        /// </summary>
        /// <remarks>
        /// 解像度サイズデータのアスペクト比
        /// </remarks>
        [NonSerialized]
        public float Aspect;

        /// <summary>
        /// The depth of the resolution size.
        /// </summary>
        /// <remarks>
        /// 解像度サイズの深度。
        /// </remarks>
        public int Depth;

        /// <summary>
        /// The format of the render texture.
        /// </summary>
        /// <remarks>
        /// レンダーテクスチャのフォーマットです。
        /// </remarks>
        public RenderTextureFormat Format;

        /// <summary>
        /// Width aspect proportional value.
        /// </summary>
        /// <remarks>
        /// 幅のアスペクト比の値です。
        /// </remarks>
        [NonSerialized]
        public float WidthAspectProportional;
        /// <summary>
        /// Height aspect proportional value.
        /// </summary>
        /// <remarks>
        /// 高さのアスペクト比の値。
        /// </remarks>
        [NonSerialized]
        public float HeightAspectProportional;

        /// <summary>
        /// The screen orientation of the resolution size data.
        /// </summary>
        /// <remarks>
        /// 画面の向きを表す列挙型です。
        /// </remarks>
        [NonSerialized]
        public ScreenOrientation Orientation;


        /// <summary>
        /// Represents a resolution size data object.
        /// </summary>
        /// <remarks>
        /// 解像度サイズのデータオブジェクト
        /// </remarks>
        /// <param name="categoryName">The name of the category that this resolution size belongs to.</param>
        public ResolutionSizeData( string categoryName )
        {
            CategoryName = categoryName;
        }

        /// <inheritdoc cref="ResolutionSizeData(string)"/>
        /// <param name="width">The width of the resolution size data.</param>
        /// <param name="height">The height of the resolution size data.</param>
        /// <param name="depth">The depth of the resolution size.</param>
        /// <param name="format">The format of the render texture.</param>
        public ResolutionSizeData( string categoryName, int width, int height, int depth, RenderTextureFormat format )
        {
            SetSize( categoryName, width, height, depth, format );
        }

        /// <summary>
        /// Sets the name of the category.
        /// </summary>
        /// <remarks>
        /// カテゴリー名の設定
        /// </remarks>
        /// <param name="categoryName">The name of the category.</param>
        public void SetCategoryName( string categoryName )
        {
            CategoryName = categoryName;
        }

        /// <summary>
        /// Sets the format of the render texture.
        /// </summary>
        /// <remarks>
        /// レンダーテクスチャのフォーマットの設定
        /// </remarks>
        /// <param name="format">The format of the render texture.</param>
        public void SetRenderTextureFormat( RenderTextureFormat format )
        {
            Format = format;
        }

        /// <summary>
        /// Set the size of the render texture.
        /// </summary>
        /// <remarks>
        /// レンダーテクスチャのサイズを設定します。
        /// </remarks>
        /// <param name="categoryName">The name of the category.</param>
        /// <param name="width">The width of the render texture.</param>
        /// <param name="height">The height of the render texture.</param>
        /// <param name="depth">The depth of the render texture.</param>
        /// <param name="format">The format of the render texture.</param>
        public void SetSize( string categoryName, int width, int height, int depth, RenderTextureFormat format )
        {
            SetCategoryName( categoryName );
            SetSize( width, height, depth, format );
        }
        /// <summary>
        /// Sets the size of the render texture based on the given width, height, depth, and format.
        /// </summary>
        /// <remarks>
        /// レンダーテクスチャのサイズを、指定された幅、高さ、深度、およびフォーマットに基づいて設定します。
        /// </remarks>
        /// <param name="width">The width of the render texture.</param>
        /// <param name="height">The height of the render texture.</param>
        /// <param name="depth">The depth of the render texture.</param>
        /// <param name="format">The format of the render texture.</param>
        public void SetSize( int width, int height, int depth, RenderTextureFormat format )
        {
            var result = SizeCalculation( width, height );

            Width = result.width;
            Height = result.height;
            Depth = depth;
            SetRenderTextureFormat( format );

            WidthAspectProportional = result.widthAspectProportional;
            HeightAspectProportional = result.heightAspectProportional;

            Aspect = result.aspect;

            Orientation = result.orientation;
        }

        /// <summary>
        /// Set the size of the resolution.
        /// </summary>
        /// <remarks>
        /// 解像度のサイズを設定します。
        /// </remarks>
        public void SetSize()
        {
            SetSize( Width, Height, Depth, Format );
        }

        /// <summary>
        /// Calculates the size of the screen based on the provided width and height values.
        /// </summary>
        /// <remarks>
        /// 画面の幅と高さの値に基づいて画面のサイズを計算します。
        /// </remarks>
        /// <param name="width">The width of the screen.</param>
        /// <param name="height">The height of the screen.</param>
        /// <returns>A tuple containing the calculated width, height, aspect ratio, width aspect proportional, height aspect proportional, and screen orientation.</returns>
        public static ( int width, int height, float aspect, float widthAspectProportional, float heightAspectProportional, ScreenOrientation orientation ) SizeCalculation( int width, int height )
        {
            var aspect = (float)width / (float)height;
            if( float.IsNaN( aspect ) ) aspect = 0f; // 0除算対策

            var gcd = System.Numerics.BigInteger.GreatestCommonDivisor( width, height );
            var widthAspectProportional = width / (float)gcd;
            var heightAspectProportional = height / (float)gcd;

            var orientation = aspect >= 1f ? ScreenOrientation.Landscape : ScreenOrientation.Portrait;

            return ( width, height, aspect, widthAspectProportional, heightAspectProportional, orientation );
        }
    }
}
