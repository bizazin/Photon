using Utils;

namespace Services.Window
{
    public interface IWindowService
    {
        void ClearWindows();
        void RegisterWindow(MvcCore.Abstracts.Window window, bool isFocusable, int orderNumber, bool isDontDestroyOnLoad);
        void Open(Enumerators.EWindow windowName);
        void Close(Enumerators.EWindow windowName);
        void SortBySiblingIndex();
    }
}