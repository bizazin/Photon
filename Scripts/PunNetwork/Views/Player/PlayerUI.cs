using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PunNetwork.Views.Player
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nicknameText;
        [SerializeField] private Image _fillHealthImage;

        private const float HealthChangeDuration = 1f;

        private void Awake()
        {
            _fillHealthImage.fillAmount = 0;
        }

        public void SetNickName(string value) => _nicknameText.text = value;

        public void SetHealthPoints(float currentHealthPoints, float maxHealthPoints)
        {
            if (maxHealthPoints <= 0)
            {
                Debug.LogError($"[{nameof(PlayerUI)}] Parameter {nameof(maxHealthPoints)} must be positive");
                return;
            }

            var targetFillAmount = currentHealthPoints / maxHealthPoints;
            _fillHealthImage.DOFillAmount(targetFillAmount, HealthChangeDuration);
        }
    }
}