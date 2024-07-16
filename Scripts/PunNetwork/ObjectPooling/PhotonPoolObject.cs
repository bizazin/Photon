using Photon.PhotonUnityNetworking.Code.Common;
using Photon.Pun;
using Services.PhotonPool;
using UnityEngine;
using Zenject;

namespace PunNetwork.ObjectPooling
{
    public class PhotonPoolObject : MonoBehaviourPun, IPunInstantiateMagicCallback, IPunObservable
    {
        [SerializeField] private Enumerators.GameObjectEntryKey _key;
        protected IPhotonPoolService PhotonPoolService;
        private int _ownerID;

        [Inject]
        private void Construct
        (
            IPhotonPoolService photonPoolService
        )
        {
            PhotonPoolService = photonPoolService;
        }

        public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            PhotonPoolService.SetItemReady(_key.ToString(), this);
        }


        public void ActivateBase(Vector3 position, Quaternion rotation)
        {
            if (!photonView.IsMine)
            {
                _ownerID = PhotonNetwork.LocalPlayer.ActorNumber;
                photonView.TransferOwnership(_ownerID);
            }
            photonView.RPC(nameof(RPCActivate), RpcTarget.AllViaServer, position, rotation);
        }

        public void DisableBase()
        {
            photonView.RPC(nameof(RPCDisable), RpcTarget.AllViaServer);
        }

        [PunRPC]
        protected void RPCActivate(Vector3 position, Quaternion rotation)
        {
            transform.position = position;
            transform.rotation = rotation;
            gameObject.SetActive(true);
            Activate();
        }

        [PunRPC]
        protected void RPCDisable()
        {
            gameObject.SetActive(false);
            Disable();
        }

        protected virtual void Activate()
        {
            
        }

        protected virtual void Disable()
        {
            
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
                stream.SendNext(_ownerID);
            else
                _ownerID = (int)stream.ReceiveNext();
        }
    }
}