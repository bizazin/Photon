using System.Collections.Generic;
using DG.Tweening;
using Photon.PhotonUnityNetworking.Code.Common;
using UnityEngine;
using Utils;
using static Utils.Enumerators;
using Enumerators = Photon.PhotonUnityNetworking.Code.Common.Enumerators;

namespace PunNetwork.Views.Player
{
    [RequireComponent(typeof(SphereCollider))]
    public class EnemiesTriggerCollider : MonoBehaviour
    {
        private readonly List<Transform> _enemiesInRange = new();
        private float _radius;

        private void Awake()
        {
            _radius = GetComponent<SphereCollider>().radius;
        }

        private void Start() => DOVirtual.DelayedCall(.1f, RemoveDistantEnemies);

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<PlayerView>(out var playerView) ||
                playerView.TeamRole != TeamRole.EnemyPlayer || playerView.PhotonView.IsMine) return;
            if (_enemiesInRange.Contains(other.transform)) return;
            _enemiesInRange.Add(other.transform);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent<PlayerView>(out var playerView) ||
                playerView.TeamRole != TeamRole.EnemyPlayer) return;
            if (!_enemiesInRange.Contains(other.transform)) return;
            _enemiesInRange.Remove(other.transform);
        }

        public bool TryGetNearestEnemy(out Transform nearestEnemy)
        {
            nearestEnemy = null;
            var minDistance = Mathf.Infinity;
            var found = false;

            foreach (var enemy in _enemiesInRange)
            {
                var distance = Vector3.Distance(transform.position, enemy.position);
                if (!(distance < minDistance)) continue;
                minDistance = distance;
                nearestEnemy = enemy;
                found = true;
            }

            return found;
        }

        private void RemoveDistantEnemies() => _enemiesInRange.RemoveAll(enemy => Vector3.Distance(transform.position, enemy.position) > _radius);
    }
}