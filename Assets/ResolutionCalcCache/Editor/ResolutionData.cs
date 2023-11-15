using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;


namespace ADONEGames.ResolutionCalcCache.Editor
{
    /// <summary>
    /// Represents a resolution data object that contains information about the resolution size, category name, and fit direction.
    /// </summary>
    /// <remarks>
    /// 解像度のサイズ、カテゴリ名、フィット方向に関する情報を含む解像度データオブジェクトを表します。
    /// </remarks>
    [Serializable]
    internal class ResolutionData
    {
        /// <summary>
        /// The name of the level.
        /// レベルの名前
        /// </summary>
        public string LevelName;

        /// <summary>
        /// List of resolution size data.
        /// </summary>
        /// <remarks>
        /// 解像度サイズデータのリスト
        /// </remarks>
        public List<ResolutionSizeData> ResolutionSizeDataList;

        /// <summary>
        /// The direction in which the content should fit the screen.
        /// </summary>
        /// <remarks>
        /// 画面に合わせてコンテンツを合わせる方向
        /// </remarks>
        public FitDirection FitDirection;

        /// <summary>
        /// Represents a resolution data object that contains information about the width, height, depth, and format of a render texture.
        /// </summary>
        /// <remarks>
        /// レンダーテクスチャの幅、高さ、深度、およびフォーマットに関する情報を含む解像度データオブジェクト
        /// </remarks>
        /// <param name="categoryName">The name of the category.</param>
        /// <param name="width">The width of the render texture.</param>
        /// <param name="height">The height of the render texture.</param>
        /// <param name="depth">The depth of the render texture.</param>
        /// <param name="format">The format of the render texture.</param>
        public ResolutionData( string categoryName, int width, int height, int depth, RenderTextureFormat format )
        {
            SetSize( categoryName, width, height, depth, format );
        }

        /// <inheritdoc cref="ResolutionData(string, int, int, int, RenderTextureFormat)"/>
        /// <param name="categoryNames">The name of the category.</param>
        /// <param name="width">The width of the render texture.</param>
        /// <param name="height">The height of the render texture.</param>
        /// <param name="depth">The depth of the render texture.</param>
        /// <param name="format">The format of the render texture.</param>
        public ResolutionData( List<string> categoryNames, int width, int height, int depth, RenderTextureFormat format )
        {
            foreach( var categoryName in categoryNames )
            {
                SetSize( categoryName, width, height, depth, format );
            }
        }

        /// <summary>
        /// Represents a resolution data object that contains the level name and fit direction.
        /// </summary>
        /// <remarks>
        /// レベル名とフィット方向を含む解像度データオブジェクト
        /// </remarks>
        /// <param name="levelName">The name of the level.</param>
        /// <param name="fitDirection">The direction in which the content should fit the screen.</param>
        public ResolutionData( string levelName, FitDirection fitDirection )
        {
            LevelName = levelName;
            SetFitDirection( fitDirection );
        }

        /// <summary>
        /// Set the category name for a specific index in the ResolutionSizeDataList.
        /// </summary>
        /// <remarks>
        /// 特定のインデックスのResolutionSizeDataListにカテゴリ名を設定する
        /// </remarks>
        /// <param name="index">The index of the ResolutionSizeDataList to set the category name for.</param>
        /// <param name="categoryName">The new category name to set.</param>
        public void SetCategoryName( int index, string categoryName )
        {
            ResolutionSizeDataList[ index ].SetCategoryName( categoryName );
        }

        /// <summary>
        /// Sets the size of a resolution category.
        /// </summary>
        /// <remarks>
        /// 解像度カテゴリのサイズを設定
        /// </remarks>
        /// <param name="categoryName">The name of the category.</param>
        /// <param name="width">The width of the resolution.</param>
        /// <param name="height">The height of the resolution.</param>
        /// <param name="depth">The depth of the resolution.</param>
        /// <param name="format">The format of the render texture.</param>
        public void SetSize( string categoryName, int width, int height, int depth, RenderTextureFormat format )
        {
            ResolutionSizeDataList ??= new List<ResolutionSizeData>();

            for( var i = 0; i < ResolutionSizeDataList.Count; i++ )
            {
                if( !ResolutionSizeDataList[ i ].CategoryName.Equals( categoryName ) )
                    continue;

                ResolutionSizeDataList[ i ].SetSize( width, height, depth, format );
                return;
            }

            if( !ResolutionSizeDataList.Any( data => data.CategoryName == categoryName ) )
            {
                ResolutionSizeDataList.Add( new ResolutionSizeData( categoryName, width, height, depth, format ) );
            }
        }

        /// <summary>
        /// Removes the ResolutionSizeData object with the specified category name from the ResolutionSizeDataList.
        /// </summary>
        /// <remarks>
        /// 指定されたカテゴリ名を持つ ResolutionSizeData オブジェクトを ResolutionSizeDataList から削除します。
        /// </remarks>
        /// <param name="categoryName">The name of the category to remove.</param>
        public void RemoveSize( string categoryName )
        {
            for( var i = 0; i < ResolutionSizeDataList.Count; )
            {
                if( ResolutionSizeDataList[ i ].CategoryName.Equals(categoryName ) )
                {
                    ResolutionSizeDataList.RemoveAt( i );
                    continue;
                }

                i++;
            }
        }

        /// <summary>
        /// Sets the size of each resolution data in the list.
        /// </summary>
        /// <remarks>
        /// リスト内の各解像度データのサイズを設定
        /// </remarks>
        public void SetSize()
        {
            foreach( var data in ResolutionSizeDataList )
            {
                data.SetSize();
            }
        }

        /// <summary>
        /// Sets the fit direction of the resolution data.
        /// </summary>
        /// <remarks>
        /// 解像度データのフィット方向を設定。
        /// </remarks>
        /// <param name="fitDirection">The fit direction to set.</param>
        public void SetFitDirection( FitDirection fitDirection )
        {
            FitDirection = fitDirection;
        }
    }
}
