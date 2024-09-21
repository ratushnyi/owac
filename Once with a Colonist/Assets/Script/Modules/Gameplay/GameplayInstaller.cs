using System.Collections.Generic;
using TendedTarsier.Script.Modules.Gameplay.Character;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;
using TendedTarsier.Script.Modules.Gameplay.Configs;
using TendedTarsier.Script.Modules.Gameplay.Services.HUD;
using TendedTarsier.Script.Modules.Gameplay.Services.Inventory;
using TendedTarsier.Script.Modules.Gameplay.Services.Tilemaps;
using TendedTarsier.Script.Utilities.Extensions;

namespace TendedTarsier.Script.Modules.Gameplay
{
    public class GameplayInstaller : MonoInstaller
    {
        public const string ItemsTransformId = "item_transform";

        [Header("Configs")]
        [SerializeField]
        private InventoryConfig _inventoryConfig;
        [SerializeField]
        private TilemapConfig _tilemapConfig;
        [SerializeField]
        private GameplayConfig _gameplayConfig;

        [Header("Common")]
        [SerializeField]
        private List<Tilemap> _tilemaps;
        [SerializeField]
        private GameplayController _gameplayController;
        [SerializeField]
        private PlayerController _playerController;
        [SerializeField]
        private Transform _itemsTransform;

        [Header("UI")]
        [SerializeField]
        private Canvas _gameplayCanvas;

        [Header("Panels")]
        [SerializeField]
        private ToolBarController _toolBarController;
        [SerializeField]
        private InventoryController _inventoryController;

        public override void InstallBindings()
        {
            BindConfigs();
            BindServices();
            BindPanels();
            BindCommon();
        }

        private void BindServices()
        {
            Container.BindWithParents<TilemapService>();
            Container.BindWithParents<InventoryService>();
            Container.BindWithParents<HUDService>();
        }

        private void BindConfigs()
        {
            Container.Bind<InventoryConfig>().FromScriptableObject(_inventoryConfig).AsSingle();
            Container.Bind<TilemapConfig>().FromScriptableObject(_tilemapConfig).AsSingle();
            Container.Bind<GameplayConfig>().FromScriptableObject(_gameplayConfig).AsSingle();
        }

        private void BindPanels()
        {
            Container.Bind<PanelLoader<ToolBarController>>().FromNew().AsSingle().WithArguments(_toolBarController, _gameplayCanvas);
            Container.Bind<PanelLoader<InventoryController>>().FromNew().AsSingle().WithArguments(_inventoryController, _gameplayCanvas);
        }

        private void BindCommon()
        {
            Container.Bind<Transform>().FromInstance(_itemsTransform).AsSingle().WithConcreteId(ItemsTransformId);
            Container.Bind<GameplayController>().FromInstance(_gameplayController).AsSingle();
            Container.Bind<PlayerController>().FromInstance(_playerController).AsSingle();
            Container.Bind<List<Tilemap>>().FromInstance(_tilemaps).AsSingle();
        }
    }
}