using Controllers;
using Controllers.MainMenu;
using States.Core;

namespace States
{
    public class MenuState : IState
    {
        private readonly LoadingController _loadingController;
        private readonly MainMenuController _mainMenuController;

        public MenuState
        (
            LoadingController loadingController,
            MainMenuController mainMenuController
        )
        {
            _loadingController = loadingController;
            _mainMenuController = mainMenuController;
        }

        public void Enter()
        {
            _mainMenuController.Setup();
            _loadingController.Hide();
        }

        public void Exit()
        {
        }
    }
}