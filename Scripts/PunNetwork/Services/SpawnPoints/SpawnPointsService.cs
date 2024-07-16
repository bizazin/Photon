using Photon.Pun;
using Photon.Pun.UtilityScripts;
using PunNetwork.PhotonTeams;
using UnityEngine;

namespace PunNetwork.Services.SpawnPoints
{
    public class SpawnPointsService : MonoBehaviour, ISpawnPointsService
    {
        [SerializeField] private Transform _leftSpawnArea;
        [SerializeField] private Transform _rightSpawnArea;

        private const float IntervalZ = 2;

        public Vector3 GetPlayerPosition(int index, PhotonTeam team)
        {
            var playerArea = team.Name == "Blue" ? _leftSpawnArea : _rightSpawnArea;
            
            var middlePoint = (PhotonNetwork.PlayerList.Length - 1) * IntervalZ / 2;

            var valueZ = index * IntervalZ - middlePoint;
            return new Vector3(playerArea.position.x,playerArea.position.y,valueZ);
        }
    }
}