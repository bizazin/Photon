using Databases.Interfaces;
using MvcCore.Abstracts;
using Services.Data;
using Services.Window;
using UnityEngine;
using Utils;
using Views.MainMenuView;

namespace Controllers.MainMenu
{
    public class SelectedCharacterHandler : Handler<SelectedCharacterPanel>
    {
        private readonly IDataService _dataService;
        private readonly IWindowService _windowService;
        private readonly ICharactersVisualDatabase _charactersVisualDatabase;

        public SelectedCharacterHandler
        (
            IDataService dataService,
            IWindowService windowService,
            ICharactersVisualDatabase charactersVisualDatabase
        )
        {
            _dataService = dataService;
            _windowService = windowService;
            _charactersVisualDatabase = charactersVisualDatabase;
        }

        protected override void Initialize()
        {
            Debug.Log("SelectedCharacterHandler initialized");

            var characterData = _charactersVisualDatabase.CharactersDataData.CharactersData.Find(
                c=> c.Character == _dataService.CachedUserLocalData.SelectedCharacter);
            var characterSprite = characterData.FullImage;
            
            View.SetCharacterImage(characterSprite, _dataService.CachedUserLocalData.SelectedCharacter.ToString());
            View.CharacterButton.onClick.AddListener(CharacterButtonClick);
        }


        private void CharacterButtonClick()
        {
            _windowService.Open(Enumerators.EWindow.SelectCharacter);
        }
    }
}