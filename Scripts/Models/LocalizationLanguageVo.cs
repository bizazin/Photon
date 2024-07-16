using System;
using System.Collections.Generic;
using Utils;

namespace Models
{
    [Serializable]
    public class LocalizationLanguageVo
    {
        public Enumerators.ELanguage Language;
        public List<FieldInfoVo> LocalizedTexts;
    }
}