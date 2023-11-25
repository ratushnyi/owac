using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace TendedTarsier
{
    public class InventoryCellView : MonoBehaviour
    {
        [SerializeField]
        private Image _image;
        [SerializeField]
        private TextMeshProUGUI _count;

        private string _id;
        private readonly CompositeDisposable _compositeDisposable = new ();
        
        public void Init(string id, ReactiveProperty<int> count, InventoryItemModel model)
        {
            _id = id;
            
            count.Subscribe(t => _count.SetText(t.ToString())).AddTo(_compositeDisposable);
            _count.enabled = true;
            
            _image.sprite = model.Sprite;
            _image.enabled = true;
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(_id);
        }

        public void Dispose()
        {
            _id = null;
            _image.enabled = false;
            _count.enabled = false;
            _compositeDisposable.Clear();
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}