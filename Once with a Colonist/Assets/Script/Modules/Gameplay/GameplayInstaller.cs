using UnityEngine;
using Zenject;

namespace TendedTarsier
{
    public class GameplayInstaller : MonoInstaller
    {
        [SerializeField]
        private GameplayConfig _gameplayConfig;

        [SerializeField]
        private GameplayController _gameplayController;

        public override void InstallBindings()
        {
            Container.Bind<GameplayInput>().FromNew().AsSingle();
            Container.Bind<GameplayConfig>().FromInstance(_gameplayConfig).AsSingle();
            Container.Bind<GameplayController>().FromInstance(_gameplayController).AsSingle();
        }
    }
}