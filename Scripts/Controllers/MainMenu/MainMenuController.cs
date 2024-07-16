using MvcCore.Abstracts;
using PunNetwork.Services.MenuNetwork;
using Services.Window;
using UniRx;
using Views.MainMenuView;
using UnityEngine;
using Utils;

namespace Controllers.MainMenu
{
    public class MainMenuController : Controller<MainMenuView>
    {
        private readonly IMenuNetworkService _menuNetworkService;
        private readonly SelectedCharacterHandler _selectedCharacterHandler;
        private readonly MenuProfileHandler _menuProfileHandler;
        private readonly IWindowService _windowService;

        public MainMenuController
        (
            IMenuNetworkService menuNetworkService,
            SelectedCharacterHandler selectedCharacterHandler,
            MenuProfileHandler menuProfileHandler,
            IWindowService windowService
        )
        {
            _menuNetworkService = menuNetworkService;
            _selectedCharacterHandler = selectedCharacterHandler;
            _menuProfileHandler = menuProfileHandler;
            _windowService = windowService;
        }

        public override void Initialize()
        {
            View.PlayButton1.OnClickAsObservable().Subscribe(_ => ConnectOne()).AddTo(View);
            View.PlayButton2.OnClickAsObservable().Subscribe(_ => ConnectTwo()).AddTo(View);

            View.QuitButton.OnClickAsObservable().Subscribe(_ =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
            }).AddTo(View);
            
            _selectedCharacterHandler.Setup(View.SelectedCharacterPanel);
            _menuProfileHandler.Setup(View.MenuProfilePanel);
        }

        public void Setup()
        {
            _windowService.Open(Enumerators.EWindow.MainMenu);
        }

        private void ConnectOne()
        {
            _menuNetworkService.SetMaxPlayers(1);
            _menuNetworkService.Connect();
        }

        private void ConnectTwo()
        {
            _menuNetworkService.SetMaxPlayers(2);
            _menuNetworkService.Connect();
        }
    }
}