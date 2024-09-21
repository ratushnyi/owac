using System.Collections.Generic;
using TendedTarsier.Script.Modules.General.Services;
using UnityEngine;
using Zenject;

namespace TendedTarsier
{
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

            var playerProfile = new Script.Modules.Gameplay.Character.PlayerProfile();
            Container.Bind<Script.Modules.Gameplay.Character.PlayerProfile>().FromInstance(playerProfile).AsSingle();
            profileSections.Add(playerProfile);

            var tilemapProfile = new TilemapProfile();
            Container.Bind<TilemapProfile>().FromInstance(tilemapProfile).AsSingle();
            profileSections.Add(tilemapProfile);

            var inventoryProfile = new InventoryProfile();
            Container.Bind<InventoryProfile>().FromInstance(inventoryProfile).AsSingle();
            profileSections.Add(inventoryProfile);

            Container.Bind<ProfileService>().FromInstance(new ProfileService(profileSections)).AsSingle();
        }
    }
}