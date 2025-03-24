using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Map.MapObject
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public abstract class MapObjectBase : NetworkBehaviour
    {
        [field: SerializeField]
        public Collider2D Collider { get; set; }

        [field: SerializeField]
        public SpriteRenderer SpriteRenderer { get; set; }

        private void Start()
        {
            Collider.isTrigger = true;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            InitializeNetworkObject().Forget();
        }

        private async UniTaskVoid InitializeNetworkObject()
        {
            await UniTask.Yield();
            gameObject.AddComponent<ZenAutoInjecter>();
            OnNetworkInitialized();
        }

        protected abstract void OnNetworkInitialized();
    }
}