using System.Collections.Generic;
using System.Threading.Tasks;
using Services.Network;
using UnityEngine;
using Utils;

namespace Models.Data
{
    public class SpreadsheetInfo
    {
        private readonly INetworkService _networkService;
        private readonly string _spreadsheetId;
        private readonly string _gid = "0";
        private readonly string _format = "csv";

        private string _data;

        public bool IsLoaded { get; private set; }

        public SpreadsheetInfo
        (
            INetworkService networkService,
            string spreadsheetId
        )
        {
            _networkService = networkService;
            _spreadsheetId = spreadsheetId;
        }

        public SpreadsheetInfo
        (
            INetworkService networkService,
            string spreadsheetId,
            string gid
        ) : this(networkService, spreadsheetId)
        {
            _gid = gid;
        }

        public SpreadsheetInfo
        (
            INetworkService networkService,
            string spreadsheetId,
            string gid,
            string format
        ) : this(networkService, spreadsheetId, gid)
        {
            _format = format;
        }

        public async Task LoadData()
        {
            _data = await _networkService.GetRequest(
                $"https://docs.google.com/spreadsheets/export?id={_spreadsheetId}&exportFormat={_format}&gid={_gid}");
            IsLoaded = _data != null;

#if UNITY_EDITOR
            if (IsLoaded)
                Debug.Log($"Loaded spreadsheet: {_spreadsheetId}:{_gid}");
#endif
        }

        public List<T> GetObject<T>() => !IsLoaded ? null : CsvUtils.ParseCsv<T>(_data);

        public void Dispose()
        {
            _data = null;
        }
    }
}