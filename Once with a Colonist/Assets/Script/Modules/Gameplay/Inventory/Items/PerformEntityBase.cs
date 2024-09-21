using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TendedTarsier.Script.Modules.Gameplay.Inventory.Items
{
    public class PerformEntityBase : ScriptableObject
    {
        public virtual bool Perform(Tilemap tilemap, Vector3Int targetPosition)
        {
            throw new NotImplementedException();
        }
    }
}