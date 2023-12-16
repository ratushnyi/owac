using UniRx;
using Zenject;

namespace TendedTarsier
{
    public abstract class ServiceBase
    {
        protected readonly CompositeDisposable CompositeDisposable = new();

        [Inject]
        protected virtual void Initialize()
        {
            Observable.OnceApplicationQuit().Subscribe(_ => Terminate());
        }

        private void Terminate()
        {
            CompositeDisposable.Dispose();
            Dispose();
        }

        protected virtual void Dispose() { }
    }
}