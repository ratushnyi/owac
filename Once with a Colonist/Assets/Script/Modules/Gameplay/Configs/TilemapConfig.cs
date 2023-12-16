using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TendedTarsier
{
    [CreateAssetMenu(menuName = "Config/TilemapConfig", fileName = "TilemapConfig", order = 0)]
    public class TilemapConfig : ScriptableObject
    {
        public TileBase this[TileModel.TileType type] => TilesModels.FirstOrDefault(t => t.Type == type)?.Tile;
        
        [field: SerializeField]
        public List<TileModel> TilesModels { get; set; }
    }
}