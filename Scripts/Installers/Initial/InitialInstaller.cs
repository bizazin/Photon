using States.Core;
using Utils.Extensions;
using Zenject;

namespace Installers.Initial
{
    public class InitialInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindServices();
            Container.InjectSceneContainer();
        }

        private void BindServices()
        {
            Container.BindInterfacesTo<InitialEntryPoint>().AsSingle();
        }
    }
}