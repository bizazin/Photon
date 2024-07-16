using Models;

namespace Databases.Interfaces
{
    public interface IDataManagementDatabase
    {
        DataManagementVo Settings { get; }
    }
}