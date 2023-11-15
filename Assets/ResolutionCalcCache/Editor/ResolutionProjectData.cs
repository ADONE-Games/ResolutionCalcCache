using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;


namespace ADONEGames.ResolutionCalcCache.Editor
{
    /// <summary>
    /// Serializable class that represents the resolution project data.
    /// </summary>
    /// <remarks>
    /// シリアル化可能なクラスで、解像度プロジェクトデータを表します。
    /// </remarks>
    [Serializable]
    internal class ResolutionProjectData
    {
        /// <summary>
        /// The path to the resolution project data.
        /// </summary>
        /// <remarks>
        /// 解像度プロジェクトデータへのパス
        /// </remarks>
        private static readonly string AssetPath = "../ProjectSettings/ResolutionCalcCache.asset";

        /// <summary>
        /// The cached file path for the asset.
        /// </summary>
        /// <remarks>
        /// アセットのキャッシュされたファイルパスです。
        /// </remarks>
        private static string _assetFilePathCache = string.Empty;
        /// <inheritdoc cref="_assetFilePathCache"/>
        private static string AssetFilePath => !string.IsNullOrEmpty( _assetFilePathCache ) ? _assetFilePathCache : (_assetFilePathCache = GetAssetFilePath());
        /// <summary>
        /// Get the file path to the asset.
        /// </summary>
        /// <remarks>
        /// アセットへのファイルパスを取得します。
        /// </remarks>
        private static string GetAssetFilePath()
        {
            if( AssetPath.StartsWith( "/" ) )
                return Application.dataPath + AssetPath;
            return Application.dataPath + "/" + AssetPath;
        }

        /// <summary>
        /// The full path to the resolution project data.
        /// </summary>
        /// <remarks>
        /// 解像度プロジェクトデータへの完全なパス
        /// </remarks>
        private static string _assetFileFullPathCache = string.Empty;
        /// <inheritdoc cref="_assetFileFullPathCache"/>
        private static string AssetFileFullPath => !string.IsNullOrEmpty( _assetFileFullPathCache ) ? _assetFileFullPathCache : (_assetFileFullPathCache = Path.GetFullPath( AssetFilePath ));


        /// <summary>
        /// The folder where the resolution data is stored.
        /// </summary>
        /// <remarks>
        /// 解像度データを格納するフォルダ
        /// </remarks>
        public string ResolutionDataFolderGUID;

        /// <summary>
        /// List of resolution data.
        /// </summary>
        /// <remarks>
        /// 解像度データのリストです。
        /// </remarks>
        public List<ResolutionData> ResolutionDataList;
        /// <summary>
        /// List of resolution categories.
        /// </summary>
        /// <remarks>
        /// 解像度カテゴリのリストです。
        /// </remarks>
        public List<string> CategoryList = new() { "Base" };

        /// <summary>
        /// The namespace name of the resolution data.
        /// </summary>
        /// <remarks>
        /// 解像度データの名前空間名です。
        /// </remarks>
        public string NamespaceName;
        /// <summary>
        /// The class name of the resolution data.
        /// </summary>
        /// <remarks>
        /// 解像度データのクラス名です。
        /// </remarks>
        public string ClassName;

        /// <summary>
        /// Represents the data of a resolution project.
        /// </summary>
        /// <remarks>
        /// 解像度プロジェクトのデータの生成。
        /// </remarks>
        public static ResolutionProjectData CreateOrLoad()
        {
            var instance = new ResolutionProjectData();
            instance.Load();

            return instance;
        }

        /// <summary>
        /// Save the resolution project data.
        /// </summary>
        /// <remarks>
        /// 解像度プロジェクトデータを保存します。
        /// </remarks>
        public void Save()
        {
            var directoryName = Path.GetDirectoryName( AssetFileFullPath );
            if( !Directory.Exists( directoryName ) )
                Directory.CreateDirectory( directoryName );
            
            using var writer = new StreamWriter( AssetFileFullPath, false, System.Text.Encoding.UTF8 );
            writer.Write( JsonUtility.ToJson( this, true ) );
        }

        /// <summary>
        /// Load the resolution project data.
        /// </summary>
        /// <remarks>
        /// 解像度プロジェクトデータを読み込みます。
        /// </remarks>
        public void Load()
        {
            if( !File.Exists( AssetFileFullPath ) )
                return;

            using var reader = new StreamReader( AssetFileFullPath, System.Text.Encoding.UTF8 );
            
            JsonUtility.FromJsonOverwrite( reader.ReadToEnd(), this );
            ResolutionDataList.ForEach( data => data.SetSize() );
        }
    }
}
