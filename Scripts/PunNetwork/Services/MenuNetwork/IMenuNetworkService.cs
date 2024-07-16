    using System;

    namespace PunNetwork.Services.MenuNetwork
{
    public interface IMenuNetworkService
    {
        void Connect();
        void SetMaxPlayers(byte count);
    }
}