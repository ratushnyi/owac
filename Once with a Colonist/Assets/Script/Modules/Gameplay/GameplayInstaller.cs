using System.Collections.Generic;
using TendedTarsier.Script.Modules.Gameplay.Character;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;
using TendedTarsier.Script.Utilities.Extensions;
using TendedTarsier.Script.Modules.Gameplay.ToolBar;
using TendedTarsier.Script.Modules.Gameplay.Configs;
using TendedTarsier.Script.Modules.Gameplay.Services.HUD;
using TendedTarsier.Script.Modules.Gameplay.Services.Inventory;
using TendedTarsier.Script.Modules.Gameplay.Services.Tilemaps;
using TendedTarsier.Script.Modules.General.Panels;
using UnityEngine.Serialization;

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

        [FormerlySerializedAs("_toolBarController")]
        [Header("Panels")]
        [SerializeField]
        private ToolBarPanel _toolBarPanel;
        [FormerlySerializedAs("_inventoryController")]
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
        }

        private void BindConfigs()
        {
            Container.Bind<InventoryConfig>().FromScriptableObject(_inventoryConfig).AsSingle();
            Container.Bind<TilemapConfig>().FromScriptableObject(_tilemapConfig).AsSingle();
            Container.Bind<GameplayConfig>().FromScriptableObject(_gameplayConfig).AsSingle();
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