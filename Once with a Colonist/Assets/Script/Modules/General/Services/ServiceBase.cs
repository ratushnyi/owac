using System;
using UniRx;
using Zenject;

namespace TendedTarsier.Script.Modules.General.Services
{
    public abstract class ServiceBase : IDisposable
    {
        protected readonly CompositeDisposable CompositeDisposable = new();

        [Inject]
        protected virtual void Initialize()
        {
            Observable.OnceApplicationQuit().Subscribe(_ => Terminate());
        }

        private void Terminate()
        {
            Dispose();
        }

        public virtual void Dispose()
        {
            CompositeDisposable.Dispose();
        }
    }
}