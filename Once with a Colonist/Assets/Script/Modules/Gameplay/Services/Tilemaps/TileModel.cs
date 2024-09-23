using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Tilemaps
{
    [Serializable]
    public class TileModel
    {
        [Serializable]
        public enum TileType
        {
            Default,
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