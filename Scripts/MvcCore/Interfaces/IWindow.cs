using static Utils.Enumerators;

namespace MvcCore.Interfaces
{
    public interface IWindow
    {
        EWindow Name { get; }
        void Open();
        void Close();
    }
}