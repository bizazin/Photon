using Photon.PhotonUnityNetworking.Code.Common;
using Zenject;

namespace States.Core
{
    public class GameStateMachine : IGameStateMachine, ISceneContainerInjectable
    {
        private DiContainer _container;  
        private IExitableState _activeState;

        public void SetSceneContainer
        (
            DiContainer container
        )
        {
            _container = container;
        }

        public void Enter<TState>() where TState : class, IState
        {
            ChangeState<TState>().Enter();
        }

        public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload>
        {
            ChangeState<TState>().Enter(payload);
        }

        private TState ChangeState<TState>() where TState : class, IExitableState
        {
            _activeState?.Exit();
            var state = _container.Instantiate<TState>();
            _activeState = state;
            return state;
        }
    }
}