using Databases.Interfaces;
using Models;
using UnityEngine;

namespace Databases
{
    [CreateAssetMenu(menuName = "Databases/SpreadsheetsSettingsDatabase", fileName = "SpreadsheetsSettingsDatabase")]
    public class SpreadsheetsSettingsDatabase : ScriptableObject, ISpreadsheetsSettingsDatabase
    {
        [SerializeField] private SpreadsheetsSettingsVo _spreadsheetsSettings;

        public SpreadsheetsSettingsVo Settings => _spreadsheetsSettings;
    }
}