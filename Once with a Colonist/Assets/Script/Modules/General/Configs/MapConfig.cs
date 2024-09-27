using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using TendedTarsier.Script.Modules.Gameplay.Services.Tilemaps;
using TendedTarsier.Script.Modules.Gameplay.Services.Map.MapObject;

namespace TendedTarsier.Script.Modules.General.Configs
{
    [CreateAssetMenu(menuName = "Config/MapConfig", fileName = "MapConfig", order = 0)]
    public class MapConfig : ScriptableObject
    {
        public TileBase this[TileModel.TileType type] => TileModelsList.FirstOrDefault(t => t.Type == type)?.Tile;

        [field: SerializeField]
        public List<TileModel> TileModelsList { get; set; }

        [field: SerializeField]
        public ItemMapObject ItemMapObjectPrefab { get; set; }

        [field: SerializeField]
        public List<ItemMapObject> ItemMapObjectsPreconditionList { get; set; } = new();

        [field: SerializeField]
        public float ItemMapActivationDelay { get; set; } = 1;
    }
}