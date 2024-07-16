using PunNetwork.Services.RoomPlayer;
using Services.Data;
using static PunNetwork.NetworkData.NetworkDataModel;

namespace PunNetwork.Services.PlayerData
{
    public class PlayerDataService : IPlayerDataService
    {
        private readonly IDataService _dataService;
        private readonly IRoomPlayersService _roomPlayersService;

        public PlayerDataService
        (
            IDataService dataService,
            IRoomPlayersService roomPlayersService
        )
        {
            _dataService = dataService;
            _roomPlayersService = roomPlayersService;
        }

        public void SendImmutableData()
        {
            var playerImmutableDataVo = new PlayerImmutableDataVo
            {
                Nickname = _dataService.CachedUserLocalData.NickName,
                CharacterName = _dataService.CachedUserLocalData.SelectedCharacter.ToString(),
                AvatarID = 1,
                InitialStats = new StatsValuesVo
                {
                    HealthPoints = 100,
                }
            };

            _roomPlayersService.SendPlayerImmutableData(playerImmutableDataVo);
        }
    }
}