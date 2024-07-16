using Controllers;
using Services.Window;
using States.Core;
using static Utils.Enumerators;

namespace States
{
    public class GameResultsState : IState
    {
        private readonly MatchResultsController _matchResultsController;
        private readonly IWindowService _windowService;

        public GameResultsState
        (
            MatchResultsController matchResultsController,
            IWindowService windowService
        )
        {
            _matchResultsController = matchResultsController;
            _windowService = windowService;
        }
        
        public void Enter()
        {
            _windowService.Close(EWindow.MatchInfo);
            _matchResultsController.Show();
        }

        public void Exit()
        {
            
        }
    }
}