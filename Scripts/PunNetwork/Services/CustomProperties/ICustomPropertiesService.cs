using System;
using Photon.Realtime;
using PunNetwork.NetworkData;
using Utils;

namespace PunNetwork.Services.CustomProperties
{
    public interface ICustomPropertiesService 
    {
        void Subscribe<T>(Enumerators.PlayerProperty property, Action<Player, T> handler);
        void Unsubscribe<T>(Enumerators.PlayerProperty property, Action<Player, T> handler);
    }
}