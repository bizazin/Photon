using MvcCore.Abstracts;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils;

namespace Views.MainMenuView
{
    public class MainMenuView : Window
    {
        [SerializeField] private Button _playButton1;
        [SerializeField] private Button _playButton2;
        [SerializeField] private Button _quitButton;
        [SerializeField] private SelectedCharacterPanel _selectedCharacterPanel;
        [SerializeField] private MenuProfilePanel _menuProfilePanel;

        public override Enumerators.EWindow Name => Enumerators.EWindow.MainMenu;

        public Button PlayButton1 => _playButton1;
        public Button PlayButton2 => _playButton2;
        public Button QuitButton => _quitButton;
        public SelectedCharacterPanel SelectedCharacterPanel => _selectedCharacterPanel;
        public MenuProfilePanel MenuProfilePanel => _menuProfilePanel;

        public override void Show()
        {
            base.Show();
        }

        public void EnableSelectedCharacterPanel(bool state)
        {
            if (state)
                _selectedCharacterPanel.Show();
            else
                _selectedCharacterPanel.Hide();
        }

        /*
        public void ShowSelectedCharacterPanel()
        {
            _selectedCharacterPanel.Show();
        }
        */

    }
}