using Collections;
using MvcCore.Abstracts;
using UnityEngine;
using UnityEngine.UI;

namespace Views.SelectCharacterView
{
    public class CharactersListPanel: View
    { 
        [SerializeField] private Button _backButton;
        [SerializeField] private CharactersCollection _charactersCollection;
        
        public Button BackButton => _backButton;
        public CharactersCollection CharactersCollection => _charactersCollection;

    }
}