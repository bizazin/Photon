using System;
using Photon.Realtime;

namespace PunNetwork.Services.ProjectNetwork
{
    public interface IProjectNetworkService
    {
        event Action ConnectedToMasterEvent;
        void EnterRoom();
        event Action<Player> PlayerLeftRoomEvent;
    }
}