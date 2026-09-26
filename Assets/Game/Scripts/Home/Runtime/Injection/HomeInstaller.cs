using RollicGames.ColorBlockJamClone.Home.Runtime.Model;
using RollicGames.ColorBlockJamClone.Home.Runtime.View;
using RollicGames.ColorBlockJamClone.Home.Runtime.Controller;
using Zenject;
using UnityEngine;

namespace RollicGames.ColorBlockJamClone.Home.Runtime.Injection
{
    public class HomeInstaller : MonoInstaller
    {
        [SerializeField] private HomeView _homeView;

        public override void InstallBindings()
        {
            Container.Bind<HomeModel>().AsSingle().NonLazy();
            Container.BindInterfacesTo<HomeView>().FromInstance(_homeView).AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<HomeController>().AsSingle().NonLazy();
        }
    }
}