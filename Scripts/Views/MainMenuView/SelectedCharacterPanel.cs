using MvcCore.Abstracts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Views.MainMenuView
{
    public class SelectedCharacterPanel: View
    {
        [SerializeField] private Button _characterButton;
        [SerializeField] private Image _selectedCharacterImage;
        [SerializeField] private TextMeshProUGUI _characterName;

        public Button CharacterButton => _characterButton;
        
        private void Start()
        {
            Debug.Log("start");
        }

        public void SetCharacterImage(Sprite sprite, string characterName)
        {
            _selectedCharacterImage.sprite = sprite;
            _characterName.text = characterName;
        }
    }
    
}