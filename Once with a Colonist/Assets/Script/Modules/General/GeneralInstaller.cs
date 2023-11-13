using System.Collections.Generic;
using TendedTarsier;
using UnityEngine;
using Zenject;

public class GeneralInstaller : MonoInstaller
{
    [SerializeField]
    private GeneralConfig _generalConfig;

    public override void InstallBindings()
    {
        BindProfiles();
        
        Container.Bind<GeneralConfig>().FromInstance(_generalConfig).AsSingle();
    }

    private void BindProfiles()
    {
        var profileSections = new List<ProfileBase>();
        
        var generalProfile = new GameplayProfile();
        Container.Bind<GameplayProfile>().FromInstance(generalProfile).AsSingle();
        profileSections.Add(generalProfile);
        
        var tilemapProfile = new TilemapProfile();
        Container.Bind<TilemapProfile>().FromInstance(tilemapProfile).AsSingle();
        profileSections.Add(tilemapProfile);

        Container.Bind<ProfileService>().FromInstance(new ProfileService(profileSections)).AsSingle();
    }
}