using Photon.Pun;
using Photon.Pun.UtilityScripts;
using PunNetwork.PhotonTeams;
using PunNetwork.Services.RoomPlayer;
using PunNetwork.Services.SpawnPoints;
using UnityEngine;

namespace PunNetwork.Services.SpawnPlayer
{
    public class SpawnPlayerService : ISpawnPlayerService
    {
        private readonly IRoomPlayersService _roomPlayersService;
        private readonly ISpawnPointsService _spawnPointsService;

        public SpawnPlayerService
        (
            IRoomPlayersService roomPlayersService,
            ISpawnPointsService spawnPointsService
        )
        {
            _roomPlayersService = roomPlayersService;
            _spawnPointsService = spawnPointsService;
        }

        public void SpawnPlayer()
        {
            var photonTeam = PhotonNetwork.LocalPlayer.GetPhotonTeam();
            var playerPosition =
                _spawnPointsService.GetPlayerPosition(PhotonNetwork.LocalPlayer.ActorNumber - 1, photonTeam);

            PhotonNetwork.Instantiate(
                _roomPlayersService.GetPlayerInfo(PhotonNetwork.LocalPlayer).ImmutableDataVo.CharacterName, playerPosition,
                Quaternion.identity);
        }
    }
}