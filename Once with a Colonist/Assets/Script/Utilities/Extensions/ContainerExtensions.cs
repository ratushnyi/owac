using System.Linq;
using TendedTarsier.Script.Modules.General.Panels;
using TendedTarsier.Script.Modules.General.Services;
using TendedTarsier.Script.Modules.General.Services.Profile;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace TendedTarsier.Script.Utilities.Extensions
{
    public static class ContainerExtensions
    {
        public static void BindPanel<TPanel>(this DiContainer container, PanelBase panel, Canvas canvas) where TPanel : PanelBase
        {
            container.BindWithInterfaces<PanelLoader<TPanel>>().FromNew().AsSingle().WithArguments(panel, canvas).NonLazy();
        }

        public static void BindService<TService>(this DiContainer container, NetworkObject networkObject) where TService : ServiceBase
        {
            container.BindWithInterfaces<TService>().FromNewComponentOn(networkObject.gameObject).AsSingle().NonLazy();
        }

        public static void BindProfile<TProfile>(this DiContainer container) where TProfile : ProfileBase
        {
            container.BindWithInterfaces<TProfile>().AsSingle().NonLazy();
        }

        private static FromBinderNonGeneric BindWithInterfaces<T>(this DiContainer container)
        {
            var current = typeof(T);
            var types = current.GetInterfaces().ToHashSet();
            types.Add(current);
            return container.Bind(types).To<T>();
        }
    }
}