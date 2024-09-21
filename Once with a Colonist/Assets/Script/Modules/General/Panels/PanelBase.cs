using UniRx;
using UnityEngine;

namespace TendedTarsier.Script.Modules.General.Panels
{
    public class PanelBase : MonoBehaviour
    {
        protected readonly CompositeDisposable CompositeDisposable = new();

        public virtual void OnDestroy()
        {
            CompositeDisposable.Dispose();
        }
    }
}