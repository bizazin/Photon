using System.Collections.Generic;
using System.Linq;
using Models;
using Utils.Extensions;
using static Utils.Enumerators;

namespace Services.Window
{
    public class WindowService : IWindowService
    {
        private readonly Dictionary<EWindow, WindowVo> _windows = new();

        private EWindow? _focusedWindow;

        public void ClearWindows()
        {
            var windowsToKeep = _windows.Where(kvp => kvp.Value.IsDontDestroyOnLoad).ToList();

            _windows.Clear();

            foreach (var window in windowsToKeep) 
                _windows.Add(window.Key, window.Value);
            BindExtensions.ClearWindows();
            
            _focusedWindow = null;
        }

        public void RegisterWindow(MvcCore.Abstracts.Window window, bool isFocusable, int orderNumber,
            bool isDontDestroyOnLoad) => _windows.Add(window.Name,
            new WindowVo
            {
                Window = window, 
                IsFocusable = isFocusable, 
                OrderNumber = orderNumber,
                IsDontDestroyOnLoad = isDontDestroyOnLoad
            });

        public void Open(EWindow windowName)
        {
            var windowVo = _windows[windowName];
            windowVo.Window.Open();
            if (!windowVo.IsFocusable) 
                return;

            if (_focusedWindow != null && _focusedWindow != windowName)
                _windows[_focusedWindow.Value].Window.Close();
            _focusedWindow = windowName;
        }

        public void Close(EWindow windowName)
        {
            var windowVo = _windows[windowName];
            windowVo.Window.Close();
        }

        public void SortBySiblingIndex()
        {
            foreach (var windowVo in _windows.Values)
                windowVo.Window.transform.SetSiblingIndex(windowVo.OrderNumber);
        }
    }
}