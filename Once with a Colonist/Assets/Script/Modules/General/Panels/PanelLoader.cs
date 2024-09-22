using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace TendedTarsier.Script.Modules.General.Panels
{
    [UsedImplicitly]
    public class PanelLoader<T> where T : PanelBase
    {
        public enum State
        {
            Hide = 0,
            Show = 1,
            Hiding = 2,
            Showing = 3,
        }

        private readonly T _prefab;
        private readonly Canvas _canvas;
        private readonly DiContainer _container;

        public T Instance;
        public State PanelState;

        private PanelLoader(T prefab, Canvas canvas, DiContainer container)
        {
            _prefab = prefab;
            _canvas = canvas;
            _container = container;
        }

        public async UniTask<T> Show()
        {
            if (Instance != null)
            {
                Debug.LogError($"You try to Show {nameof(T)} panel, but it already Showed.");
                return Instance;
            }

            PanelState = State.Showing;
            await Load();
            await Instance.InitializeAsync();
            await Instance.ShowAnimation();
            PanelState = State.Show;
            return Instance;
        }

        public async UniTask Hide()
        {
            if (Instance == null)
            {
                Debug.LogError($"You try to Hide {nameof(T)} panel, but it not been Showed.");
                return;
            }

            PanelState = State.Hiding;
            await Instance.HideAnimation();
            await Instance.DisposeAsync();
            await Unload();
            PanelState = State.Hide;
        }

        private UniTask Load()
        {
            Instance = _container.InstantiatePrefabForComponent<T>(_prefab, _canvas.transform);
            return UniTask.CompletedTask;
        }

        private UniTask Unload()
        {
            Object.DestroyImmediate(Instance.gameObject);
            return UniTask.CompletedTask;
        }
    }
}