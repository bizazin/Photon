using Controllers;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using PunNetwork.PhotonTeams;
using PunNetwork.Services.CustomProperties;
using PunNetwork.Services.ProjectNetwork;
using States.Core;
using UnityEngine;
using Utils;

namespace States
{
    public class PhotonConnectionState : IState
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly LoadingController _loadingController;
        private readonly ILoadBalancingClient _loadBalancingClient;
        private readonly ICustomPropertiesService _customPropertiesService;
        private readonly IPhotonTeamsManager _photonTeamsManager;
        private readonly IProjectNetworkService _projectNetworkService;

        public PhotonConnectionState
        (
            IGameStateMachine gameStateMachine,
            LoadingController loadingController,
            ILoadBalancingClient loadBalancingClient,
            ICustomPropertiesService customPropertiesService,
            IPhotonTeamsManager photonTeamsManager,
            IProjectNetworkService projectNetworkService
        )
        {
            _gameStateMachine = gameStateMachine;
            _loadingController = loadingController;
            _loadBalancingClient = loadBalancingClient;
            _customPropertiesService = customPropertiesService;
            _photonTeamsManager = photonTeamsManager;
            _projectNetworkService = projectNetworkService;
        }
        
        public void Enter()
        {
            _loadBalancingClient.AddCallbackTarget(_customPropertiesService);
            _projectNetworkService.ConnectedToMasterEvent += OnConnectedToMaster;
            
            _loadingController.Show();
            
            PhotonNetwork.AutomaticallySyncScene = true;

            if (!PhotonNetwork.IsConnectedAndReady)
            {
                Debug.Log("Connecting to server..");
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        public void Exit()
        {
            _projectNetworkService.ConnectedToMasterEvent -= OnConnectedToMaster;
        }

        private void OnConnectedToMaster()
        {
            _gameStateMachine.Enter<LoadDataState>();
        }
    }
}