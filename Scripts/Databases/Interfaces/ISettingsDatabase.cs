using Models;

namespace Databases.Interfaces
{
    public interface ISettingsDatabase
    {
        SettingsVo Settings { get; }
    }
}