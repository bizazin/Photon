using DG.Tweening;
using MvcCore.Abstracts;
using TMPro;
using UnityEngine;
using static Utils.Enumerators;

namespace Views.PreviewMatchAnimationView
{
    public class PreviewMatchAnimationView : Window
    {
        [SerializeField] private TMP_Text _text;
        public override EWindow Name => EWindow.PreviewMatchAnimation;

        private void Awake()
        {
            Reset();
        }

        public Sequence PlayAnimation() =>
            DOTween.Sequence()
                .Append(_text.DOFade(1, .5f))
                .AppendInterval(2)
                .Append(_text.DOFade(0, .5f));

        public void Reset()
        {
            _text.alpha = 0;
        }
    }
}