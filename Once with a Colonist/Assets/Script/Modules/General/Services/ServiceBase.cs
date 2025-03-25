using System;
using UniRx;
using Unity.Netcode;
using Zenject;

namespace TendedTarsier.Script.Modules.General.Services
{
    public abstract class ServiceBase : NetworkBehaviour, IDisposable, IInitializable
    {
        protected readonly CompositeDisposable CompositeDisposable = new();

        [Inject]
        private void Inject()
        {
            Observable.OnceApplicationQuit().Subscribe(_ => Terminate());
        }

        public abstract void Initialize();

        private void Terminate()
        {
            Dispose();
        }

        public virtual void Dispose()
        {
            CompositeDisposable?.Dispose();
        }
    }
}