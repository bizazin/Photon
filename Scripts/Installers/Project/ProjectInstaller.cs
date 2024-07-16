using Photon.PhotonUnityNetworking.Code.Common.Factory;
using Photon.Realtime;
using PunNetwork.Services.MasterEvent;
using PunNetwork.Services.PlayersStats;
using PunNetwork.Services.RoomPlayer;
using Services.Data;
using Services.Localization;
using Services.Network;
using Services.Pool;
using Services.SceneLoading;
using Services.Sound.Impls;
using Services.Spreadsheets;
using Services.Window;
using States.Core;
using Zenject;

namespace Installers.Project
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
            BindServices();
        }

        private void BindServices()
        {
            var loadBalancingClient = new LoadBalancingClient();
            Container.Bind<ILoadBalancingClient>().FromInstance(loadBalancingClient).AsSingle();
            Container.BindInterfacesTo<NetworkService>().AsSingle();
            Container.BindInterfacesTo<DataService>().AsSingle();
            Container.BindInterfacesTo<LocalizationService>().AsSingle();
            Container.BindInterfacesTo<SpreadsheetsService>().AsSingle();
            Container.BindInterfacesTo<WindowService>().AsSingle();
            Container.BindInterfacesTo<SceneLoadingService>().AsSingle();
			Container.BindInterfacesTo<GameStateMachine>().AsSingle();
            Container.BindInterfacesTo<GameFactory>().AsSingle();
            Container.BindInterfacesTo<PoolService>().AsSingle();
            Container.BindInterfacesTo<RoomPlayersService>().AsSingle();
            Container.BindInterfacesTo<MasterEventService>().AsSingle();

            Container.BindInterfacesTo<AudioSourcePool>().AsSingle();
            Container.BindInterfacesTo<SoundService>().AsSingle();
        }
    }
}