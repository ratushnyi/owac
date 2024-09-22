using UnityEngine;
using UnityEngine.Tilemaps;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Inventory.Items
{
    public class PerformEntityBase : ScriptableObject
    {
        public virtual bool Perform(Tilemap tilemap, Vector3Int targetPosition)
        {
            return false;
        }
    }
}