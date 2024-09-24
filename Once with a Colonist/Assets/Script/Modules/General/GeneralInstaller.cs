using UnityEngine;
using Zenject;
using TendedTarsier.Script.Utilities.Extensions;
using TendedTarsier.Script.Modules.General.Services.Profile;
using TendedTarsier.Script.Modules.Gameplay.Character;
using TendedTarsier.Script.Modules.Gameplay.Configs;
using TendedTarsier.Script.Modules.Gameplay.Configs.Stats;
using TendedTarsier.Script.Modules.Gameplay.Services.Inventory;
using TendedTarsier.Script.Modules.Gameplay.Services.Tilemaps;
using TendedTarsier.Script.Modules.General.Services.Input;
using UnityEngine.EventSystems;
using InventoryProfile = TendedTarsier.Script.Modules.General.Profiles.InventoryProfile;
using StatsProfile = TendedTarsier.Script.Modules.General.Profiles.Stats.StatsProfile;
using TilemapProfile = TendedTarsier.Script.Modules.General.Profiles.TilemapProfile;

namespace TendedTarsier.Script.Modules.General
{
    public class GeneralInstaller : MonoInstaller
    {
        [SerializeField]
        private EventSystem _eventSystem;
        [Header("Configs")]
        [SerializeField]
        private GeneralConfig _generalConfig;

        public override void InstallBindings()
        {
            BindSystem();
            BindConfigs();
            BindProfiles();
            BindServices();
        }

        private void BindSystem()
        {
            Container.Bind<GameplayInput>().FromNew().AsSingle();
            Container.Bind<EventSystem>().FromInstance(_eventSystem).AsSingle();
        }

        private void BindServices()
        {
            Container.BindService<ProfileService>();
            Container.BindService<InputService>();
        }

        private void BindProfiles()
        {
            Container.BindProfile<StatsProfile>();
            Container.BindProfile<TilemapProfile>();
            Container.BindProfile<InventoryProfile>();
        }

        private void BindConfigs()
        {
            Container.Bind<GeneralConfig>().FromInstance(_generalConfig).AsSingle().NonLazy();
        }
    }
}