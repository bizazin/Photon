using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

namespace Utils
{
    public static class GameEventsRaiser
    {
        public static void RaiseEvent(byte eventByteIndex, object eventContent)
        {
            var raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            var sendOptions = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(eventByteIndex, eventContent, raiseEventOptions, sendOptions);
        }
    }
}