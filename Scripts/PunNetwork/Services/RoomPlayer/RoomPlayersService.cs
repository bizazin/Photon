using Models;
using PunNetwork.Services.CustomProperties;
using PunNetwork.Services.ProjectNetwork;
using PunNetwork.Views.Player;
using Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using PunNetwork.Services.MasterEvent;
using static PunNetwork.NetworkData.NetworkDataModel;
using static Utils.Enumerators;

namespace PunNetwork.Services.RoomPlayer
{
   
    public class RoomPlayersService : IRoomPlayersService, IDisposable
    {
        private readonly ICustomPropertiesService _customPropertiesService;
        private readonly IProjectNetworkService _projectNetworkService;
        private readonly IMasterEventService _masterEventService;

        private readonly Dictionary<Player, PlayerInfoVo> _playersDictionary = new();

        private bool IsAllPlayersSpawned => _playersDictionary.Values.All(info => info.IsLocalPlayersSpawned);
        private bool IsAllPoolsPrepared => _playersDictionary.Values.All(info => info.IsLocalPoolsPrepared);
        private bool IsAllPlayersSpawnedLocal => _playersDictionary.Values.All(info => info.View != null);
        private bool IsAllDataGet => _playersDictionary.Values.All(info => info.ImmutableDataVo != null);

        public IEnumerable<Player> Players => _playersDictionary.Keys;
        public IEnumerable<PlayerView> PlayerViews => Players.Select(p=> GetPlayerInfo(p).View);

        public RoomPlayersService
        (
            ICustomPropertiesService customPropertiesService,
            IProjectNetworkService projectNetworkService,
            IMasterEventService masterEventService
        )
        {
            _customPropertiesService = customPropertiesService;
            _projectNetworkService = projectNetworkService;
            _masterEventService = masterEventService;
        }

        public void Dispose()
        {
            _projectNetworkService.PlayerLeftRoomEvent -= LeaveRoom;

            _customPropertiesService.Unsubscribe<string>(PlayerProperty.PlayerImmutableData,
                OnPlayerImmutableDataChanged);
            _customPropertiesService.Unsubscribe<bool>(PlayerProperty.LocalPlayersSpawned,
                OnLocalPlayersSpawnedChanged);
            _customPropertiesService.Unsubscribe<bool>(PlayerProperty.LocalPoolsPrepared, OnLocalPoolsPreparedChanged);
        }

        public PlayerInfoVo GetPlayerInfo(Player player)
        {
            try
            {
                return _playersDictionary[player];
            }
            catch (Exception e)
            {
                throw new Exception(
                    $"[{nameof(RoomPlayersService)}] Player with name {player} was not present in the dictionary. {e.StackTrace}");
            }
        }

        public void EnterRoom(IEnumerable<Player> players)
        {
            foreach (var player in players)
                _playersDictionary.Add(player, new PlayerInfoVo());

            _customPropertiesService.Subscribe<string>(PlayerProperty.PlayerImmutableData, OnPlayerImmutableDataChanged);
            _customPropertiesService.Subscribe<bool>(PlayerProperty.LocalPlayersSpawned, OnLocalPlayersSpawnedChanged);
            _customPropertiesService.Subscribe<bool>(PlayerProperty.LocalPoolsPrepared, OnLocalPoolsPreparedChanged);

            _projectNetworkService.EnterRoom();
            _projectNetworkService.PlayerLeftRoomEvent += LeaveRoom;

            _masterEventService.Setup();
        }

        public void SendPlayerImmutableData(PlayerImmutableDataVo data)
        {
            var json = JsonConvert.SerializeObject(data);
            PhotonNetwork.LocalPlayer.SetCustomProperty(PlayerProperty.PlayerImmutableData, json);
        }

        public void SendLocalPlayersSpawned(Player player, PlayerView playerView)
        {
            var teamRole = player.GetTeamRole();
            playerView.SetTeamRole(teamRole);

            _playersDictionary[player].View = playerView;
            _playersDictionary[player].TeamRole = teamRole;

            if (IsAllPlayersSpawnedLocal)
                PhotonNetwork.LocalPlayer.SetCustomProperty(PlayerProperty.LocalPlayersSpawned, true);
        }

        public void SendLocalPoolsPrepared() =>
            PhotonNetwork.LocalPlayer.SetCustomProperty(PlayerProperty.LocalPoolsPrepared, true);

        private void LeaveRoom(Player player)
        {
            if (player.IsLocal)
            {
                _projectNetworkService.PlayerLeftRoomEvent -= LeaveRoom;

                _playersDictionary.Clear();

                _customPropertiesService.Unsubscribe<string>(PlayerProperty.PlayerImmutableData,
                    OnPlayerImmutableDataChanged);
                _customPropertiesService.Unsubscribe<bool>(PlayerProperty.LocalPlayersSpawned,
                    OnLocalPlayersSpawnedChanged);
                _customPropertiesService.Unsubscribe<bool>(PlayerProperty.LocalPoolsPrepared,
                    OnLocalPoolsPreparedChanged);
            }
            else
                _playersDictionary.Remove(player);
        }

        private void OnPlayerImmutableDataChanged(Player player, string value)
        {
            _playersDictionary[player].ImmutableDataVo = JsonConvert.DeserializeObject<PlayerImmutableDataVo>(value);

            if (IsAllDataGet)
                _masterEventService.OnAllDataGet();
        }

        private void OnLocalPlayersSpawnedChanged(Player player, bool value)
        {
            _playersDictionary[player].IsLocalPlayersSpawned = value;

            if (IsAllPlayersSpawned)
                _masterEventService.OnAllPlayersSpawned();
        }

        private void OnLocalPoolsPreparedChanged(Player player, bool value)
        {
            _playersDictionary[player].IsLocalPoolsPrepared = value;

            if (IsAllPoolsPrepared)
                _masterEventService.OnAllPoolsPrepared();
        }
    }
}