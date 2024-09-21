using System.Linq;
using TendedTarsier.Script.Modules.General.Services;
using Zenject;

namespace TendedTarsier.Script.Utilities.Extensions
{
    public static class ContainerExtensions
    {
        public static void BindService<TService>(this DiContainer container) where TService : ServiceBase
        {
            var current = typeof(TService);
            var types = current.GetInterfaces().ToHashSet();
            types.Add(current);
            container.Bind(types).To<TService>().AsSingle().NonLazy();
        }

    }
}
