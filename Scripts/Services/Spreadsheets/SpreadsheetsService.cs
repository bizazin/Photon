using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Databases;
using Databases.Interfaces;
using Models;
using Models.Data;
using Services.Network;
using static Utils.Enumerators;

namespace Services.Spreadsheets
{
    public class SpreadsheetsService : ISpreadsheetsService, IDisposable
    {
        private readonly ISpreadsheetsSettingsDatabase _spreadsheetsSettingsDatabase;
        private readonly ILocalizationDatabase _localizationDatabase;
        private readonly INetworkService _networkService;
        private readonly IEnumerable<ISpreadsheetDatabase> _spreadsheetDatabases;
        private Dictionary<ESpreadsheetDataType, SpreadsheetInfo> _spreadsheetsInfo = new();

        public SpreadsheetsService
        (
            ISpreadsheetsSettingsDatabase spreadsheetsSettingsDatabase,
            ILocalizationDatabase localizationDatabase,
            INetworkService networkService,
            IEnumerable<ISpreadsheetDatabase> spreadsheetDatabases
        )
        {
            _spreadsheetsSettingsDatabase = spreadsheetsSettingsDatabase;
            _localizationDatabase = localizationDatabase;
            _networkService = networkService;
            _spreadsheetDatabases = spreadsheetDatabases;
        }

        public void Dispose()
        {
            foreach (var spreadsheet in _spreadsheetsInfo.Values) 
                spreadsheet.Dispose();
        }
        
        public void FillSpreadsheetsInfo()
        {
            _spreadsheetsInfo = new Dictionary<ESpreadsheetDataType, SpreadsheetInfo>();

            if (_localizationDatabase.Settings.RefreshLocalizationAtStart)
                _spreadsheetsInfo.Add(ESpreadsheetDataType.Localization,
                    new SpreadsheetInfo(_networkService, _spreadsheetsSettingsDatabase.Settings.GoogleSpreadsheet));
            foreach (var database in _spreadsheetDatabases.Where(database => database.RefreshAtStart))
                _spreadsheetsInfo.Add(database.Name,
                    new SpreadsheetInfo(_networkService, _spreadsheetsSettingsDatabase.Settings.GoogleSpreadsheet,
                        database.Data.Gid));
        }

        public async Task StartLoadSpreadsheetsData()
        {
            foreach (var item in _spreadsheetsInfo) 
                await item.Value.LoadData();
        }

        public SpreadsheetInfo GetSpreadsheetByType(ESpreadsheetDataType type) => _spreadsheetsInfo[type];
        
        public void SetupDatabases()
        {
            foreach (var database in _spreadsheetDatabases)
            {
                if (!database.RefreshAtStart)
                    continue;
                var spreadsheet = _spreadsheetsInfo[database.Name];
                if (spreadsheet == null)
                    throw new Exception($"[{nameof(SpreadsheetsService)}] Failed to setup spreadsheet. Spreadsheet is null.");
                
                if (!spreadsheet.IsLoaded)
                    throw new Exception($"[{nameof(SpreadsheetsService)}] Failed to setup spreadsheet. Spreadsheet {spreadsheet} is not loaded.");

                var fieldInfoVos = spreadsheet.GetObject<FieldInfoVo>();

                foreach (var fieldInfoVo in fieldInfoVos)
                {
                    var childType = database.Data.GetType();
                    var field = childType.GetField(fieldInfoVo.Key, BindingFlags.Public | BindingFlags.Instance);
                    if (field == null)
                        throw new Exception($"Field '{fieldInfoVo.Key}' not found on type '{childType}'.");

                    var convertedValue = Convert.ChangeType(fieldInfoVo.Value, field.FieldType, CultureInfo.InvariantCulture);

                    field.SetValue(database.Data, convertedValue);
                }
            }
        }
    }
}