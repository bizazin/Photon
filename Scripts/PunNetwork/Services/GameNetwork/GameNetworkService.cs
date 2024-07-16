using System.Collections;
using System.Linq;
using Controllers;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using PunNetwork.PhotonTeams;
using PunNetwork.Services.RoomPlayer;
using UnityEngine;
using Utils;
using Utils.Extensions;
using Zenject;

namespace PunNetwork.Services.GameNetwork
{
    public class GameNetworkService : MonoBehaviourPunCallbacks, IGameNetworkService
    {
        private IRoomPlayersService _roomPlayersService;
        private LoadingController _loadingController;
        private IPhotonTeamsManager _photonTeamsManager;

        private bool _isMatchEnded;

        [Inject]
        private void Construct
        (
            IRoomPlayersService roomPlayersService,
            LoadingController loadingController
        )
        {
            _roomPlayersService = roomPlayersService;
            _loadingController = loadingController;
        }
        
        public void LeaveGame() => StartCoroutine(HandleGameEnd());

        public override void OnPlayerLeftRoom(Player other)
        {
            CheckIfGameEnded();
        }

        private IEnumerator HandleGameEnd()
        {
            /*if (PhotonNetwork.InRoom)
            {*/
                if (PhotonNetwork.LocalPlayer.GetPhotonTeam() != null) 
                    PhotonNetwork.LocalPlayer.LeaveCurrentTeam();

                PhotonNetwork.LocalPlayer.ResetCustomProperties();
                PhotonNetwork.LeaveRoom();

                while (PhotonNetwork.InRoom || PhotonNetwork.NetworkClientState == ClientState.Leaving)
                    yield return null;
//           }

            _loadingController.Show();

            PhotonNetwork.LoadLevel(SceneNames.Menu);
        }
        private void CheckIfGameEnded()
        {
            if(_isMatchEnded)
                return;
            
            if (!PhotonNetwork.IsMasterClient)
                return;

            var alivePlayers = _roomPlayersService.PlayerViews
                .Where(p => p.CurrentHealthPoints > 0)
                .Select(playerView => playerView.Player)
                .ToList();

            if (alivePlayers.Count == 0)
            {
                Debug.Log("No players are alive.");
                return;
            }

            byte firstPlayerTeam = alivePlayers.First().GetPhotonTeam().Code;
            var allSameTeam = alivePlayers.All(player => player.GetPhotonTeam().Code == firstPlayerTeam);

            if (!allSameTeam) 
                return;
        
            _isMatchEnded = true;
            
            Debug.LogError($"Raise EndMatchEvent {PhotonNetwork.LocalPlayer.ActorNumber}");
            
            GameEventsRaiser.RaiseEvent(GameEventCodes.EndMatchEventCode, firstPlayerTeam);
        }
    }
}