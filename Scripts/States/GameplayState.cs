using System.Linq;
using Controllers;
using PunNetwork.Services.RoomPlayer;
using Services.Input;
using States.Core;

namespace States
{
    public class GameplayState : IState
    {
        private readonly MatchInfoController _matchInfoController;
        private readonly IInputService _inputService;
        private readonly IRoomPlayersService _roomPlayersService;

        public GameplayState
        (
            MatchInfoController matchInfoController,
            IInputService inputService,
            IRoomPlayersService roomPlayersService
        )
        {
            _matchInfoController = matchInfoController;
            _inputService = inputService;
            _roomPlayersService = roomPlayersService;
        }

        public void Enter()
        {
            _inputService.Enable();
            var playerViews = _roomPlayersService.Players.Select(p
                => _roomPlayersService.GetPlayerInfo(p).View);
            
            foreach (var playerView in playerViews)
                playerView.SubscribeOnInput();
            
            _matchInfoController.Show();
        }

        public void Exit()
        {
        }
    }
}