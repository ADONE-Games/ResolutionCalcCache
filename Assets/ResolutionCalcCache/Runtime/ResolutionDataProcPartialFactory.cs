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
        /// <returns>生成されたInstance</returns>
        private static ResolutionDataProc CreateInstance( IResolutionData[] resolutionDatas )
        {
            var resolutionDataProcs = new ResolutionData[ resolutionDatas.Length ];
            for( var i = 0; i < resolutionDatas.Length; i++ )
            {
                // resolutionDataProcs[ i ] = new ResolutionData { Width = resolutionDatas[ i ].Width, Height = resolutionDatas[ i ].Height, Aspect = resolutionDatas[ i ].Aspect, Orientation = resolutionDatas[ i ].Orientation };
            }

            var instance = new ResolutionDataProc( resolutionDataProcs );
            instance.Initialize();

            return instance;
        }

        /// <summary>
        /// Switches the instance.
        /// </summary>
        /// <remarks>
        /// Instanceの切り替え
        /// </remarks>
        public static void SwitchResolution( int levelIndex )
        {
            if( !ResolutionDataProcLocator.TryGetValue( levelIndex, out var instance ) ) return;

            Instance = instance;
        }

        /// <inheritdoc cref="SwitchResolution(int)"/>
        public static void SwitchResolution( Enum levelIndex )
        {
            SwitchResolution( Convert.ToInt32( levelIndex ) );
        }

        /// <summary>
        /// Generates an instance and registers it to the locator.
        /// </summary>
        /// <remarks>
        /// Instanceの生成とLocatorへの登録
        /// </remarks>
        public static void Append( int levelIndex, IResolutionData[] resolutionDatas )
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
        public static void Append( Enum levelIndex, IResolutionData[] resolutionDatas )
        {
            Append( Convert.ToInt32( levelIndex ), resolutionDatas );
        }
    }
}
