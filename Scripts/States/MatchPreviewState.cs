using Controllers;
using PunNetwork.Services.PlayersStats;
using States.Core;


namespace States
{
    public class MatchPreviewState : IState
    {
        private readonly PreviewMatchAnimationController _previewMatchAnimationController;
        private readonly IPlayersStatsService _playersStatsService;

        public MatchPreviewState
        (
            PreviewMatchAnimationController previewMatchAnimationController,
            IPlayersStatsService playersStatsService
        )
        {
            _previewMatchAnimationController = previewMatchAnimationController;
            _playersStatsService = playersStatsService;
        }

        public void Enter()
        {
            _playersStatsService.SendPersonalInitialStats();
            _previewMatchAnimationController.Start();
        }

        public void Exit()
        {
            _previewMatchAnimationController.Hide();
        }
    }
}