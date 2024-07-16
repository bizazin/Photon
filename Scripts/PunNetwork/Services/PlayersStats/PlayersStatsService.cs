using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using PunNetwork.PhotonTeams;
using PunNetwork.Services.CustomProperties;
using PunNetwork.Services.RoomPlayer;
using Utils;
using Utils.Extensions;
using Zenject;
using static PunNetwork.NetworkData.NetworkDataModel;

namespace PunNetwork.Services.PlayersStats
{
    public class PlayersStatsService : IPlayersStatsService, IInitializable, IDisposable
    {
        private readonly ICustomPropertiesService _customPropertiesService;
        private readonly IRoomPlayersService _roomPlayersService;
        private readonly IPhotonTeamsManager _photonTeamsManager;
        private readonly Dictionary<Player, StatsValuesVo> _playersStats = new();

        public PlayersStatsService
        (
            ICustomPropertiesService customPropertiesService,
            IRoomPlayersService roomPlayersService,
            IPhotonTeamsManager photonTeamsManager
        )
        {
            _customPropertiesService = customPropertiesService;
            _roomPlayersService = roomPlayersService;
            _photonTeamsManager = photonTeamsManager;
        }

        public void Initialize()
        {
            foreach (var player in _roomPlayersService.Players)
                _playersStats.Add(player, _roomPlayersService.GetPlayerInfo(player).ImmutableDataVo.InitialStats);

            _customPropertiesService.Subscribe<float>(Enumerators.PlayerProperty.PlayerHP, OnPlayerHPChanged);
            _customPropertiesService.Subscribe<bool>(Enumerators.PlayerProperty.IsDead, OnPlayerDead);
        }

        public void Dispose()
        {
            _customPropertiesService.Unsubscribe<float>(Enumerators.PlayerProperty.PlayerHP, OnPlayerHPChanged);
            _customPropertiesService.Unsubscribe<bool>(Enumerators.PlayerProperty.IsDead, OnPlayerDead);
        }

        public void SendPlayerHp(float healthPoints)
        {
            PhotonNetwork.LocalPlayer.SetCustomProperty(Enumerators.PlayerProperty.PlayerHP, healthPoints);

            if (healthPoints <= 0)
                SendPlayerDead();
        }

        public void SendPersonalInitialStats()
        {
            var healthPoints = _playersStats[PhotonNetwork.LocalPlayer].HealthPoints;
            PhotonNetwork.LocalPlayer.SetCustomProperty(Enumerators.PlayerProperty.PlayerHP, healthPoints);
        }

        private static void SendPlayerDead()
        {
            PhotonNetwork.LocalPlayer.SetCustomProperty(Enumerators.PlayerProperty.IsDead, true);
        }

        private void OnPlayerHPChanged(Player player, float hp)
        {
            var playerView = _roomPlayersService.GetPlayerInfo(player).View;
            playerView.UpdateHealthPoints(hp);

            _playersStats[player].HealthPoints = hp;
        }

        private void OnPlayerDead(Player player, bool state)
        {
            _playersStats[player].IsDead = state;
            _photonTeamsManager.PlayerDeadHandler(player, state);
        }
    }
}