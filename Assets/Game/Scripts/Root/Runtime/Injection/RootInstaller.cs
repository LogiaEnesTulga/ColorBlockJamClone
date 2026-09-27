using RollicGames.AddressableLoading.Runtime.View;
using RollicGames.ColorBlockJamClone.Player.Runtime.Controller;
using RollicGames.ColorBlockJamClone.Player.Runtime.Model;
using RollicGames.ColorBlockJamClone.Root.Runtime.Controller;
using RollicGames.ColorBlockJamClone.SceneLoad.Runtime.Presenter;
using RollicGames.Persistence.Runtime.Presenter;
using Zenject;

namespace RollicGames.ColorBlockJamClone.Root.Runtime.Injection
{
    public class RootInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<BinaryPlayerPrefsPersistence>().AsSingle();

            Container.Bind<PlayerModel>().AsSingle();
            Container.BindInterfacesTo<PlayerController>().AsSingle();

            Container.BindInterfacesTo<SceneLoadPresenter>().AsSingle();
            Container.BindInterfacesTo<AddressableLoader>().AsSingle();

            Container.BindInterfacesTo<RootController>().AsSingle();
        }
    }
}
