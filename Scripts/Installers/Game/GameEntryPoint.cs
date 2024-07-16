using States;
using States.Core;
using Zenject;

namespace Installers.Game
{
    public class GameEntryPoint : IInitializable
    {
        private readonly IGameStateMachine _gameStateMachine;

        public GameEntryPoint
        (
            IGameStateMachine gameStateMachine
        )
        {
            _gameStateMachine = gameStateMachine;
        }
        
        public void Initialize()
        {
            _gameStateMachine.Enter<PrepareGameState>();
        }
    }
}