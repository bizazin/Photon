using Models;
using Utils;

namespace Databases.Interfaces
{
    public interface ISpreadsheetDatabase
    {
        public bool RefreshAtStart { get; }
        public Enumerators.ESpreadsheetDataType Name { get; }
        public SpreadsheetDataVo Data { get; }

    }
}