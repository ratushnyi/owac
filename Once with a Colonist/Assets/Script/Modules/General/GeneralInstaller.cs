using System.Collections.Generic;
using TendedTarsier.Script.Modules.Gameplay.Character;
using TendedTarsier.Script.Modules.General.Services;
using TendedTarsier.Script.Modules.General.Services.Profile;
using TendedTarsier.Script.Utilities.Extensions;
using UnityEngine;
using Zenject;

namespace TendedTarsier.Script.Modules.General
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
            Container.BindWithParents<PlayerProfile>();
            Container.BindWithParents<Gameplay.Services.Tilemaps.TilemapProfile>();
            Container.BindWithParents<Gameplay.Services.Inventory.InventoryProfile>();
            
            Container.BindWithParents<ProfileService>();
        }
    }
}