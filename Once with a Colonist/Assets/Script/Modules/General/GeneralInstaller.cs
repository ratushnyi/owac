using UnityEngine;
using Zenject;
using UnityEngine.EventSystems;
using TendedTarsier.Script.Utilities.Extensions;
using TendedTarsier.Script.Modules.General.Profiles.Stats;
using TendedTarsier.Script.Modules.General.Profiles.Tilemap;
using TendedTarsier.Script.Modules.General.Profiles.Inventory;
using TendedTarsier.Script.Modules.General.Services.Profile;
using TendedTarsier.Script.Modules.General.Services.Input;

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
            Container.BindProfile<MapProfile>();
            Container.BindProfile<InventoryProfile>();
        }

        private void BindConfigs()
        {
            Container.Bind<GeneralConfig>().FromInstance(_generalConfig).AsSingle().NonLazy();
        }
    }
}