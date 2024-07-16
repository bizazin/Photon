using MvcCore.Abstracts;
using UnityEngine;
using UnityEngine.UI;

namespace Views.SelectCharacterView
{
    public class CharacterPagePanel: View
    {
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _selectButton;
        [SerializeField] private Image _characterImage;

        public Button BackButton => _backButton;
        public Button SelectButton => _selectButton;
        public Image CharacterImage => _characterImage;
        
        public void SetCharacterImage(Sprite sprite)
        {
            _characterImage.sprite = sprite;
        }
    }
}