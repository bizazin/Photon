using MvcCore.Abstracts;
using PunNetwork.Services.GameNetwork;
using UniRx;
using UnityEngine.UI;
using Views;
using Views.MatchInfoView;

namespace Controllers
{
    public class MatchInfoController : Controller<MatchInfoView>
    {
        private readonly IGameNetworkService _gameNetworkService;
        private readonly Button _leaveButton;

        public MatchInfoController
        (
            IGameNetworkService gameNetworkService
        )
        {
            _gameNetworkService = gameNetworkService;
        }

        public override void Initialize()
        {
            View.LeaveButton.OnClickAsObservable().Subscribe(_ => OnLeaveButton()).AddTo(View);
        }

        public void Show()
        {
            View.Reset();
            View.Show();
        }

        private void OnLeaveButton() => _gameNetworkService.LeaveGame();
    }
}