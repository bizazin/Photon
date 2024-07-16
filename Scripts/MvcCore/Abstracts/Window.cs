using MvcCore.Interfaces;
using static Utils.Enumerators;

namespace MvcCore.Abstracts
{
    public abstract class Window : View, IWindow
    {
        public abstract EWindow Name { get; }

        public virtual void Open() => Show();

        public virtual void Close() => Hide();
    }
}