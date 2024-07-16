using Controllers;
using Models;
using PunNetwork.Services.GameNetwork;
using Services.Window;
using UnityEngine;
using Utils.Extensions;
using Views;
using Views.MatchInfoView;
using Views.MatchResultView;
using Views.PreviewMatchAnimationView;
using Zenject;

namespace Installers.Game
{
    
    [CreateAssetMenu(menuName = "Installers/GamePrefabInstaller", fileName = "GamePrefabInstaller")]
    public class GamePrefabInstaller : ScriptableObjectInstaller
    {
        [Header("Canvas")] 
        [SerializeField] private Canvas _canvas;

        [Header("Windows")] 
        [SerializeField] private MatchInfoView _matchInfoView;
        [SerializeField] private PreviewMatchAnimationView _previewMatchAnimationView;
        [SerializeField] private MatchResultsView _matchResultsView;

        [Header("Network")] 
        [SerializeField] private GameNetworkService _gameNetworkService;
        
        public override void InstallBindings()
        {
            BindWindows();
            BindPrefabs();
        }

        private void BindPrefabs()
        {
            Container.BindPrefab(_gameNetworkService);
        }
        
        private void BindWindows()
        {
            Container.Resolve<IWindowService>().ClearWindows();
            
            var canvas = Container.InstantiatePrefabForComponent<Canvas>(_canvas);
            Container.Bind<Canvas>().FromInstance(canvas).AsSingle();
            
            var parent = canvas.transform;
            
            Container.AddWindowToQueue<MatchInfoController, MatchInfoView>(_matchInfoView, parent, 0);
            Container.AddWindowToQueue<PreviewMatchAnimationController, PreviewMatchAnimationView>(_previewMatchAnimationView, parent, 1);
            Container.AddWindowToQueue<MatchResultsController, MatchResultsView>(_matchResultsView, parent, 2);

            Container.BindWindows();
        }
    }
}