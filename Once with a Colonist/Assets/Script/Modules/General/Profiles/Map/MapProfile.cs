using System.Collections.Generic;
using JetBrains.Annotations;
using MemoryPack;
using TendedTarsier.Script.Modules.Gameplay.Services.Tilemaps;
using TendedTarsier.Script.Modules.General.Configs;
using TendedTarsier.Script.Modules.General.Services.Profile;
using UnityEngine;
using Zenject;

namespace TendedTarsier.Script.Modules.General.Profiles.Map
{
    [MemoryPackable(GenerateType.VersionTolerant)]
    public partial class MapProfile : ProfileBase
    {
        private MapConfig _mapConfig;
        public override string Name => "Map";

        [MemoryPackOrder(0)]
        public List<ItemMapModel> MapItemsList { get; [UsedImplicitly] set; } = new();

        [MemoryPackOrder(1)]
        public Dictionary<Vector2Int, TileModel.TileType> ChangedTiles { get; [UsedImplicitly] set; } = new();

        [Inject]
        public void Construct(MapConfig mapConfig)
        {
            _mapConfig = mapConfig;
        }

        public override void OnSectionCreated()
        {
            foreach (var mapItem in _mapConfig.ItemMapObjectsPreconditionList)
            {
                MapItemsList.Add(new ItemMapModel
                {
                    Position = mapItem.transform.position,
                    ItemEntity = mapItem.ItemEntity,
                    SortingLayerID = mapItem.SpriteRenderer.sortingLayerID,
                    LayerID = mapItem.gameObject.layer
                });
            }
        }
    }
}