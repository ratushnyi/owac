using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

namespace TendedTarsier
{
    public class GameplayInstaller : MonoInstaller
    {
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
        private Canvas _gameplayCanvas;
        [SerializeField]
        private PlayerController _playerController;

        public override void InstallBindings()
        {
            //Configs
            Container.Bind<InventoryConfig>().FromInstance(_inventoryConfig).AsSingle();
            Container.Bind<TilemapConfig>().FromInstance(_tilemapConfig).AsSingle();
            Container.Bind<GameplayConfig>().FromInstance(_gameplayConfig).AsSingle();
            
            //Services
            Container.Bind<TilemapService>().FromNew().AsSingle();
            Container.Bind<InventoryService>().FromNew().AsSingle();
            
            //Common
            Container.Bind<List<Tilemap>>().FromInstance(_tilemaps).AsSingle();
            Container.Bind<Canvas>().FromInstance(_gameplayCanvas).AsSingle();
            Container.Bind<GameplayController>().FromInstance(_gameplayController).AsSingle();
            Container.Bind<GameplayInput>().FromNew().AsSingle();
            Container.Bind<PlayerController>().FromInstance(_playerController).AsSingle();
        }
    }
}