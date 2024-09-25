using System;
using MemoryPack;
using UnityEngine;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Inventory.Items
{
    [Serializable, MemoryPackable]
    public partial class ItemEntity : IEquatable<ItemEntity>
    {
        [field: SerializeField]
        public string Id { get; set; }

        [field: SerializeField]
        public int Count { get; set; }

        public bool Equals(ItemEntity other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id && Count == other.Count;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ItemEntity)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Count);
        }
    }
}