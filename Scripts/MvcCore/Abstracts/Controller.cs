using MvcCore.Interfaces;
using Zenject;

namespace MvcCore.Abstracts
{
    public abstract class Controller<T> : IInitializable, IController where T : IView
    {
        [Inject] protected readonly T View;
        public virtual void Initialize()
        {}
    }
}