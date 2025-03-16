using System;
using UniRx;
using Zenject;

namespace TendedTarsier.Script.Modules.General.Services
{
    public abstract class ServiceBase : IDisposable, IInitializable
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
            CompositeDisposable?.Dispose();
            Dispose();
        }

        public virtual void Dispose()
        {
        }
    }
}