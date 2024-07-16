using Utils;
using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using static Utils.Enumerators;

namespace PunNetwork.Services.CustomProperties
{
    public class CustomPropertiesService : MonoBehaviourPunCallbacks, ICustomPropertiesService
    {
        private readonly Dictionary<PlayerProperty, List<Delegate>> _eventSubscriptions = new();

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            foreach (var entry in changedProps)
            {
                if (!Enum.TryParse(entry.Key.ToString(), out PlayerProperty propKey)) continue;
                var propValue = entry.Value;
                Logger.Log(this, $"OnPlayer {targetPlayer.ActorNumber}, Property {propKey} was updated to {propValue}");
                HandlePropertyChange(propKey, propValue, targetPlayer);
            }
        }

        public void Subscribe<T>(PlayerProperty property, Action<Player, T> handler)
        {
            if (!_eventSubscriptions.ContainsKey(property))
                _eventSubscriptions[property] = new List<Delegate>();

            _eventSubscriptions[property].Add(handler);
        }

        public void Unsubscribe<T>(PlayerProperty property, Action<Player, T> handler)
        {
            if (!_eventSubscriptions.TryGetValue(property, out var subscription)) 
                return;
            subscription.Remove(handler);
            if (_eventSubscriptions[property].Count == 0) 
                _eventSubscriptions.Remove(property);
        }

        private void HandlePropertyChange(PlayerProperty key, object value, Player player)
        {
            if (!_eventSubscriptions.TryGetValue(key, out var subscribers)) 
                return;
            foreach (var subscriber in subscribers) 
                subscriber.DynamicInvoke(player, value);
        }
    }
}