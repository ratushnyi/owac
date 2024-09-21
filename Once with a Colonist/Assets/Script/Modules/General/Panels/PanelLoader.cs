using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace TendedTarsier.Script.Modules.General.Panels
{
    [UsedImplicitly]
    public class PanelLoader<T> where T : PanelBase
    {
        private readonly T _prefab;
        private readonly Canvas _canvas;
        private readonly DiContainer _container;

        public T Instance;

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
                Debug.LogError($"You try to Show {nameof(T)} panel, but it already exist.");
                return Instance;
            }
            
            await Load();
            await PlayAnimation(true);
            return Instance;
        }

        public async UniTask Hide()
        {
            if (Instance == null)
            {
                Debug.LogError($"You try to Hide {nameof(T)} panel, but it not been Showed.");
                return;
            }

            await PlayAnimation(false);
            await Unload();
        }
        
        private UniTask PlayAnimation(bool isShowing)
        {
            Instance.gameObject.SetActive(isShowing);
            return UniTask.CompletedTask;
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