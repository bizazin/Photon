using DG.Tweening;
using ExitGames.Client.Photon;
using MvcCore.Abstracts;
using Photon.Pun;
using Photon.Realtime;
using PunNetwork.Services.MasterEvent;
using Utils;
using Views;
using Views.PreviewMatchAnimationView;

namespace Controllers
{
    public class PreviewMatchAnimationController : Controller<PreviewMatchAnimationView>
    {
        private readonly IMasterEventService _masterEventService;

        public PreviewMatchAnimationController
        (
            IMasterEventService masterEventService
        )
        {
            _masterEventService = masterEventService;
        }
        
        public void Start()
        {
            View.Reset();
            View.Show();
            
            View.PlayAnimation()
                .AppendInterval(.1f)
                .OnComplete(() => _masterEventService.RaiseEvent(GameEventCodes.StartMatchEventCode));
        }

        public void Hide()
        {
            View.Hide();
        }
    }
}