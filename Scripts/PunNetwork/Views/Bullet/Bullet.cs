using Photon.Pun;
using Photon.Pun.UtilityScripts;
using PunNetwork.ObjectPooling;
using PunNetwork.Views.Player;
using UnityEngine;
using Utils;
using Zenject;
using Logger = Utils.Logger;
using static Photon.PhotonUnityNetworking.Code.Common.Enumerators;

namespace PunNetwork.Views.Bullet
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(PhotonView))]
    [RequireComponent(typeof(Renderer))]
    public class Bullet : PhotonPoolObject
    {
        private Rigidbody _rb;
        private bool _isActive;
        
        private const float BulletSpeed = 10f;
        private const float Damage = 33.4f;
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        public void Fire(Vector3 position)
        {
            photonView.RPC(nameof(RPCFire), RpcTarget.AllViaServer, position);
        }

        [PunRPC]
        private void RPCFire(Vector3 position, PhotonMessageInfo info)
        {
            var lag = (float)(PhotonNetwork.Time - info.SentServerTime);
            transform.position = position + transform.forward * BulletSpeed * lag;

            _isActive = true;
        }

        [PunRPC]
        private void RPCDeactivate()
        {
            Logger.Log($"bulletPos: {transform.position}, bullet rot: {transform.rotation}", nameof(RPCDeactivate));
            _isActive = false;
        }

        private void Deactivate()
        {
            PhotonPoolService.DisablePoolItem(GameObjectEntryKey.Bullet.ToString(), this);
            photonView.RPC(nameof(RPCDeactivate), RpcTarget.All);
        }

        private void FixedUpdate()
        {
            if (!_isActive)
                return;

            _rb.velocity = transform.forward * BulletSpeed;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isActive || !photonView.IsMine)
                return;

            var playerView = other.GetComponent<PlayerView>();

            if (playerView != null && playerView.TeamRole == Enumerators.TeamRole.EnemyPlayer)
            {
                playerView.PhotonView.RPC(nameof(PlayerView.RegisterHit), RpcTarget.All, Damage);
                photonView.Owner.AddScore(Damage);
                Logger.Log(this,$"Player{photonView.Owner.ActorNumber} damaged Player {playerView.PhotonView.Owner.ActorNumber}");
                
                Deactivate();
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle")) 
                Deactivate();
        }
    }
}