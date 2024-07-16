using System;
using System.Linq;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using PunNetwork.PhotonTeams;
using PunNetwork.Services.MasterEvent;
using States;
using States.Core;
using UnityEngine;
using Utils;
using Zenject;
using static Utils.Enumerators;


namespace PunNetwork.Services.MatchInfo
{
    public class MatchInfoService : IMatchInfoService, IInitializable, IDisposable
    {
        private readonly IMasterEventService _masterEventService;
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IPhotonTeamsManager _photonTeamsManager;

        public GameResult GameResult { get; private set; }

        public MatchInfoService
        (
            IMasterEventService masterEventService,
            IGameStateMachine gameStateMachine,
            IPhotonTeamsManager photonTeamsManager
        )
        {
            _masterEventService = masterEventService;
            _gameStateMachine = gameStateMachine;
            _photonTeamsManager = photonTeamsManager;
        }

        public void Initialize()
        {
            _masterEventService.Subscribe(GameEventCodes.StartMatchEventCode, OnStartMatch);
            _masterEventService.Subscribe<byte>(GameEventCodes.EndMatchEventCode, OnEndMatch);
            _photonTeamsManager.TeamDeadEvent += TeamDeadHandler;
        }

        public void Dispose()
        {
            _masterEventService.Unsubscribe(GameEventCodes.StartMatchEventCode, OnStartMatch);
            _masterEventService.Unsubscribe<byte>(GameEventCodes.EndMatchEventCode, OnEndMatch);
            _photonTeamsManager.TeamDeadEvent -= TeamDeadHandler;
        }

        private void TeamDeadHandler()
        {
            var survivingTeam = CheckIsGameEnded();
            if (survivingTeam != null)
                _masterEventService.RaiseEvent(GameEventCodes.EndMatchEventCode, survivingTeam.Code);
        }

        private PhotonTeam CheckIsGameEnded()
        {
            var allTeams = _photonTeamsManager.GetAllTeams();
            var aliveTeams = allTeams.Values.Where(team => team.IsAlive).ToList();

            return aliveTeams.Count == 1 ? aliveTeams[0] : null;
        }

        private void OnStartMatch(object eventContent)
        {
            _gameStateMachine.Enter<GameplayState>();
        }

        private void OnEndMatch(byte winningTeam)
        {
            GameResult = winningTeam == PhotonNetwork.LocalPlayer.GetPhotonTeam().Code
                ? GameResult.Win
                : GameResult.Lose;
            _gameStateMachine.Enter<GameResultsState>();
        }
    }
}