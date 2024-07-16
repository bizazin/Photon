using Controllers;
using Controllers.MainMenu;
using Photon.Pun.UtilityScripts;
using PunNetwork.Services.MenuNetwork;
using Services.Window;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;
using Utils.Extensions;
using Views;
using Views.MainMenuView;
using Views.SelectCharacterView;
using Zenject;

namespace Installers.Menu
{ 
    [CreateAssetMenu(menuName = "Installers/MenuPrefabInstaller", fileName = "MenuPrefabInstaller")]
    public class MenuPrefabInstaller : ScriptableObjectInstaller
    {
        [Header("Canvas")]
        [SerializeField] private Canvas _canvas;
        
        [Header("Windows")]
        [SerializeField] private MainMenuView _mainMenuView;
        [SerializeField] private SelectCharacterView _selectCharacterView;

        [Header("Network")]
        [SerializeField] private MenuNetworkService _menuNetworkService;

        [Header("Prefabs")]
        [SerializeField] private CharacterElementView _characterElementView;
        
        public override void InstallBindings()
        {
            BindWindows();
            BindPrefabs();
        }

        private void BindPrefabs()
        {
            Container.BindPrefab(_menuNetworkService);
            
            Container.BindInstances(_characterElementView);
        }

        private void BindWindows()
        {
            Container.Resolve<IWindowService>().ClearWindows();
            var parent = Instantiate(_canvas).transform;
            
            Container.AddWindowToQueue<MainMenuController, MainMenuView>(_mainMenuView, parent, 0);
            Container.AddWindowToQueue<SelectCharacterController, SelectCharacterView>(_selectCharacterView, parent, 0, true);
            
            Container.BindWindows();
        }
    }
}