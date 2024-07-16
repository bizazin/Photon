using Models;
using MvcCore.Abstracts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Views.SelectCharacterView
{
    public class CharacterElementView: View
    {
        [SerializeField] private TextMeshProUGUI _characterNameText;
        [SerializeField] private Button _selectButton;
        [SerializeField] private Image _characterImage;

        public Button Button => _selectButton;

        public void SetUp(CharacterData characterData)
        {
            _characterNameText.text = characterData.Character.ToString();
            _characterImage.sprite = characterData.AvatarSprite;
        }

    }
}