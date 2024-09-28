using System;
using System.Linq;
using TendedTarsier.Script.Modules.General.Panels;
using TendedTarsier.Script.Modules.General.Services;
using TendedTarsier.Script.Modules.General.Services.Profile;
using UnityEngine;
using Zenject;

namespace TendedTarsier.Script.Utilities.Extensions
{
    public static class ContainerExtensions
    {
        public static void BindPanel<TPanel>(this DiContainer container, PanelBase panel, Canvas canvas) where TPanel : PanelBase
        {
            container.BindWithParents<PanelLoader<TPanel>>().FromNew().AsSingle().WithArguments(panel, canvas).NonLazy();
        }

        public static void BindService<TService>(this DiContainer container) where TService : ServiceBase
        {
            BindWithParents<TService>(container).AsSingle().NonLazy();
        }

        public static void BindProfile<TProfile>(this DiContainer container) where TProfile : ProfileBase
        {
            BindWithParents<TProfile>(container).AsSingle().NonLazy();
        }

        private static FromBinderNonGeneric BindWithParents<T>(this DiContainer container)
        {
            var current = typeof(T);
            var types = current.GetInterfaces().ToHashSet();
            types.Add(current);
            return container.Bind(types).To<T>();
        }
    }
}