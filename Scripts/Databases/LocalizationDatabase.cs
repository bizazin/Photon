using Databases.Interfaces;
using Models;
using UnityEngine;

namespace Databases
{
    [CreateAssetMenu(fileName = "LocalizationDatabase", menuName = "Databases/LocalizationDatabase", order = 2)]
    public class LocalizationDatabase : ScriptableObject, ILocalizationDatabase
    {
        [SerializeField] private LocalizationSettingsVo _localizationSettings;

        public LocalizationSettingsVo Settings => _localizationSettings;
    }
}