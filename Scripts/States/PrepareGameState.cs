using Controllers;
using PunNetwork.Services.GameNetwork;
using PunNetwork.Services.ProjectNetwork;
using PunNetwork.Services.SpawnPlayer;
using Services.PhotonPool;
using States.Core;

namespace States
{
    public class PrepareGameState : IState
    {
        private readonly LoadingController _loadingController;
        private readonly IGameNetworkService _gameNetworkService;
        private readonly IProjectNetworkService _projectNetworkService;
        private readonly IPhotonPoolService _photonPoolService;
        private readonly ISpawnPlayerService _spawnPlayerService;

        public PrepareGameState
        (
            LoadingController loadingController,
            IGameNetworkService gameNetworkService,
            IProjectNetworkService projectNetworkService,
            IPhotonPoolService photonPoolService,
            ISpawnPlayerService spawnPlayerService
        )
        {
            _loadingController = loadingController;
            _gameNetworkService = gameNetworkService;
            _projectNetworkService = projectNetworkService;
            _photonPoolService = photonPoolService;
            _spawnPlayerService = spawnPlayerService;
        }

        public void Enter()
        {
            _photonPoolService.PreparePools();
            _spawnPlayerService.SpawnPlayer();
        }

        public void Exit()
        {
            _loadingController.Hide();
        }
    }
}