using UnityEngine;
using Zenject;
using TendedTarsier.Script.Utilities.Extensions;
using TendedTarsier.Script.Modules.General.Services.Profile;
using TendedTarsier.Script.Modules.Gameplay.Character;
using TendedTarsier.Script.Modules.Gameplay.Services.Inventory;
using TendedTarsier.Script.Modules.Gameplay.Services.Tilemaps;
using TendedTarsier.Script.Modules.General.Services.Input;

namespace TendedTarsier.Script.Modules.General
{
    public class GeneralInstaller : MonoInstaller
    {
        [SerializeField]
        private GeneralConfig _generalConfig;

        public override void InstallBindings()
        {
            BindConfigs();
            BindProfiles();
            BindServices();
        }

        private void BindServices()
        {
            Container.Bind<GameplayInput>().FromNew().AsSingle();
            Container.BindWithParents<ProfileService>();
            Container.BindWithParents<InputService>();
        }

        private void BindProfiles()
        {
            Container.BindWithParents<PlayerProfile>();
            Container.BindWithParents<TilemapProfile>();
            Container.BindWithParents<InventoryProfile>();
        }

        private void BindConfigs()
        {
            Container.Bind<GeneralConfig>().FromInstance(_generalConfig).AsSingle();
        }
    }
}