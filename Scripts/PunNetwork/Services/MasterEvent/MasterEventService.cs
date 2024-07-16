using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using States;
using States.Core;
using Utils;
using Zenject;

namespace PunNetwork.Services.MasterEvent
{
    public class MasterEventService : IMasterEventService, IInitializable, IDisposable
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly Dictionary<byte, List<Delegate>> _eventSubscriptions = new();

        private bool _isAllPlayersSpawned;
        private bool _isAllPoolsPrepared;

        public MasterEventService
        (
            IGameStateMachine gameStateMachine
        )
        {
            _gameStateMachine = gameStateMachine;
        }

        public void Initialize()
        {
            PhotonNetwork.NetworkingClient.EventReceived += OnEventReceived;
        }

        public void Dispose()
        {
            PhotonNetwork.NetworkingClient.EventReceived -= OnEventReceived;
        }

        public void Subscribe<T>(byte eventCode, Action<T> handler)
        {
            if (!_eventSubscriptions.ContainsKey(eventCode)) 
                _eventSubscriptions[eventCode] = new List<Delegate>();

            _eventSubscriptions[eventCode].Add(handler);
        }

        public void Subscribe(byte eventCode, Action<object> handler)
        {
            if (!_eventSubscriptions.ContainsKey(eventCode)) 
                _eventSubscriptions[eventCode] = new List<Delegate>();

            _eventSubscriptions[eventCode].Add(handler);
        }

        public void Unsubscribe<T>(byte eventCode, Action<T> handler)
        {
            if (!_eventSubscriptions.TryGetValue(eventCode, out var subscription)) 
                return;
            subscription.Remove(handler);
            if (_eventSubscriptions[eventCode].Count == 0) 
                _eventSubscriptions.Remove(eventCode);
        }

        public void Unsubscribe(byte eventCode, Action<object> handler)
        {
            if (!_eventSubscriptions.TryGetValue(eventCode, out var subscription)) 
                return;
            subscription.Remove(handler);
            if (_eventSubscriptions[eventCode].Count == 0) 
                _eventSubscriptions.Remove(eventCode);
        }

        public void RaiseEvent(byte eventCode, object eventContent = null)
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            var raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            var sendOptions = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(eventCode, eventContent, raiseEventOptions, sendOptions);
        }

        public void Setup()
        {
            _isAllPlayersSpawned = false;
            _isAllPoolsPrepared = false;
        }

        public void OnAllDataGet()
        {
            if (!PhotonNetwork.LocalPlayer.IsMasterClient)
                return;

            PhotonNetwork.LoadLevel(SceneNames.Game);
        }

        public void OnAllPlayersSpawned()
        {
            _isAllPlayersSpawned = true;

            if (_isAllPlayersSpawned && _isAllPoolsPrepared)
                OnSceneReady();
        }

        public void OnAllPoolsPrepared()
        {
            _isAllPoolsPrepared = true;

            if (_isAllPlayersSpawned && _isAllPoolsPrepared)
                OnSceneReady();
        }

        private void OnSceneReady() => _gameStateMachine.Enter<MatchPreviewState>();

        private void OnEventReceived(EventData photonEvent)
        {
            if (!_eventSubscriptions.TryGetValue(photonEvent.Code, out var subscribers)) 
                return;
            foreach (var subscriber in subscribers)
                subscriber.DynamicInvoke(photonEvent.CustomData);
        }
    }
}