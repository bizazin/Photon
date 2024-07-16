using States;
using States.Core;
using Zenject;

namespace Installers.Menu
{
    public class MenuEntryPoint : IInitializable
    {
        private readonly IGameStateMachine _gameStateMachine;

        public MenuEntryPoint
        (
            IGameStateMachine gameStateMachine
        )
        {
            _gameStateMachine = gameStateMachine;
        }
        
        public void Initialize()
        {
            _gameStateMachine.Enter<MenuState>();
        }
    }
}