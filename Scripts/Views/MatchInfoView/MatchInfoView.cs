using MvcCore.Abstracts;
using SimpleInputNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Utils.Enumerators;

namespace Views.MatchInfoView
{
    public class MatchInfoView : Window
    {
        [SerializeField] private Button _leaveButton;
        [SerializeField] private TMP_Text _infoText;
        [SerializeField] private Image[] _heartImages;
        [SerializeField] private Joystick _moveJoystick;
        [SerializeField] private Joystick _shootJoystick;

        public override EWindow Name => EWindow.MatchInfo;

        public Button LeaveButton => _leaveButton;

        public void SetWinnerInfo(string winner, int score, float timer)
        {
            _infoText.text = $"Player {winner} won with {score} points.\n\n\nReturning to login screen in {timer:n2} seconds.";
        }

        public void Reset()
        {
            _infoText.text = string.Empty;
            foreach (var heart in _heartImages) 
                heart.enabled = true;
        }
    }
}