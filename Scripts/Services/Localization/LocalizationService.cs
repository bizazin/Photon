using System;
using System.Collections.Generic;
using System.Linq;
using Databases.Interfaces;
using Models;
using Models.Data;
using Services.Data;
using Services.Spreadsheets;
using UnityEngine;
using Utils;
using Zenject;
using static Utils.Enumerators;

namespace Services.Localization
{
    public class LocalizationService : IInitializable, IDisposable, ILocalizationService
    {
        private readonly ILocalizationDatabase _localizationDatabase;
        private readonly IDataService _dataService;
        private readonly ISpreadsheetsService _spreadsheetsService;
        private readonly ISpreadsheetsSettingsDatabase _spreadsheetsSettingsDatabase;
        public event Action LanguageWasChangedEvent;

        private LocalizationLanguageVo _currentLocalizationLanguageVo;

        public Dictionary<SystemLanguage, ELanguage> SupportedLanguages { get; private set; }
        public ELanguage CurrentLanguage { get; private set; }
        public ELanguage DefaultLanguage { get; private set; }

        public LocalizationService
        (
            ILocalizationDatabase localizationDatabase,
            IDataService dataService,
            ISpreadsheetsService spreadsheetsService,
            ISpreadsheetsSettingsDatabase spreadsheetsSettingsDatabase
        )
        {
            _localizationDatabase = localizationDatabase;
            _dataService = dataService;
            _spreadsheetsService = spreadsheetsService;
            _spreadsheetsSettingsDatabase = spreadsheetsSettingsDatabase;
        }

        public void Initialize()
        {
            DefaultLanguage = _localizationDatabase.Settings.DefaultLanguage;
            CurrentLanguage = ELanguage.Unknown;

            if (_dataService.DataIsLoaded)
                OnDataLoaded();
            else
                _dataService.DataLoadedEvent += OnDataLoaded;
        }

        public void Dispose()
        {
            _dataService.DataLoadedEvent -= OnDataLoaded;
        }

        public void SetLanguage(ELanguage language, bool forceUpdate = false)
        {
            if (language == CurrentLanguage && !forceUpdate)
                return;

            if (SupportedLanguages.ContainsValue(language))
            {
                CurrentLanguage = language;
                _dataService.CachedUserLocalData.AppLanguage = language;
                _currentLocalizationLanguageVo =
                    _localizationDatabase.Settings.Languages.Find(item => item.Language == CurrentLanguage);
            }

            LanguageWasChangedEvent?.Invoke();
        }

        public string GetUITranslation(string key)
        {
            if (_currentLocalizationLanguageVo == null)
                throw new Exception($"[{nameof(LocalizationService)}] There is no current localization language.");

            var localizedText = _currentLocalizationLanguageVo.LocalizedTexts.Find(item =>
                CsvUtils.ReplaceLineBreaks(item.Key) == CsvUtils.ReplaceLineBreaks(key));

            if (localizedText == null)
                throw new Exception(
                    $"[{nameof(LocalizationService)}] Translation with key {key} was not present in the current localization language.");

            return localizedText.Value;
        }

        private void OnDataLoaded()
        {
            if (_localizationDatabase.Settings.RefreshLocalizationAtStart && _spreadsheetsSettingsDatabase.Settings.RefreshSpreadsheets)
                RefreshLocalizations();
            FillLanguages();
            ApplyLocalization();
        }

