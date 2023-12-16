using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace TendedTarsier
{
    public class InventoryCellView : MonoBehaviour
    {
        private readonly CompositeDisposable _compositeDisposable = new();
        private readonly ISubject<string> _onButtonClicked = new Subject<string>();

        [SerializeField]
        private Image _image;
        [SerializeField]
        private Button _button;
        [SerializeField]
        private TextMeshProUGUI _countTMP;

        private ItemModel _model;

        public IObservable<string> OnButtonClicked => _onButtonClicked;

        public void SetItem(ItemModel model, ReactiveProperty<int> count)
        {
            count.Subscribe(OnCountChanged).AddTo(_compositeDisposable);

            _model = model;
            _image.sprite = _model.Sprite;
            _image.enabled = true;
            _countTMP.enabled = true;
            _button.OnClickAsObservable().Subscribe(_ => _onButtonClicked.OnNext(_model.Id)).AddTo(_compositeDisposable);
        }

        public bool IsEmpty()
        {
            return _model == null;
        }

        private void OnCountChanged(int count)
        {
            if (count == 0)
            {
                Dispose();
                return;
            }

            _countTMP.SetText(count.ToString());
        }

        public void Dispose()
        {
            _model = null;
            _image.sprite = null;
            _image.enabled = false;
            _countTMP.enabled = false;
            _compositeDisposable.Clear();
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}