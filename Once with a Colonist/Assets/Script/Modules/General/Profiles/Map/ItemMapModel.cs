using MemoryPack;
using UnityEngine;
using TendedTarsier.Script.Modules.Gameplay.Services.Inventory.Items;
using Unity.Netcode;

namespace TendedTarsier.Script.Modules.General.Profiles.Map
{
    [MemoryPackable]
    public partial class ItemMapModel: INetworkSerializable
    {
        [MemoryPackAllowSerialize]
        private Vector3 _position;
        [MemoryPackAllowSerialize]
        private int _sortingLayerID;
        [MemoryPackAllowSerialize]
        private int _layerID;
        [MemoryPackAllowSerialize]
        private ItemEntity _itemEntity;

        public Vector3 Position
        {
            get => _position;
            set => _position = value;
        }

        public int SortingLayerID
        {
            get => _sortingLayerID;
            set => _sortingLayerID = value;
        }

        public int LayerID
        {
            get => _layerID;
            set => _layerID = value;
        }

        public ItemEntity ItemEntity
        {
            get => _itemEntity;
            set => _itemEntity = value;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _position);
            serializer.SerializeValue(ref _sortingLayerID);
            serializer.SerializeValue(ref _layerID);
            serializer.SerializeNetworkSerializable(ref _itemEntity);
        }
    }
}