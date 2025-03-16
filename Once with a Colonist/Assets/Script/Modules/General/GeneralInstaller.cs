using UnityEngine;
using Zenject;
using UnityEngine.EventSystems;
using TendedTarsier.Script.Utilities.Extensions;
using TendedTarsier.Script.Modules.General.Configs;
using TendedTarsier.Script.Modules.General.Configs.Stats;
using TendedTarsier.Script.Modules.General.Profiles.Stats;
using TendedTarsier.Script.Modules.General.Profiles.Inventory;
using TendedTarsier.Script.Modules.General.Services.Profile;
using TendedTarsier.Script.Modules.General.Services.Input;
using Unity.Netcode;
using MapProfile = TendedTarsier.Script.Modules.General.Profiles.Map.MapProfile;

namespace TendedTarsier.Script.Modules.General
{
    public class GeneralInstaller : MonoInstaller
    {
        [SerializeField]
        private EventSystem _eventSystem;
        [SerializeField]
        private NetworkManager _networkManager;
        [Header("Configs")]
        [SerializeField]
        private GameplayConfig _gameplayConfig;
        [SerializeField]
        private GeneralConfig _generalConfig;
        [SerializeField]
        private InventoryConfig _inventoryConfig;
        [SerializeField]
        private MapConfig _mapConfig;
        [SerializeField]
        private MenuConfig _menuConfig;
        [SerializeField]
        private StatsConfig _statsConfig;
        [SerializeField]
        private PlayerConfig _playerConfig;

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
            Container.Bind<NetworkManager>().FromInstance(_networkManager).AsSingle();
        }

        private void BindServices()
        {
            Container.BindService<ProfileService>();
            Container.BindService<InputService>();
        }

        private void BindProfiles()
        {
            Container.BindProfile<PlayerProfile>();
            Container.BindProfile<StatsProfile>();
            Container.BindProfile<MapProfile>();
            Container.BindProfile<InventoryProfile>();
        }

        private void BindConfigs()
        {
            Container.Bind<PlayerConfig>().FromScriptableObject(_playerConfig).AsSingle().NonLazy();
            Container.Bind<GameplayConfig>().FromScriptableObject(_gameplayConfig).AsSingle().NonLazy();
            Container.Bind<GeneralConfig>().FromInstance(_generalConfig).AsSingle().NonLazy();
            Container.Bind<InventoryConfig>().FromScriptableObject(_inventoryConfig).AsSingle().NonLazy();
            Container.Bind<MapConfig>().FromScriptableObject(_mapConfig).AsSingle().NonLazy();
            Container.Bind<MenuConfig>().FromInstance(_menuConfig).AsSingle().NonLazy();
            Container.Bind<StatsConfig>().FromInstance(_statsConfig).AsSingle().NonLazy();
        }
    }
}