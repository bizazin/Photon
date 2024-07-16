using Services.Data;
using Services.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Behaviours
{
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField] private string _localizationKey;

        private Text _uiText;
        private TMP_Text _tmpText;
        private ILocalizationService _localizationService;
        private IDataService _dataService;

        public string LocalizationKey
        {
            set
            {
                _localizationKey = value;
                if (_dataService.DataIsLoaded)
                    UpdateText();
                _localizationService.LanguageWasChangedEvent += UpdateText;
            }
        }

        [Inject]
        private void Construct
        (
            ILocalizationService localizationService,
            IDataService dataService
        )
        {
            _localizationService = localizationService;
            _dataService = dataService;
        }

        private void Awake()
        {
            _uiText = GetComponent<Text>();
            _tmpText = GetComponent<TMP_Text>();
        }

        private void Start()
        {
            if (string.IsNullOrEmpty(_localizationKey))
                return;
            
            if (_dataService.DataIsLoaded)
                UpdateText();
            _localizationService.LanguageWasChangedEvent += UpdateText;
        }

        private void OnDestroy()
        {
            if (string.IsNullOrEmpty(_localizationKey))
                return;
            
            _localizationService.LanguageWasChangedEvent -= UpdateText;
        }

        private void UpdateText()
        {
            if (string.IsNullOrEmpty(_localizationKey)) return;
            var localizedContent = _localizationService.GetUITranslation(_localizationKey);

            if (_uiText)
                _uiText.text = localizedContent;

            if (_tmpText)
                _tmpText.text = localizedContent;
        }
    }
}