using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace TendedTarsier.Script.Modules.General.Panels
{
    public class PanelBase : MonoBehaviour
    {
        protected readonly CompositeDisposable CompositeDisposable = new();

        public virtual UniTask HideAnimation()
        {
            gameObject.SetActive(false);
            return UniTask.CompletedTask;
        }
        
        public virtual UniTask ShowAnimation()
        {
            gameObject.SetActive(true);
            return UniTask.CompletedTask;
        }
        
        public virtual void OnDestroy()
        {
            CompositeDisposable.Dispose();
        }
    }
}