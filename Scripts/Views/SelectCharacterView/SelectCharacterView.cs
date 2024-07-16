using MvcCore.Abstracts;
using UnityEngine;
using static Utils.Enumerators;

namespace Views.SelectCharacterView
{
    public class SelectCharacterView: Window
    {
        [SerializeField] private CharactersListPanel _charactersListPanel;
        [SerializeField] private CharacterPagePanel _characterPagePanel;
        
        public override EWindow Name => EWindow.SelectCharacter;
        
        public CharacterPagePanel CharacterPagePanel => _characterPagePanel;
        public CharactersListPanel CharactersListPanel => _charactersListPanel;

        public override void Show()
        {
            base.Show();
            EnableCharactersListPanel(true);
        }

        public void EnableCharacterPagePanel(bool state)
        {
            if (state)
                _characterPagePanel.Show();
            else
                _characterPagePanel.Hide();
        }
        
        public void EnableCharactersListPanel(bool state)
        {
            if (state)
                _charactersListPanel.Show();
            else
                _charactersListPanel.Hide();
        }

    }
}