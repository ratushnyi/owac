using System.Linq;
using TendedTarsier.Script.Modules.General.Panels;
using TendedTarsier.Script.Modules.General.Profile;
using TendedTarsier.Script.Modules.General.Services;
using UnityEngine;
using Zenject;

namespace TendedTarsier.Script.Utilities.Extensions
{
    public static class ContainerExtensions
    {
        public static void BindPanel<TPanel>(this DiContainer container, PanelBase panel, Canvas canvas) where TPanel : PanelBase
        {
            container.Bind<PanelLoader<TPanel>>().FromNew().AsSingle().WithArguments(panel, canvas);
        }

        public static void BindService<TService>(this DiContainer container) where TService : ServiceBase
        {
            BindWithParents<TService>(container);
        }

        public static void BindProfile<TProfile>(this DiContainer container) where TProfile : ProfileBase
        {
            BindWithParents<TProfile>(container);
        }

        private static void BindWithParents<T>(this DiContainer container)
        {
            var current = typeof(T);
            var types = current.GetInterfaces().ToHashSet();
            types.Add(current);
            container.Bind(types).To<T>().AsSingle().NonLazy();
        }
    }
}