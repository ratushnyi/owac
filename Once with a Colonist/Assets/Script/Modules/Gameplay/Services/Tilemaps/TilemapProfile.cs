using System.Collections.Generic;
using JetBrains.Annotations;
using MemoryPack;
using UnityEngine;
using TendedTarsier.Script.Modules.General.Profile;
using TendedTarsier.Script.Modules.General.Services.Profile;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Tilemaps
{
    [MemoryPackable(GenerateType.VersionTolerant)]
    public partial class TilemapProfile : ProfileBase
    {
        public override string Name => "Tilemap";

        [MemoryPackOrder(0)]
        public Dictionary<Vector2Int, TileModel.TileType> ChangedTiles { get; [UsedImplicitly] set; } = new ();
    }
}