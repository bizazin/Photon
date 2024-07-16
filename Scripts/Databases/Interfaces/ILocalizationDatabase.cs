using Models;

namespace Databases.Interfaces
{
    public interface ILocalizationDatabase
    {
        LocalizationSettingsVo Settings { get; }
    }
}