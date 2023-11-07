using UnityEngine;
using Zenject;

public class GameplayInstaller : MonoInstaller
{
    [SerializeField]
    private GameplayConfig _gameplayConfig;

    public override void InstallBindings()
    {
        Container.Bind<GameplayConfig>().FromInstance(_gameplayConfig).AsSingle();
    }
}