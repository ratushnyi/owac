using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TendedTarsier
{
    [Serializable]
    public class TileModel
    {
        [Serializable]
        public enum TileType
        {
            Stone,
            Grass,
            Sett
        }
        
        [field: SerializeField]
        public TileType Type { get; set; }
        
        [field: SerializeField]
        public TileBase Tile { get; set; }
    }
}