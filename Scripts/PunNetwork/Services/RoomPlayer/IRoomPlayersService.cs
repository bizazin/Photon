using System.Collections.Generic;
using Models;
using Photon.Realtime;
using PunNetwork.NetworkData;
using PunNetwork.Views.Player;

namespace PunNetwork.Services.RoomPlayer
{
    public interface IRoomPlayersService
    {
        void SendLocalPlayersSpawned(Player player, PlayerView playerView);
        void SendLocalPoolsPrepared();
        void SendPlayerImmutableData(NetworkDataModel.PlayerImmutableDataVo data);
        void EnterRoom(IEnumerable<Player> players);
        PlayerInfoVo GetPlayerInfo(Player player);
        IEnumerable<Player> Players { get; }
        public IEnumerable<PlayerView> PlayerViews { get; }
    }
}