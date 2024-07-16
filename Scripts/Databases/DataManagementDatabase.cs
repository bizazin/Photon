using Databases.Interfaces;
using Models;
using UnityEngine;

namespace Databases
{
    [CreateAssetMenu(menuName = "Databases/DataManagementDatabase", fileName = "DataManagementDatabase")]
    public class DataManagementDatabase : ScriptableObject, IDataManagementDatabase
    {
        [SerializeField] private DataManagementVo _settings;
        
        public DataManagementVo Settings => _settings;
    }
}