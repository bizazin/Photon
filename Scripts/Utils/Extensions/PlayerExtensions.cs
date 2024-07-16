using System;
using ExitGames.Client.Photon;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using PunNetwork.PhotonTeams;
using UnityEngine;
using static Utils.Enumerators;

namespace Utils.Extensions
{
    public static class PlayerExtensions
    {
        public static bool SetCustomProperty(this Player player, PlayerProperty property, object value)
        {
            var props = new Hashtable
            {
                { property.ToString(), value }
            };
            return player.SetCustomProperties(props);
        }
        
        public static bool TryGetCustomProperty<T>(this Player player, PlayerProperty propertyKey, out T propertyValue)
        {
            var isSuccess = player.CustomProperties.TryGetValue(propertyKey.ToString(), out var value);

            if (isSuccess)
            {
                if (value is T typedValue)
                {
                    propertyValue = typedValue;
                    return true;
                }
                else if (value is string stringValue && typeof(T).IsClass)
                {
                    try
                    {
                        propertyValue = JsonConvert.DeserializeObject<T>(stringValue);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Failed to deserialize property: {ex.Message}");
                    }
                }
            }

            propertyValue = default;
            return false;
        }
        
        public static void ResetCustomProperties(this Player player) => 
            player.SetCustomProperties(new Hashtable());

        public static TeamRole GetTeamRole(this Player player)
        {
            TeamRole teamRole;
            if (player.IsLocal)
                teamRole = TeamRole.MyPlayer;
            else
                teamRole = player.GetPhotonTeam().Code == PhotonNetwork.LocalPlayer.GetPhotonTeam().Code
                    ? TeamRole.AllyPlayer
                    : TeamRole.EnemyPlayer;
            return teamRole;
        }

    }
}