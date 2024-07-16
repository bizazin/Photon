using System;
using System.Collections.Generic;
using Utils;

namespace Models
{
    [Serializable]
    public class LocalizationSettingsVo
    {
        public List<LocalizationLanguageVo> Languages;
        public Enumerators.ELanguage DefaultLanguage;
        public bool RefreshLocalizationAtStart = true;
    }
}