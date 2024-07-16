using System;
using Models.Data;

namespace Services.Data
{
    public interface IDataService
    {
        event Action DataLoadedEvent;
        CachedUserData CachedUserLocalData { get; }
        bool DataIsLoaded { get; }
        void StartLoading();
    }
}