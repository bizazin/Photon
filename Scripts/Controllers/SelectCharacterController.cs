using Controllers.MainMenu;
using MvcCore.Abstracts;
using Services.Data;
using Views;
using Views.SelectCharacterView;

namespace Controllers
{
    public class SelectCharacterController: Controller<SelectCharacterView>
    {
        private readonly IDataService _dataService;
        private readonly CharacterPageHandler _characterPageHandler;
        private readonly CharactersListHandler _charactersListHandler;

        public SelectCharacterController
        (
            IDataService dataService,
            CharacterPageHandler characterPageHandler,
            CharactersListHandler charactersListHandler
        )
        {
            _dataService = dataService;
            _characterPageHandler = characterPageHandler;
            _charactersListHandler = charactersListHandler;
        }
        
        public override void Initialize()
        {
            _dataService.DataLoadedEvent += DataLoadedHandler;
            
            _characterPageHandler.Setup(View.CharacterPagePanel);
            _charactersListHandler.Setup(View.CharactersListPanel);
        }

        private void DataLoadedHandler()
        {
            
        }

        public void Setup()
        {
            View.Show();
        }
    }
}