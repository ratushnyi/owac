using System.Linq;
using Zenject;

namespace TendedTarsier.Script.Utilities.Extensions
{
    public static class ContainerExtensions
    {
        public static void BindWithParents<T>(this DiContainer container)
        {
            var current = typeof(T);
            var types = current.GetInterfaces().ToHashSet();
            types.Add(current);
            container.Bind(types).To<T>().AsSingle().NonLazy();
        }
    }
}