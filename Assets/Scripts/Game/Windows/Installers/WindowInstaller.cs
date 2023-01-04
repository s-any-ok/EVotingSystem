using Game.Windows.Controllers;
using Game.Windows.Controllers.Factory;
using Game.Windows.Views;
using UnityEngine;
using Zenject;

namespace Game.Windows.Installers
{
    public class WindowInstaller : MonoInstaller
    {
        [SerializeField] private WindowView _windowView;
        [SerializeField] private CardView _cardView;
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<WindowController>().AsSingle().WithArguments(_windowView);
            Container.BindFactory<CardView, CardFactory>().FromComponentInNewPrefab(_cardView);
        }
    }
}