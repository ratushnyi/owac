using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
namespace TendedTarsier
{
    public class PerformEntityBase : ScriptableObject
    {
        public virtual bool Perform(Tilemap tilemap, Vector3Int targetPosition)
        {
            throw new NotImplementedException();
        }
    }

}