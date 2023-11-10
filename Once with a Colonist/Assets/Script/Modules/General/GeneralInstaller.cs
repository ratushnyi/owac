using NaughtyAttributes;
using TendedTarsier;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class GeneralInstaller : MonoInstaller
{
    [SerializeField, Scene]
    private string _nextScene;

    [SerializeField]
    private GeneralConfig _generalConfig;

    public override void InstallBindings()
    {
        Container.Bind<GeneralProfile>().FromNew().AsSingle();
        Container.Bind<ProfileService>().FromNew().AsSingle();
        Container.Bind<GeneralConfig>().FromInstance(_generalConfig).AsSingle();

        // SceneManager.LoadScene(_nextScene, LoadSceneMode.Additive);
    }
}