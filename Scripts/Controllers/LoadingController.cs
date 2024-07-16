using DG.Tweening;
using MvcCore.Abstracts;
using Views;
using Views.LoadingView;

namespace Controllers
{
    public class LoadingController : Controller<LoadingView>
    {
        private Tween _loadingTween;
        
        public void Show()
        {
            View.Show();
            _loadingTween = View.StartLoadingAnimation();
        }
        
        public void Hide()
        {
            _loadingTween.Kill();
            _loadingTween = null;
            View.Hide();
        }
    }
}