using Services.Data;
using Services.Localization;
using TMPro;
using UnityEngine;
using Zenject;

namespace Behaviours
{
    [RequireComponent(typeof(TMP_Dropdown))]
    public class LocalizedDropdown : MonoBehaviour
    {
        [SerializeField] private string[] _localizationKeys;
        [SerializeField] private TMP_Dropdown _dropdown;

        private ILocalizationService _localizationService;
        private IDataService _dataService;

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

        private void OnEnable()
        {
            _dropdown.options.Clear();
            for (var i = 0; i < _localizationKeys.Length; i++) 
                _dropdown.options.Add(new TMP_Dropdown.OptionData());
            
            if (_dataService.DataIsLoaded)
                UpdateOptions();
            _localizationService.LanguageWasChangedEvent += UpdateOptions;
        }

        private void OnDisable()
        {
            _localizationService.LanguageWasChangedEvent -= UpdateOptions;
        }

        private void UpdateOptions()
        {
            for (var i = 0; i < _dropdown.options.Count; i++)
                if (!string.IsNullOrEmpty(_localizationKeys[i]))
                    _dropdown.options[i].text = _localizationService.GetUITranslation(_localizationKeys[i]);

            _dropdown.RefreshShownValue();
        }

    }
}