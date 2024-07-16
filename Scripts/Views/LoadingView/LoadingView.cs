using DG.Tweening;
using MvcCore.Abstracts;
using UnityEngine;
using UnityEngine.UI;
using static Utils.Enumerators;

namespace Views.LoadingView
{
    public class LoadingView : Window
    {
        [SerializeField] private Image _loadingImage;
        public override EWindow Name => EWindow.Loading;

        private void Awake()
        {
            _loadingImage.enabled = false;
        }

        public Tween StartLoadingAnimation()
        {
            _loadingImage.enabled = true;
            return _loadingImage.transform.DORotate(new Vector3(0, 0, 360), 1, RotateMode.LocalAxisAdd)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
        }

    }
}