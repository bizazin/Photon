using System;
using Photon.Pun;
using Services.Data;
using States.Core;
using Utils;

namespace States
{
    public class LoadDataState : IState, IDisposable
    {
        private readonly IDataService _dataService;

        public LoadDataState
        (
            IDataService dataService
        )
        {
            _dataService = dataService;
        }

        public void Enter()
        {
            _dataService.DataLoadedEvent += OnDataLoaded;
            _dataService.StartLoading();
        }

        public void Exit()
        {
        }

        public void Dispose()
        {
            _dataService.DataLoadedEvent -= OnDataLoaded;
        }

        private static void OnDataLoaded()
        {
            PhotonNetwork.LoadLevel(SceneNames.Menu);
        }
    }
}