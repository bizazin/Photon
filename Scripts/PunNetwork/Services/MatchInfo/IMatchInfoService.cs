using static Utils.Enumerators;


namespace PunNetwork.Services.MatchInfo
{
    public interface IMatchInfoService
    {
        GameResult GameResult { get; }
    }
}