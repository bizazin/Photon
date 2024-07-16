using System.Collections;
using Behaviours;
using DG.Tweening;
using MvcCore.Abstracts;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using PunNetwork.Services.GameNetwork;
using PunNetwork.Services.MatchInfo;
using Views;
using Views.MatchResultView;
using static Photon.PhotonUnityNetworking.Code.Common.Enumerators;
using static Utils.Enumerators;


namespace Controllers
{
    public class MatchResultsController : Controller<MatchResultsView>
    {
        private readonly LoadingController _loadingController;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly IGameNetworkService _gameNetworkService;
        private readonly IMatchInfoService _matchInfoService;

        public MatchResultsController
        (
            LoadingController loadingController,
            ICoroutineRunner coroutineRunner,
            IGameNetworkService gameNetworkService,
            IMatchInfoService matchInfoService
        )
        {
            _loadingController = loadingController;
            _coroutineRunner = coroutineRunner;
            _gameNetworkService = gameNetworkService;
            _matchInfoService = matchInfoService;
        }
        
        public void Show()
        {
            View.Reset();
            View.Show();
            View.PlayAnimation(_matchInfoService.GameResult).OnComplete(() => _gameNetworkService.LeaveGame());
        }
        
    }
}