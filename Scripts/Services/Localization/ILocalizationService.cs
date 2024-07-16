using System;
using System.Collections.Generic;
using UnityEngine;
using static Utils.Enumerators;

namespace Services.Localization
{
    public interface ILocalizationService
    {
        event Action LanguageWasChangedEvent;
        Dictionary<SystemLanguage, ELanguage> SupportedLanguages { get; }
        ELanguage CurrentLanguage { get; }
        ELanguage DefaultLanguage { get; }
        void SetLanguage(ELanguage language, bool forceUpdate = false);
        string GetUITranslation(string key);
    }
}