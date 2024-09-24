using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace TendedTarsier.Script.Modules.General.Panels
{
    public abstract class PanelBase : MonoBehaviour
    {
        public bool ShowInstantly;
        protected readonly CompositeDisposable CompositeDisposable = new();

        public virtual UniTask InitializeAsync()
        {
            Initialize();
            return UniTask.CompletedTask;
        }

        protected virtual void Initialize()
        {
        }

        public virtual UniTask DisposeAsync()
        {
            Dispose();
            return UniTask.CompletedTask;
        }

        protected virtual void Dispose()
        {
        }

        public virtual UniTask ShowAnimation()
        {
            gameObject.SetActive(true);
            return UniTask.CompletedTask;
        }

        public virtual UniTask HideAnimation()
        {
            gameObject.SetActive(false);
            return UniTask.CompletedTask;
        }

        public virtual void OnDestroy()
        {
            CompositeDisposable.Dispose();
        }
    }
}