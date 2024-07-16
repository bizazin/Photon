using DG.Tweening;
using MvcCore.Abstracts;
using TMPro;
using UnityEngine;
using static Utils.Enumerators;


namespace Views.MatchResultView
{
    public class MatchResultsView : Window
    {
        [SerializeField] private TMP_Text _winText;
        [SerializeField] private TMP_Text _loseText;
        [SerializeField] private TMP_Text _drawText;
        public override EWindow Name => EWindow.MatchResults;

        private void Awake()
        {
            Reset();
        }

        public Sequence PlayAnimation(GameResult gameResult)
        {
            var text = gameResult switch
            {
                GameResult.Win => _winText,
                GameResult.Lose => _loseText,
                GameResult.Draw => _drawText,
                _ => null
            };
            
            return DOTween.Sequence()
                .Append(text.DOFade(1, .5f))
                .AppendInterval(2)
                .Append(text.DOFade(0, .5f));
        }

        public void Reset()
        {
            _winText.alpha = 0;
            _loseText.alpha = 0;
            _drawText.alpha = 0;
        }
    }
}