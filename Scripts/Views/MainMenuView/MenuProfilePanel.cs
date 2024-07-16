using MvcCore.Abstracts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Views.MainMenuView
{
    public class MenuProfilePanel: View
    {
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private Image _profileImage;
        
        public TMP_InputField InputField => _inputField;
        public Image ProfileImage => _profileImage;

    }
}