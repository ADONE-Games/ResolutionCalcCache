using System;


namespace ADONEGames.ResolutionCalcCache
{
    public interface IResolutionLevelChangeObserver : IResolutionGuid, IDisposable
    {
        void OnLevelChange();
    }
}

