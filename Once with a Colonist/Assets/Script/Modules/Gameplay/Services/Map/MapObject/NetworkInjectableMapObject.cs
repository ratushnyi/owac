using Cysharp.Threading.Tasks;
using Zenject;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Map.MapObject
{
    public abstract class NetworkInjectableMapObject : MapObjectBase
    {
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            InjectNetworkObject().Forget();
        }

        private async UniTaskVoid InjectNetworkObject()
        {
            await UniTask.Yield();
            gameObject.AddComponent<ZenAutoInjecter>();
            OnNetworkObjectInjected();
        }

        protected abstract void OnNetworkObjectInjected();
    }
}