using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

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
            //General
            Container.Bind<GameplayInput>().FromNew().AsSingle();
            
            //Configs
            Container.Bind<InventoryConfig>().FromScriptableObject(_inventoryConfig).AsSingle();
            Container.Bind<TilemapConfig>().FromScriptableObject(_tilemapConfig).AsSingle();
            Container.Bind<GameplayConfig>().FromScriptableObject(_gameplayConfig).AsSingle();
            
            //Services
            Container.Bind<TilemapService>().FromNew().AsSingle();
            Container.Bind<InventoryService>().FromNew().AsSingle();
            
            //UI
            Container.Bind<PanelLoader<ToolBarController>>().FromNew().AsSingle().WithArguments(_toolBarController, _gameplayCanvas);
            Container.Bind<PanelLoader<InventoryController>>().FromNew().AsSingle().WithArguments(_inventoryController, _gameplayCanvas);

            //Common
            Container.Bind<Transform>().FromInstance(_itemsTransform).AsSingle().WithConcreteId(ItemsTransformId);
            Container.Bind<GameplayController>().FromInstance(_gameplayController).AsSingle();
            Container.Bind<PlayerController>().FromInstance(_playerController).AsSingle();
            Container.Bind<List<Tilemap>>().FromInstance(_tilemaps).AsSingle();
        }
    }
}