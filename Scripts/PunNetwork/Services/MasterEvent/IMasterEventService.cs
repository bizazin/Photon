using System;

namespace PunNetwork.Services.MasterEvent
{
    public interface IMasterEventService
    {
        void Setup();
        void OnAllDataGet();
        void OnAllPlayersSpawned();
        void OnAllPoolsPrepared();
        void RaiseEvent(byte eventCode, object eventContent = null);
        void Subscribe<T>(byte eventCode, Action<T> handler);
        void Subscribe(byte eventCode, Action<object> handler);
        void Unsubscribe<T>(byte eventCode, Action<T> handler);
        void Unsubscribe(byte eventCode, Action<object> handler);
    }
}