using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

namespace TendedTarsier
{
    public class GameplayInstaller : MonoInstaller
    {
        [SerializeField]
        private List<Tilemap> _tilemaps;
        [SerializeField]
        private TilemapConfig _tilemapConfig;
        [SerializeField]
        private GameplayConfig _gameplayConfig;
        [SerializeField]
        private GameplayController _gameplayController;

        public override void InstallBindings()
        {
            Container.Bind<GameplayInput>().FromNew().AsSingle();
            Container.Bind<List<Tilemap>>().FromInstance(_tilemaps).AsSingle();
            Container.Bind<TilemapConfig>().FromInstance(_tilemapConfig).AsSingle();
            Container.Bind<GameplayConfig>().FromInstance(_gameplayConfig).AsSingle();
            Container.Bind<GameplayController>().FromInstance(_gameplayController).AsSingle();
            Container.Bind<TilemapService>().FromNew().AsSingle();
            Container.Bind<InventoryService>().FromNew().AsSingle();
        }
    }
}