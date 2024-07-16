using Behaviours;
using Controllers;
using Databases;
using Databases.Interfaces;
using Helpers;
using Photon.PhotonUnityNetworking.Code.Common.PrefabRegistry;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using PunNetwork.PhotonTeams;
using PunNetwork.Services.CustomProperties;
using PunNetwork.Services.ProjectNetwork;
using Services.Window;
using UnityEngine;
using UnityEngine.Serialization;
using Utils.Extensions;
using Views;
using Views.LoadingView;
using Zenject;

namespace Installers.Project
{
    [CreateAssetMenu(menuName = "Installers/ProjectPrefabInstaller", fileName = "ProjectPrefabInstaller")]
    public class ProjectPrefabInstaller : ScriptableObjectInstaller
    {
        [Header("Databases")] 
        [SerializeField] private LocalizationDatabase _localizationDatabase;
        [SerializeField] private SpreadsheetsSettingsDatabase _spreadsheetsSettingsDatabase;
        [SerializeField] private SoundsDatabase _soundsDatabase;
        [SerializeField] private SettingsDatabase _settingsDatabase;
        [SerializeField] private DataManagementDatabase _dataManagementDatabase;
        [SerializeField] private CharactersVisualDatabase _charactersVisualDatabase;
        [SerializeField] private PrefabRegistryDatabase _prefabRegistryDatabase;

        [Header("Common")] 
        [SerializeField] private CoroutineRunner _coroutineRunner;
        [SerializeField] private AudioMixerProvider _audioMixerProvider;
        [SerializeField] private GameRestarter _gameRestarter;

        [Header("Photon")]
        [SerializeField] private PhotonTeamsManager _photonTeamsManager;
        [SerializeField] private CustomPropertiesService _customPropertiesService;
        
        [Header("Canvas")] 
        [SerializeField] private Canvas _canvas;
        
        [Header("Windows")]
        [SerializeField] private LoadingView _loadingView;

        [Header("Network")]
        [SerializeField] private ProjectNetworkService _projectNetworkService;


        public override void InstallBindings()
        {
            BindDatabases();
            BindPrefabs();
            BindWindows();
        }

        private void BindWindows()
        {
            //Container.Resolve<IWindowService>().ClearWindows();
            var parent = Instantiate(_canvas).transform;
            
            DontDestroyOnLoad(parent.gameObject);

            var canvasComponent = parent.GetComponent<Canvas>();
            if (canvasComponent != null) 
                canvasComponent.sortingOrder = 1000; 
            
            Container.AddWindowToQueue<LoadingController, LoadingView>(_loadingView, parent, 0, isDontDestroyOnLoad:true);
            
            Container.BindWindows();

        }

        private void BindPrefabs()  
        {
            Container.BindPrefab(_coroutineRunner, isDestroyOnLoad:true);
            Container.BindPrefab(_audioMixerProvider, isDestroyOnLoad:true);
            Container.BindPrefab(_gameRestarter, isDestroyOnLoad:true);
            Container.BindPrefab(_photonTeamsManager, isDestroyOnLoad:true);
            Container.BindPrefab(_customPropertiesService, isDestroyOnLoad:true);
            Container.BindPrefab(_projectNetworkService, isDestroyOnLoad:true);
        }

        private void BindDatabases()
        {
            Container.Bind<ILocalizationDatabase>().FromInstance(_localizationDatabase).AsSingle();
            Container.Bind<ISpreadsheetsSettingsDatabase>().FromInstance(_spreadsheetsSettingsDatabase).AsSingle();
            Container.Bind<ISoundsDatabase>().FromInstance(_soundsDatabase).AsSingle();
            Container.Bind<ISettingsDatabase>().FromInstance(_settingsDatabase).AsSingle();
            Container.Bind<IDataManagementDatabase>().FromInstance(_dataManagementDatabase).AsSingle();
            Container.Bind<ICharactersVisualDatabase>().FromInstance(_charactersVisualDatabase).AsSingle();
            Container.Bind<IPrefabRegistryDatabase>().FromInstance(_prefabRegistryDatabase).AsSingle();
        }
    }
}