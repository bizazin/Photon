using System.Collections.Generic;
using MvcCore.Interfaces;
using Photon.PhotonUnityNetworking.Code.Common.Factory;
using UnityEngine;
using Zenject;

namespace MvcCore.Abstracts
{
    public abstract class UiCollection<TView> : MonoBehaviour, IUiCollection where TView : View
    {
        [SerializeField] private Transform _collectionRoot;
        
        private readonly List<TView> _items = new();
        private IGameFactory _factory;
        private TView _view;

        [Inject]
        private void Construct
        (
            IGameFactory factory,
            TView view
        )
        {
            _factory = factory;
            _view = view;
        }
        
        public TView AddItem()
        {
            var item = _factory.Create(_view, Vector3.zero, Quaternion.identity);
            item.transform.SetParent(_collectionRoot, false);
            item.Show();
            _items.Add(item);
            return item;
        }

        public List<TView> GetItems() => _items;

        public void RemoveItem(TView view)
        {
            _items.Remove(view);
            DestroyImmediate(view.gameObject);
        }

        public void Clear()
        {
            foreach (var item in _items)
                Destroy(item.gameObject);
            _items.Clear();
        }

        public int Count() => _items.Count;
    }
}