        private void RefreshLocalizations()
        {
            var spreadsheet = _spreadsheetsService.GetSpreadsheetByType(ESpreadsheetDataType.Localization);

            if (spreadsheet == null)
                throw new Exception(
                    $"[{nameof(LocalizationService)}] Failed to refresh localization. Spreadsheet is null.");

            if (!spreadsheet.IsLoaded)
                throw new Exception(
                    $"[{nameof(LocalizationService)}] Failed to refresh localization. Spreadsheet {spreadsheet} is not loaded.");

            var localizationSheetData = spreadsheet.GetObject<LocalizationSheetData>();

            _localizationDatabase.Settings.Languages = new List<LocalizationLanguageVo>();

            for (var i = 1; i < Enum.GetNames(typeof(ELanguage)).Length; i++)
            {
                var languageData = new LocalizationLanguageVo
                {
                    Language = (ELanguage)i,
                    LocalizedTexts = new List<FieldInfoVo>()
                };

                foreach (var element in localizationSheetData)
                {
                    var dataInfo = new FieldInfoVo
                    {
                        Key = element.Key
                    };

                    switch (languageData.Language)
                    {
                        case ELanguage.Unknown:
                        case ELanguage.English:
                        default:
                            dataInfo.Value = element.English;
                            break;
                        case ELanguage.Bulgarian:
                            dataInfo.Value = element.Bulgarian;
                            break;
                        case ELanguage.Hungarian:
                            dataInfo.Value = element.Hungarian;
                            break;
                        case ELanguage.Greek:
                            dataInfo.Value = element.Greek;
                            break;
                        case ELanguage.Danish:
                            dataInfo.Value = element.Danish;
                            break;
                        case ELanguage.Indonesian:
                            dataInfo.Value = element.Indonesian;
                            break;
                        case ELanguage.Spanish:
                            dataInfo.Value = element.Spanish;
                            break;
                        case ELanguage.Italian:
                            dataInfo.Value = element.Italian;
                            break;
                        case ELanguage.Chinese:
                            dataInfo.Value = element.Chinese;
                            break;
                        case ELanguage.Korean:
                            dataInfo.Value = element.Korean;
                            break;
                        case ELanguage.German:
                            dataInfo.Value = element.German;
                            break;
                        case ELanguage.Dutch:
                            dataInfo.Value = element.Dutch;
                            break;
                        case ELanguage.Polish:
                            dataInfo.Value = element.Polish;
                            break;
                        case ELanguage.Portuguese:
                            dataInfo.Value = element.Portuguese;
                            break;
                        case ELanguage.Romanian:
                            dataInfo.Value = element.Romanian;
                            break;
                        case ELanguage.Russian:
                            dataInfo.Value = element.Russian;
                            break;
                        case ELanguage.Turkish:
                            dataInfo.Value = element.Turkish;
                            break;
                        case ELanguage.Ukrainian:
                            dataInfo.Value = element.Ukrainian;
                            break;
                        case ELanguage.Finnish:
                            dataInfo.Value = element.Finnish;
                            break;
                        case ELanguage.French:
                            dataInfo.Value = element.French;
                            break;
                        case ELanguage.Czech:
                            dataInfo.Value = element.Czech;
                            break;
                        case ELanguage.Swedish:
                            dataInfo.Value = element.Swedish;
                            break;
                        case ELanguage.Estonian:
                            dataInfo.Value = element.Estonian;
                            break;
                        case ELanguage.Japanese:
                            dataInfo.Value = element.Japanese;
                            break;
                    }

                    languageData.LocalizedTexts.Add(dataInfo);
                }

                _localizationDatabase.Settings.Languages.Add(languageData);
            }
        }

        private void FillLanguages()
        {
            SupportedLanguages = new Dictionary<SystemLanguage, ELanguage>();

            var supportedLanguages = _localizationDatabase.Settings.Languages.Select(item => item.Language).ToArray();

            foreach (var item in supportedLanguages)
            {
                if (Enum.TryParse(item.ToString(), out SystemLanguage result))
                    SupportedLanguages.Add(result, item);
                else
                    throw new Exception(
                        $"[{nameof(LocalizationService)}] Cannot parse unsupported localization language: {item}.");
            }
        }

        private void ApplyLocalization()
        {
            var languageToSet = _dataService.CachedUserLocalData.AppLanguage;

            if (languageToSet == ELanguage.Unknown)
                languageToSet = SupportedLanguages.GetValueOrDefault(Application.systemLanguage, DefaultLanguage);

            SetLanguage(languageToSet, true);
        }
    }
}