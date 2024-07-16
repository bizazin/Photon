using UnityEngine;
using System.Diagnostics;

namespace Helpers
{
    public class GameRestarter : MonoBehaviour, IGameRestarter
    {
        private static string m_gameExecutablePath = string.Empty;


        private void Start()
        {
            if (!string.IsNullOrEmpty(m_gameExecutablePath)) return;
            var processModule = Process.GetCurrentProcess().MainModule;
            if (processModule != null)
                m_gameExecutablePath = processModule.FileName;
        }

        public void RestartGame()
        {
            Process.Start(m_gameExecutablePath); 
            Application.Quit(); 
        }
    }

    public interface IGameRestarter
    {
        void RestartGame();
    }
}