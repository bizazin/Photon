using Photon.Pun.UtilityScripts;
using PunNetwork.PhotonTeams;
using UnityEngine;

namespace PunNetwork.Services.SpawnPoints
{
    public interface ISpawnPointsService
    {
        // Vector3 GetSpawnPoint(int totalPlayers, int playerIndex);
        public Vector3 GetPlayerPosition(int index, PhotonTeam team);

    }}