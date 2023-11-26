using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace TendedTarsier
{
    [UsedImplicitly]
    public class PanelLoader<T> where T : MonoBehaviour
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

        public void Show()
        {
            if (Instance != null)
            {
                Debug.LogError($"You try to Show {nameof(T)} panel, but it already exist.");
                return;
            }

            Instance = _container.InstantiatePrefabForComponent<T>(_prefab, _canvas.transform);
        }

        public void Hide()
        {
            if (Instance == null)
            {
                Debug.LogError($"You try to Hide {nameof(T)} panel, but it not been Showed.");
                return;
            }
            
            Object.DestroyImmediate(Instance.gameObject);
        }
    }
}