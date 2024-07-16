using MvcCore.Abstracts;
using Services.Data;
using UnityEngine;
using Views.MainMenuView;

namespace Controllers.MainMenu
{
    public class MenuProfileHandler: Handler<MenuProfilePanel>
    {
        private readonly IDataService _dataService;

        public MenuProfileHandler
        (
            IDataService dataService
        )
        {
            _dataService = dataService;
        }

        protected override void Initialize()
        {
            Debug.Log("Profile initialized");
            
            View.InputField.onEndEdit.AddListener(ProfileNameChanged);
            LoadNickName();
        }

        private void ProfileNameChanged(string value)
        {
            View.InputField.text = value;
            _dataService.CachedUserLocalData.NickName = value;
        }

        private void LoadNickName()
        {
            View.InputField.text = _dataService.CachedUserLocalData.NickName;
        }
    }
}