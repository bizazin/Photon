namespace PunNetwork.Services.PlayersStats
{
    public interface IPlayersStatsService
    {
        void SendPlayerHp(float healthPoints);
        void SendPersonalInitialStats();
    }
}