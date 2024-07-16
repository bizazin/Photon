using Databases.Interfaces;
using Models;
using UnityEngine;
using UnityEngine.Serialization;

namespace Databases
{
    [CreateAssetMenu(menuName = "Databases/CharactersVisualDatabase", fileName = "CharactersVisualDatabase")]
    public class CharactersVisualDatabase: ScriptableObject, ICharactersVisualDatabase
    {
        [SerializeField] private CharactersDataVo _charactersData;
        
        public CharactersDataVo CharactersDataData => _charactersData;
    }
}