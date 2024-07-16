using System;
using UnityEngine.Serialization;

namespace PunNetwork.NetworkData
{
    public class NetworkDataModel
    {
        [Serializable]
        public class PlayerImmutableDataVo
        {
            public string Nickname;
            public string CharacterName;
            public int AvatarID;
            public StatsValuesVo InitialStats;
        }
        
        [Serializable]
        public class StatsValuesVo
        {
            public float HealthPoints;
            public bool IsDead;
        }
    }

 
}