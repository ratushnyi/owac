using System.Collections.Generic;
using JetBrains.Annotations;
using MemoryPack;
using UnityEngine;
using Zenject;
using TendedTarsier.Script.Modules.General.Services.Profile;
using TendedTarsier.Script.Modules.Gameplay.Services.Tilemaps;

namespace TendedTarsier.Script.Modules.General.Profiles.Tilemap
{
    [MemoryPackable(GenerateType.VersionTolerant)]
    public partial class MapProfile : ProfileBase
    {
        private GeneralConfig _generalConfig;
        public override string Name => "Map";

        [MemoryPackOrder(0)]
        public List<MapItemModel> MapItemsList { get; [UsedImplicitly] set; } = new();

        [MemoryPackOrder(1)]
        public Dictionary<Vector2Int, TileModel.TileType> ChangedTiles { get; [UsedImplicitly] set; } = new();

        [Inject]
        public void Construct(GeneralConfig generalConfig)
        {
            _generalConfig = generalConfig;
        }

        public override void OnSectionCreated()
        {
            foreach (var mapItem in _generalConfig.MapItemsPreconditionList)
            {
                MapItemsList.Add(new MapItemModel { Position = mapItem.transform.position, ItemEntity = mapItem.ItemEntity });
            }
        }
    }
}