using Databases.Interfaces;
using Models;
using UnityEngine;
using Utils;

namespace Databases
{
    public abstract class SpreadsheetDatabase : ScriptableObject, ISpreadsheetDatabase
    {
        [SerializeField] private bool _refreshAtStart = true;
        public bool RefreshAtStart => _refreshAtStart;
        public abstract Enumerators.ESpreadsheetDataType Name { get; }
        public abstract SpreadsheetDataVo Data { get; }
    }
}