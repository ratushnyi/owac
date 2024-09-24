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
using TendedTarsier.Script.Modules.Gameplay.Panels.ToolBar;
using TendedTarsier.Script.Modules.Gameplay.Services.HUD;
using TendedTarsier.Script.Modules.Gameplay.Services.Inventory;
using TendedTarsier.Script.Modules.Gameplay.Services.Stats;
using TendedTarsier.Script.Modules.Gameplay.Services.Tilemaps;

namespace TendedTarsier.Script.Modules.Gameplay
{
    public class GameplayInstaller : MonoInstaller
    {
        public const string PropsTransformId = "props_transform";
        public const string GroundTilemapsId = "ground_tilemaps";

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
        private List<Tilemap> _groundTilemaps;
        [SerializeField]
        private Transform _propsLayerTransform;
        [SerializeField]
        private PlayerController _playerController;

        [Header("UI")]
        [SerializeField]
        private Canvas _gameplayCanvas;

        [Header("Panels")]
        [SerializeField]
        private ToolBarPanel _toolBarPanel;
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
            Container.BindPanel<ToolBarPanel>(_toolBarPanel, _gameplayCanvas);
            Container.BindPanel<InventoryPanel>(_inventoryPanel, _gameplayCanvas);
        }

        private void BindSceneObjects()
        {
            Container.Bind<PlayerController>().FromInstance(_playerController).AsSingle();
            Container.Bind<List<Tilemap>>().WithId(GroundTilemapsId).FromInstance(_groundTilemaps);
            Container.Bind<Transform>().WithId(PropsTransformId).FromInstance(_propsLayerTransform);
        }
    }
}