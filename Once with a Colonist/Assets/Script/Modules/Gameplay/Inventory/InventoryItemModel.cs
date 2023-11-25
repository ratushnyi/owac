using System;
using UnityEngine;

namespace TendedTarsier
{
    [Serializable]
    public class InventoryItemModel
    {
        [field: SerializeField]
        public string Id { get; set; }
        [field: SerializeField]
        public Sprite Sprite { get; set; }
    }
}
