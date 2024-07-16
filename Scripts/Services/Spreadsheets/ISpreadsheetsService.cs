using System.Threading.Tasks;
using Models.Data;
using static Utils.Enumerators;

namespace Services.Spreadsheets
{
    public interface ISpreadsheetsService       
    {
        void FillSpreadsheetsInfo();
        Task StartLoadSpreadsheetsData();
        SpreadsheetInfo GetSpreadsheetByType(ESpreadsheetDataType type);
        void SetupDatabases();
    }
}