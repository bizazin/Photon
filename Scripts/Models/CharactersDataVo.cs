using System;
using System.Collections.Generic;
using Photon.PhotonUnityNetworking.Code.Common;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;
using Enumerators = Utils.Enumerators;

namespace Models
{
    [Serializable]
    public class CharactersDataVo
    {
        public List<CharacterData> CharactersData;
    }

    [Serializable]
    public class CharacterData
    {
        public Enumerators.Character Character;
        [FormerlySerializedAs("AvatarImage")] public Sprite AvatarSprite;
        public Sprite FullImage;

    }
}