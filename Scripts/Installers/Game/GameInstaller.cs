using PunNetwork.Services.MatchInfo;
using PunNetwork.Services.PlayersStats;
using PunNetwork.Services.RoomPlayer;
using PunNetwork.Services.SpawnPlayer;
using PunNetwork.Services.SpawnPoints;
using Services.Input;
using Services.PhotonPool;
using Signals;
using UnityEngine;
using Utils.Extensions;
using Zenject;

namespace Installers.Game
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private SpawnPointsService _spawnPointsService;
        
        public override void InstallBindings()
        {
            BindSignals();
            BindServices();
            Container.InjectSceneContainer();
        }

        private void BindSignals()
        {
            Container.DeclareSignal<SignalMainSetup>();
        }
        
        private void BindServices()
        {
            Container.Bind<ISpawnPointsService>().FromInstance(_spawnPointsService).AsSingle();
            Container.BindInterfacesTo<PhotonPoolService>().AsSingle();
            Container.BindInterfacesTo<InputService>().AsSingle();
            Container.BindInterfacesTo<SpawnPlayerService>().AsSingle();
            Container.BindInterfacesTo<MatchInfoService>().AsSingle();
            Container.BindInterfacesTo<PlayersStatsService>().AsSingle();

            Container.BindInterfacesTo<GameEntryPoint>().AsSingle();
        }
    }
}