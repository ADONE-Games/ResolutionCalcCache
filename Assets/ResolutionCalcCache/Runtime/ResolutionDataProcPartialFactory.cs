using System;

namespace ADONEGames.ResolutionCalcCache
{
    public partial class ResolutionDataProc
    {
        /// <summary>
        /// Generates and initializes an instance.
        /// </summary>
        /// <remarks>
        /// Instanceの生成 & 初期化
        /// </remarks>
        /// <param name="resolutionDatas">Resolution data</param>
        /// <returns>ResolutionDataProc Instance</returns>
        private static ResolutionDataProc CreateInstance( ResolutionData[] resolutionDatas )
        {
            var resolutionDataProcs = new ResolutionData[ resolutionDatas.Length ];
            for( var i = 0; i < resolutionDatas.Length; i++ )
            {
                resolutionDataProcs[ i ] = resolutionDatas[ i ].Recalculate();
            }

            var instance = new ResolutionDataProc( resolutionDataProcs );
            instance.Initialize();

            return instance;
        }

        /// <summary>
        /// Generates an instance and registers it to the locator.
        /// </summary>
        /// <remarks>
        /// Instanceの生成とLocatorへの登録
        /// </remarks>
        /// <param name="levelIndex">Level index</param>
        /// <param name="resolutionDatas">Resolution data</param>
        public static void Append( int levelIndex, ResolutionData[] resolutionDatas )
        {
            if( ResolutionDataProcLocator.ContainsKey( levelIndex ) )
            {
                var temp = ResolutionDataProcLocator[ levelIndex ];

                ResolutionDataProcLocator[levelIndex] = CreateInstance( resolutionDatas );

                if( _instance == temp )
                    _instance = null;

                temp.Dispose();
            }
            else
            {
                ResolutionDataProcLocator.Add( levelIndex, CreateInstance( resolutionDatas ) );
            }

            if( _instance == null )
                SwitchResolution( levelIndex );
        }

        /// <inheritdoc cref="Append(int, IResolutionData[])"/>
        /// <param name="levelIndex">Level index</param>
        /// <param name="resolutionDatas">Resolution data</param>
        public static void Append( Enum levelIndex, ResolutionData[] resolutionDatas )
        {
            Append( Convert.ToInt32( levelIndex ), resolutionDatas );
        }
    }
}
