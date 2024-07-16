using System;
using System.Collections.Generic;
using Installers;
using Models;
using MvcCore.Abstracts;
using MvcCore.Interfaces;
using Photon.PhotonUnityNetworking.Code.Common;
using Services.Window;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Utils.Extensions
{
    public static class BindExtensions
    {
        private static readonly List<WindowBindingInfoVo> WindowsList = new();
        private static IWindowService _windowService;

        public static void BindView<T, TU>(this DiContainer container, Object viewPrefab)
            where TU : IView
            where T : IController
        {
            container.BindInterfacesAndSelfTo<T>().AsSingle();
            var instance = container.InstantiatePrefabForComponent<TU>(viewPrefab);
            container.Bind<TU>().FromInstance(instance).AsSingle();
        }

        public static void BindViewDisabled<T, TU>(this DiContainer container, Object viewPrefab)
            where TU : IView
            where T : IController
        {
            container.BindInterfacesAndSelfTo<T>().AsSingle();
            var instance = container.InstantiatePrefabForComponent<TU>(viewPrefab);
            container.Bind<TU>()
                .FromInstance(instance)
                .AsSingle()
                .OnInstantiated((_, o) => (o as MonoBehaviour)?.gameObject.SetActive(false));
        }

        public static void AddWindowToQueue<T, TU>(this DiContainer container, Object viewPrefab, Transform parent,
            int orderNumber, bool isFocusable = true, bool isDontDestroyOnLoad = false)
            where TU : IWindow where T : IController
        {
            container.BindInterfacesAndSelfTo<T>().AsSingle();
            AddWindowToQueue<TU>(viewPrefab, parent, orderNumber, isFocusable: isFocusable,
                isDontDestroyOnLoad: isDontDestroyOnLoad);
        }

        public static void AddWindowToQueue<T>(Object viewPrefab, Transform parent,
            int orderNumber, bool isFocusable = true, bool isDontDestroyOnLoad = false) where T : IWindow
        {
            var windowInfo = new WindowBindingInfoVo
            {
                Type = typeof(T),
                ViewPrefab = viewPrefab,
                Parent = parent,
                OrderNumber = orderNumber,
                IsFocusable = isFocusable,
                IsDontDestroyOnLoad = isDontDestroyOnLoad
            };
            WindowsList.Add(windowInfo);
        }

        public static void BindWindows(this DiContainer container)
        {
            var windowBindings = new List<WindowBindingInfoVo>(WindowsList);

            var index = 0;
            foreach (var windowBindingInfoVo in windowBindings)
            {
                var binding = container.BindInterfacesAndSelfTo(windowBindingInfoVo.Type)
                    .FromComponentInNewPrefab(windowBindingInfoVo.ViewPrefab)
                    .UnderTransform(windowBindingInfoVo.Parent)
                    .AsSingle();

                binding.OnInstantiated((_, instance) =>
                {
                    index++;
                    var window = instance as Window;
                    if (window == null)
                        throw new Exception(
                            $"[{nameof(BindExtensions)}] Cannot convert {windowBindingInfoVo.ViewPrefab} to window");

                    window.gameObject.SetActive(false);
                    _windowService ??= container.Resolve<IWindowService>();
                    _windowService.RegisterWindow(window, windowBindingInfoVo.IsFocusable,
                        windowBindingInfoVo.OrderNumber, windowBindingInfoVo.IsDontDestroyOnLoad);

                    if (index == windowBindings.Count)
                        container.Resolve<IWindowService>().SortBySiblingIndex();
                });
            }
        }

        public static void BindPrefab<TContent>(this DiContainer container, TContent prefab, Transform parent = null,
            bool isDestroyOnLoad = false)
            where TContent : Object =>
            container.BindInterfacesTo<TContent>()
                .FromComponentInNewPrefab(prefab)
#if UNITY_EDITOR
                .UnderTransform(parent)
#endif
                .AsSingle()
                .OnInstantiated((_, o) =>
                {
                    if (isDestroyOnLoad)
                        Object.DontDestroyOnLoad(o as MonoBehaviour);
                });

        public static void InjectSceneContainer(this DiContainer container)
        {
            var sceneContainerInjectables = ProjectContext.Instance.Container.ResolveAll<ISceneContainerInjectable>();
            foreach (var containerInjectable in sceneContainerInjectables)
                containerInjectable.SetSceneContainer(container);
        }

        public static void ClearWindows()
        {
            WindowsList.Clear();
        }
    }
}