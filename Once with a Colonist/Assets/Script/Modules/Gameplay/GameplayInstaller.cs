using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;
using TendedTarsier.Script.Utilities.Extensions;
using TendedTarsier.Script.Modules.Gameplay.Character;
using TendedTarsier.Script.Modules.Gameplay.Configs.Gameplay;
using TendedTarsier.Script.Modules.Gameplay.Configs.Inventory;
using TendedTarsier.Script.Modules.Gameplay.Configs.Stats;
using TendedTarsier.Script.Modules.Gameplay.Configs.Tilemap;
using TendedTarsier.Script.Modules.Gameplay.Panels.HUD;
using TendedTarsier.Script.Modules.Gameplay.Services.HUD;
using TendedTarsier.Script.Modules.Gameplay.Services.Inventory;
using TendedTarsier.Script.Modules.Gameplay.Services.Stats;
using TendedTarsier.Script.Modules.Gameplay.Services.Tilemaps;

namespace TendedTarsier.Script.Modules.Gameplay
{
    public class GameplayInstaller : MonoInstaller
    {
        public const string GroundTilemapsListId = "ground_tilemaps_list";
        public const string MapItemsContainerTransformId = "map_items_container_transform";

        [Header("Configs")]
        [SerializeField]
        private InventoryConfig _inventoryConfig;
        [SerializeField]
        private TilemapConfig _tilemapConfig;
        [SerializeField]
        private GameplayConfig _gameplayConfig;
        [SerializeField]
        private StatsConfig _statsConfig;

        [Header("Common")]
        [SerializeField]
        private PlayerController _playerController;
        [SerializeField]
        private Transform _mapItemsContainerTransform;
        [SerializeField]
        private List<Tilemap> _groundTilemapsList;

        [Header("UI")]
        [SerializeField]
        private Canvas _gameplayCanvas;

        [Header("Panels")]
        [SerializeField]
        private InputPanel _inputPanel;
        [SerializeField]
        private HUDPanel _hudPanel;
        [SerializeField]
        private InventoryPanel _inventoryPanel;

        public override void InstallBindings()
        {
            BindConfigs();
            BindServices();
            BindPanels();
            BindSceneObjects();
        }

        private void BindServices()
        {
            Container.BindService<TilemapService>();
            Container.BindService<InventoryService>();
            Container.BindService<HUDService>();
            Container.BindService<StatsService>();
            Container.BindService<MapService>();
        }

        private void BindConfigs()
        {
            Container.Bind<InventoryConfig>().FromScriptableObject(_inventoryConfig).AsSingle().NonLazy();
            Container.Bind<TilemapConfig>().FromScriptableObject(_tilemapConfig).AsSingle().NonLazy();
            Container.Bind<GameplayConfig>().FromScriptableObject(_gameplayConfig).AsSingle().NonLazy();
            Container.Bind<StatsConfig>().FromInstance(_statsConfig).AsSingle().NonLazy();
        }

        private void BindPanels()
        {
            Container.BindPanel<InputPanel>(_inputPanel, _gameplayCanvas);
            Container.BindPanel<HUDPanel>(_hudPanel, _gameplayCanvas);
            Container.BindPanel<InventoryPanel>(_inventoryPanel, _gameplayCanvas);
        }

        private void BindSceneObjects()
        {
            Container.Bind<PlayerController>().FromInstance(_playerController).AsSingle();
            Container.Bind<List<Tilemap>>().WithId(GroundTilemapsListId).FromInstance(_groundTilemapsList);
            Container.Bind<Transform>().WithId(MapItemsContainerTransformId).FromInstance(_mapItemsContainerTransform);
        }
    }
}