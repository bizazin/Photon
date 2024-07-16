using System;
using Photon.Pun;
using Photon.Realtime;

namespace PunNetwork.Services.ProjectNetwork
{
    public class ProjectNetworkService : MonoBehaviourPunCallbacks, IProjectNetworkService
    {
        private bool _isInRoom;

        public event Action<Player> PlayerLeftRoomEvent;
        public event Action ConnectedToMasterEvent;

        public void EnterRoom()
        {
            _isInRoom = true;
        }

        public override void OnConnectedToMaster() => ConnectedToMasterEvent?.Invoke();

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (!_isInRoom)
                return;
            PlayerLeftRoomEvent?.Invoke(otherPlayer);
        }

        public override void OnLeftRoom()
        {
            PlayerLeftRoomEvent?.Invoke(PhotonNetwork.LocalPlayer);

            _isInRoom = false;
        }
    }
}