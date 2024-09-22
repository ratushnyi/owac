using UnityEngine;
using Zenject;

namespace TendedTarsier.Script.Modules.Menu
{
    public class MenuInstaller : MonoInstaller
    {
        [SerializeField]
        private MenuConfig _menuConfig;

        public override void InstallBindings()
        {
            Container.Bind<MenuConfig>().FromInstance(_menuConfig).AsSingle();
        }
    }
}