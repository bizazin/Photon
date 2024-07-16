using Databases.Interfaces;
using MvcCore.Abstracts;
using Services.Window;
using UniRx;
using UnityEngine;
using Views.SelectCharacterView;
using static Photon.PhotonUnityNetworking.Code.Common.Enumerators;
using static Utils.Enumerators;

namespace Controllers.MainMenu
{
    public class CharactersListHandler: Handler<CharactersListPanel>
    {
        private readonly IWindowService _windowService;
        private readonly ICharactersVisualDatabase _charactersVisualDatabase;

        private readonly SelectCharacterView _selectCharacterView;

        public Character SelectedCharacter { get; private set; }

        public CharactersListHandler
        (
            IWindowService windowService,
            ICharactersVisualDatabase charactersVisualDatabase,
            SelectCharacterView selectCharacterView
        )
        {
            _windowService = windowService;
            _charactersVisualDatabase = charactersVisualDatabase;
            _selectCharacterView = selectCharacterView;
        }


        protected override void Initialize()
        {
            Debug.Log("CharactersListHandler initialized");
            View.BackButton.OnClickAsObservable().Subscribe(_ => BackButtonClick()).AddTo(View);
            
            FillCharactersList();
        }

        private void FillCharactersList()
        {
            foreach (var characterData in _charactersVisualDatabase.CharactersDataData.CharactersData)
            {
                var characterElementView = View.CharactersCollection.AddItem();
                characterElementView.SetUp(characterData);
                characterElementView.Button.onClick.AddListener(()=>SelectCharacter(characterData.Character));
            }
        }

        private void SelectCharacter(Character character)
        {
            SelectedCharacter = character;
            
            var characterVisualData = _charactersVisualDatabase.CharactersDataData.CharactersData.Find(
                c=>c.Character == SelectedCharacter);
            
            _selectCharacterView.CharacterPagePanel.SetCharacterImage(characterVisualData.FullImage);
            _selectCharacterView.EnableCharacterPagePanel(true);
            _selectCharacterView.EnableCharactersListPanel(false);
        }
        
        private void BackButtonClick()
        {
            _windowService.Open(EWindow.MainMenu);
        }
    }
}