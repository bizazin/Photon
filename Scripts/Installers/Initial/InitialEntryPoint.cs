using States;
using States.Core;
using Zenject;

namespace Installers.Initial
{
    public class InitialEntryPoint : IInitializable
    {
        private readonly IGameStateMachine _gameStateMachine;

        public InitialEntryPoint
        (
            IGameStateMachine gameStateMachine
        )
        {
            _gameStateMachine = gameStateMachine;
        }
        
        public void Initialize()
        {
            _gameStateMachine.Enter<PhotonConnectionState>();
        }
    }
}