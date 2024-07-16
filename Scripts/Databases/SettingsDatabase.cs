using Databases.Interfaces;
using Models;
using UnityEngine;

namespace Databases
{
    [CreateAssetMenu(menuName = "Databases/SettingsDatabase", fileName = "SettingsDatabase")]
    public class SettingsDatabase : ScriptableObject, ISettingsDatabase
    {
        [SerializeField] private SettingsVo _settings;

        public SettingsVo Settings => _settings;
    }
}